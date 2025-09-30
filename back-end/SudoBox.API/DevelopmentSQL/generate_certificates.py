from cryptography.x509.oid import NameOID, ExtendedKeyUsageOID, ObjectIdentifier
from cryptography.hazmat.primitives import hashes, serialization
from cryptography.hazmat.primitives.asymmetric import rsa
from ipaddress import ip_address
from ipaddress import ip_network
from datetime import datetime
from cryptography import x509
import json
import os

EXTENDED_KEY_USAGE_OID_MAP = {
    "ServerAuthentication": ExtendedKeyUsageOID.SERVER_AUTH,
    "ClientAuthentication": ExtendedKeyUsageOID.CLIENT_AUTH,
    "CodeSigning": ExtendedKeyUsageOID.CODE_SIGNING,
    "EmailProtection": ExtendedKeyUsageOID.EMAIL_PROTECTION,
    "IpSecEndSystem": ObjectIdentifier("1.3.6.1.5.5.7.3.5"),
    "IpSecTunnel": ObjectIdentifier("1.3.6.1.5.5.7.3.6"),
    "IpSecUser": ObjectIdentifier("1.3.6.1.5.5.7.3.7"),
    "TimeStamping": ExtendedKeyUsageOID.TIME_STAMPING,
    "OcspSigning": ExtendedKeyUsageOID.OCSP_SIGNING,
    "Dvcs": ObjectIdentifier("1.3.6.1.5.5.7.3.10")
}

KEY_USAGE_MAP = {
    "DigitalSignature": "digital_signature",
    "NonRepudiation": "content_commitment",
    "KeyEncipherment": "key_encipherment",
    "DataEncipherment": "data_encipherment",
    "KeyAgreement": "key_agreement",
    "CertificateSigning": "key_cert_sign",
    "CrlSigning": "crl_sign",
    "EncipherOnly": "encipher_only",
    "DecipherOnly": "decipher_only"
}

CRL_DISTRIBUTION_URL = "https://localhost:8081/api/v1/crl"


def parse_general_names(general_names_json, for_name_constraints=False):
    general_names_list = []
    for general_name in general_names_json:
        name_type = general_name.get("type", "DNS").upper()
        name_value = general_name["value"]
        if name_type == "DNS":
            general_names_list.append(x509.DNSName(name_value))
        elif name_type == "IP":
            if for_name_constraints:
                general_names_list.append(x509.IPAddress(ip_network(name_value)))
            else:
                general_names_list.append(x509.IPAddress(ip_address(name_value)))
        elif name_type == "URI":
            general_names_list.append(x509.UniformResourceIdentifier(name_value))
        elif name_type in ("EMAIL", "RFC822"):
            general_names_list.append(x509.RFC822Name(name_value))
    return general_names_list


def create_key_usage_extension(key_usage_json):
    return x509.KeyUsage(
        digital_signature="DigitalSignature" in key_usage_json,
        content_commitment="NonRepudiation" in key_usage_json,
        key_encipherment="KeyEncipherment" in key_usage_json,
        data_encipherment="DataEncipherment" in key_usage_json,
        key_agreement="KeyAgreement" in key_usage_json,
        key_cert_sign="CertificateSigning" in key_usage_json,
        crl_sign="CrlSigning" in key_usage_json,
        encipher_only="EncipherOnly" in key_usage_json,
        decipher_only="DecipherOnly" in key_usage_json
    )


def create_certificate(certificate_json, all_certificates):
    certificate_private_key = rsa.generate_private_key(public_exponent=65537, key_size=2048)

    certificate_subject_name = x509.Name([
        x509.NameAttribute(NameOID.COUNTRY_NAME, certificate_json.get("C", "")),
        x509.NameAttribute(NameOID.STATE_OR_PROVINCE_NAME, certificate_json.get("ST", "")),
        x509.NameAttribute(NameOID.LOCALITY_NAME, certificate_json.get("L", "")),
        x509.NameAttribute(NameOID.ORGANIZATION_NAME, certificate_json.get("O", "")),
        x509.NameAttribute(NameOID.ORGANIZATIONAL_UNIT_NAME, certificate_json.get("OU", "")),
        x509.NameAttribute(NameOID.COMMON_NAME, certificate_json.get("CN", ""))
    ])

    if certificate_json.get("Type") == "Root":
        issuer_name = certificate_subject_name
        issuer_private_key = certificate_private_key
    else:
        issuer_certificate_json = next(c for c in all_certificates if c["Name"] == certificate_json["Issuer"])
        issuer_name = issuer_certificate_json["Certificate"].subject
        issuer_private_key = issuer_certificate_json["Key"]

    not_before = datetime.fromisoformat(certificate_json["NotBefore"].replace("Z", "+00:00"))
    not_after = datetime.fromisoformat(certificate_json["NotAfter"].replace("Z", "+00:00"))

    certificate_builder = x509.CertificateBuilder()
    certificate_builder = certificate_builder.subject_name(certificate_subject_name)
    certificate_builder = certificate_builder.issuer_name(issuer_name)
    certificate_builder = certificate_builder.public_key(certificate_private_key.public_key())
    certificate_builder = certificate_builder.serial_number(x509.random_serial_number())
    certificate_builder = certificate_builder.not_valid_before(not_before)
    certificate_builder = certificate_builder.not_valid_after(not_after)

    basic_constraints = certificate_json.get("BasicConstraints")
    if basic_constraints:
        certificate_builder = certificate_builder.add_extension(
            x509.BasicConstraints(ca=basic_constraints.get("IsCa", False),
                                  path_length=basic_constraints.get("PathLen")),
            critical=True
        )

    key_usage_json = certificate_json.get("KeyUsage", [])
    if key_usage_json:
        certificate_builder = certificate_builder.add_extension(
            create_key_usage_extension(key_usage_json),
            critical=True
        )

    extended_key_usage_json = certificate_json.get("ExtendedKeyUsage", [])
    if extended_key_usage_json:
        oids_list = [EXTENDED_KEY_USAGE_OID_MAP[k] for k in extended_key_usage_json if k in EXTENDED_KEY_USAGE_OID_MAP]
        if oids_list:
            certificate_builder = certificate_builder.add_extension(x509.ExtendedKeyUsage(oids_list), critical=False)

    subject_alt_names_json = certificate_json.get("SubjectAltName")
    if subject_alt_names_json:
        certificate_builder = certificate_builder.add_extension(
            x509.SubjectAlternativeName(parse_general_names(subject_alt_names_json)),
            critical=False
        )

    issuer_alt_names_json = certificate_json.get("IssuerAltName")
    if issuer_alt_names_json:
        certificate_builder = certificate_builder.add_extension(
            x509.IssuerAlternativeName(parse_general_names(issuer_alt_names_json)),
            critical=False
        )

    name_constraints_json = certificate_json.get("NameConstraints")
    if name_constraints_json:
        permitted_subtrees = parse_general_names(name_constraints_json.get("Permitted", []), for_name_constraints=True)
        excluded_subtrees = parse_general_names(name_constraints_json.get("Excluded", []), for_name_constraints=True)

        if permitted_subtrees or excluded_subtrees:
            certificate_builder = certificate_builder.add_extension(
                x509.NameConstraints(
                    permitted_subtrees=permitted_subtrees if permitted_subtrees else None,
                    excluded_subtrees=excluded_subtrees if excluded_subtrees else None
                ),
                critical=True
            )

    certificate_policies_json = certificate_json.get("CertificatePolicies", [])
    if certificate_policies_json:
        policy_list = [x509.PolicyInformation(ObjectIdentifier(p), None) for p in certificate_policies_json]
        certificate_builder = certificate_builder.add_extension(x509.CertificatePolicies(policy_list), critical=False)

    crl_dp = x509.DistributionPoint(full_name=[x509.UniformResourceIdentifier(CRL_DISTRIBUTION_URL)], relative_name=None, reasons=None, crl_issuer=None)
    certificate_builder = certificate_builder.add_extension(x509.CRLDistributionPoints([crl_dp]), critical=False)

    certificate_object = certificate_builder.sign(private_key=issuer_private_key, algorithm=hashes.SHA256())
    return {"Certificate": certificate_object, "Key": certificate_private_key, "Name": certificate_json["Name"]}


def generate_certificates(certificate_directory="certificates"):
    os.makedirs(certificate_directory, exist_ok=True)

    with open("certificate_data.json") as certificate_data_file:
        certificates_json = json.load(certificate_data_file)

    all_certificates = []
    for certificate_json in certificates_json:
        certificate_object = create_certificate(certificate_json, all_certificates)
        all_certificates.append(certificate_object)

        key_bytes = certificate_object["Key"].private_bytes(
            serialization.Encoding.PEM,
            serialization.PrivateFormat.TraditionalOpenSSL,
            serialization.NoEncryption()
        )
        certificate_bytes = certificate_object["Certificate"].public_bytes(serialization.Encoding.PEM)

        key_file_path = os.path.join(certificate_directory, f"{certificate_json['Name']}.key.pem")
        certificate_file_path = os.path.join(certificate_directory, f"{certificate_json['Name']}.cert.pem")

        with open(key_file_path, "wb") as key_file:
            key_file.write(key_bytes)
        with open(certificate_file_path, "wb") as certificate_file:
            certificate_file.write(certificate_bytes)

        print(f"Generated {certificate_file_path} and {key_file_path}")


if __name__ == "__main__":
    generate_certificates()
