from cryptography.hazmat.primitives.asymmetric import padding as asym_padding
from cryptography.hazmat.primitives.ciphers import Cipher, algorithms, modes
from cryptography.hazmat.primitives import serialization, padding, hashes
from cryptography import x509
import generate_certificates
from pathlib import Path
import base64
import shutil
import json
import os

CERT_FOLDER = Path("certificates")
CERT_JSON_FILE = Path("certificate_data.json")
SQL_OUTPUT_FILE = Path("certificates.sql")
RSA_MASTER_PUB_FILE = Path("master_rsa_public.pem")
AES_NONCE_SIZE = 12
AES_KEY_SIZE = 32


def delete_cert_folder():
    if CERT_FOLDER.exists() and CERT_FOLDER.is_dir():
        shutil.rmtree(CERT_FOLDER)
        print(f"Deleted folder {CERT_FOLDER}")


def regenerate_certificates():
    generate_certificates.generate_certificates(CERT_FOLDER)
    print("Certificates regenerated.")


def load_certificates():
    certs = {}
    for cert_file in CERT_FOLDER.glob("*.cert.pem"):
        name = cert_file.stem.replace(".cert", "")
        key_file = CERT_FOLDER / f"{name}.key.pem"
        with open(cert_file, "rb") as f:
            cert = x509.load_pem_x509_certificate(f.read())
        if key_file.exists():
            private_key_obj = serialization.load_pem_private_key(key_file.read_bytes(), password=None)
            private_key_bytes = private_key_obj.private_bytes(serialization.Encoding.DER, serialization.PrivateFormat.PKCS8, serialization.NoEncryption())
        certs[name] = {"cert": cert, "key_bytes": private_key_bytes}
    return certs


def generate_user_aes_keys(user_ids):
    keys = {}
    for uid in user_ids:
        key = os.urandom(AES_KEY_SIZE)
        keys[uid] = key
        print(f"User {uid} AES key (raw, base64): {base64.b64encode(key).decode('ascii')}")
    return keys


def encrypt_with_aes(key_bytes, plaintext_bytes):
    iv = os.urandom(16)
    padder = padding.PKCS7(128).padder()
    padded = padder.update(plaintext_bytes) + padder.finalize()
    cipher = Cipher(algorithms.AES(key_bytes), modes.CBC(iv))
    encryptor = cipher.encryptor()
    ct = encryptor.update(padded) + encryptor.finalize()
    return base64.b64encode(iv + ct).decode("ascii")


def encrypt_master_key(master_key, rsa_pub_file):
    with open(rsa_pub_file, "rb") as f:
        rsa_pub = serialization.load_pem_public_key(f.read())
    encrypted = rsa_pub.encrypt(
        master_key,
        asym_padding.OAEP(
            mgf=asym_padding.MGF1(algorithm=hashes.SHA256()),
            algorithm=hashes.SHA256(),
            label=None
        )
    )
    return base64.b64encode(encrypted).decode("ascii")


def generate_sql_script(certs, json_data, master_aes_key):
    lines = ["-- Certificates\nINSERT INTO unified.certificates "
             "(serial_number, issued_by, issued_to, not_before, not_after, encoded_value, private_key, can_sign, path_len, signing_certificate_serial_number, signed_by_id) \nVALUES "]
    values_list = []

    user_ids = {entry.get("SignedById") for entry in json_data if entry.get("SignedById")}
    user_aes_keys = generate_user_aes_keys(user_ids)
    user_keys_encrypted_with_master = {}

    for uid, key_bytes in user_aes_keys.items():
        user_keys_encrypted_with_master[uid] = encrypt_with_aes(master_aes_key, key_bytes)
    user_key_inserts = [f"    ('{uid}', '{val}')" for uid, val in user_keys_encrypted_with_master.items()]

    master_key_encrypted_b64 = encrypt_master_key(master_aes_key, RSA_MASTER_PUB_FILE)

    for cert_json in json_data:
        name = cert_json["Name"]
        entry = certs.get(name)
        if not entry:
            print(f"Warning: certificate {name} not found")
            continue

        cert = entry["cert"]
        key_bytes = entry["key_bytes"]
        serial_number = str(cert.serial_number)
        issued_by = cert.issuer.rfc4514_string().replace("'", "''")
        issued_to = cert.subject.rfc4514_string().replace("'", "''")
        not_before = cert.not_valid_before_utc.strftime("%Y-%m-%d %H:%M:%S.%f +00:00")
        not_after = cert.not_valid_after_utc.strftime("%Y-%m-%d %H:%M:%S.%f +00:00")
        encoded_value = ''.join(cert.public_bytes(serialization.Encoding.PEM).decode("utf-8").strip().splitlines()[1:-1]).replace("'", "''")

        signed_by_id = cert_json.get("SignedById")
        private_key = "NULL"
        if key_bytes and signed_by_id:
            encrypted_key = encrypt_with_aes(user_aes_keys[signed_by_id], key_bytes)
            private_key = f"e'{encrypted_key}'"

        can_sign = "true" if cert_json.get("BasicConstraints", {}).get("IsCa", False) else "false"
        path_len = cert_json.get("BasicConstraints", {}).get("PathLen")
        path_len_str = str(path_len) if path_len is not None else "-1"
        issuer_name = cert_json.get("Issuer")
        signing_serial = f"'{certs[issuer_name]['cert'].serial_number}'" if issuer_name and issuer_name in certs else "NULL"
        signed_by_id_str = f"'{signed_by_id}'" if signed_by_id else "NULL"

        value_line = (
            f"    ('{serial_number}', '{issued_by}', '{issued_to}', '{not_before}', '{not_after}', '{encoded_value}', "
            f"{private_key}, {can_sign}, {path_len_str}, {signing_serial}, {signed_by_id_str})"
        )
        values_list.append(value_line)

    lines.append(",\n".join(values_list) + ";\n")
    
    lines.append("\n-- Certificate-User Linking\nINSERT INTO unified.certificate_user (my_certificates_serial_number, user_id)\nVALUES ")
    certificate_user_lines = []
    for cert_json in json_data:
        name = cert_json["Name"]
        entry = certs.get(name)
        if not entry:
            continue
        for uid in cert_json.get("UserIds", []):
            certificate_user_lines.append(f"    ('{entry['cert'].serial_number}', '{uid}')")
    if certificate_user_lines:
        lines.append(",\n".join(certificate_user_lines) + ";\n")
    
    lines.append(f"\n-- Master AES key encrypted with RSA public key\nINSERT INTO unified.master_keys (id, encrypted_key)")
    lines.append(f"VALUES \n    ('00000000-0000-0000-0000-000000000000', '{master_key_encrypted_b64}');\n")
    
    lines.append("\n-- Per user encrypted AES keys used for encrypting certificate private keys\nINSERT INTO unified.user_keys (user_id, encrypted_key) \nVALUES ")
    lines.append(",\n".join(user_key_inserts) + ";\n")
    
    lines.append("\n-- CRL\nINSERT INTO unified.revoked_certificates (revocation_reason, certificate_serial_number) \nVALUES ")
    lines.append(f"    ('{1}', '{str(certs.get("Dunder Mifflin Intermediate")["cert"].serial_number)}');\n")

    with open(SQL_OUTPUT_FILE, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print(f"SQL script written to {SQL_OUTPUT_FILE}")


def regenerate_and_generate_sql():
    delete_cert_folder()
    regenerate_certificates()
    with open(CERT_JSON_FILE, "r", encoding="utf-8") as f:
        json_data = json.load(f)
    certs = load_certificates()
    
    master_aes_key = os.urandom(AES_KEY_SIZE)
    print(f"Master AES key (raw, base64): {base64.b64encode(master_aes_key).decode('ascii')}")
    
    generate_sql_script(certs, json_data, master_aes_key)


if __name__ == "__main__":
    regenerate_and_generate_sql()
