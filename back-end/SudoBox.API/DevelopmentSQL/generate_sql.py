from cryptography.hazmat.primitives import serialization
from cryptography import x509
import generate_certificates
from pathlib import Path
import shutil
import json

CERT_FOLDER = Path("certificates")
CERT_JSON_FILE = Path("certificate_data.json")
SQL_OUTPUT_FILE = Path("certificates.sql")


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
        private_key_str = None
        if key_file.exists():
            with open(key_file, "r") as kf:
                private_key_str = kf.read().replace("\n", "\\n").replace("'", "''")
        certs[name] = {"cert": cert, "key": private_key_str}
    return certs


def generate_sql_script(certs, json_data):
    lines = ["INSERT INTO unified.certificates "
             "(serial_number, issued_by, issued_to, not_before, not_after, encoded_value, is_approved, private_key, can_sign, path_len, signing_certificate_serial_number) \nVALUES "]
    values_list = []

    for cert_json in json_data:
        name = cert_json["Name"]
        entry = certs.get(name)
        if not entry:
            print(f"Warning: certificate {name} not found")
            continue

        cert = entry["cert"]
        key = entry["key"]
        serial_number = str(cert.serial_number)
        issued_by = cert.issuer.rfc4514_string().replace("'", "''")
        issued_to = cert.subject.rfc4514_string().replace("'", "''")
        not_before = cert.not_valid_before_utc.strftime("%Y-%m-%d %H:%M:%S.%f +00:00")
        not_after = cert.not_valid_after_utc.strftime("%Y-%m-%d %H:%M:%S.%f +00:00")
        encoded_value = ''.join(cert.public_bytes(serialization.Encoding.PEM).decode("utf-8").strip().splitlines()[1:-1]).replace("'", "''")

        is_approved = "true"
        private_key = f"e'{key}'" if key else "NULL"
        can_sign = "true" if cert_json.get("BasicConstraints", {}).get("IsCa", False) else "false"
        path_len = cert_json.get("BasicConstraints", {}).get("PathLen")
        path_len_str = str(path_len) if path_len is not None else "-1"
        issuer_name = cert_json.get("Issuer")
        signing_serial = f"'{certs[issuer_name]['cert'].serial_number}'" if issuer_name and issuer_name in certs else "NULL"

        value_line = (
            f"    ('{serial_number}', '{issued_by}', '{issued_to}', '{not_before}', '{not_after}', '{encoded_value}', "
            f"{is_approved}, {private_key}, {can_sign}, {path_len_str}, {signing_serial})"
        )
        values_list.append(value_line)

    lines.append(",\n".join(values_list) + ";\n")
    with open(SQL_OUTPUT_FILE, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print(f"SQL script written to {SQL_OUTPUT_FILE}")


def regenerate_and_generate_sql():
    delete_cert_folder()
    regenerate_certificates()
    with open(CERT_JSON_FILE, "r", encoding="utf-8") as f:
        json_data = json.load(f)
    certs = load_certificates()
    generate_sql_script(certs, json_data)


if __name__ == "__main__":
    regenerate_and_generate_sql()
