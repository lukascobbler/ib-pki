namespace SudoBox.UnifiedModule.Application.Certificates.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Contracts;
using SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

public static class CertificateBuilder
{
    public static (X509Certificate certificate, AsymmetricCipherKeyPair subjectKeyPair, byte[] certEncoded) 
        CreateCertificate(
            CreateCertificateDto dto,
            AsymmetricKeyParameter issuerPrivateKey,
            X509Certificate issuerCertificate,
            AsymmetricCipherKeyPair subjectKeyPair
        )
    {
        var serial = CreateSerialNumber();

        var subjectName = BuildX509Name(dto);
        var issuerName = issuerCertificate != null ? issuerCertificate.SubjectDN : new X509Name($"CN={issuerCertificate.SubjectDN}") ;

        var certGen = new X509V3CertificateGenerator();
        certGen.SetSerialNumber(serial);
        certGen.SetIssuerDN(issuerCertificate.SubjectDN);
        
        if (dto.NotBefore.HasValue)
            certGen.SetNotBefore(dto.NotBefore.Value);    
        
        if (dto.NotAfter.HasValue)
            certGen.SetNotAfter(dto.NotAfter.Value);
        
        
        certGen.SetSubjectDN(subjectName);
        certGen.SetPublicKey(subjectKeyPair.Public);

        if (dto.BasicConstraints != null)
        {
            var isCa = dto.BasicConstraints.IsCa;
            var pathLen = dto.BasicConstraints.PathLenConstraint.HasValue ? dto.BasicConstraints.PathLenConstraint.Value : -1;
            certGen.AddExtension(X509Extensions.BasicConstraints, true, pathLen >= 0 ? new BasicConstraints(pathLen) : new BasicConstraints(isCa));
        }

        if (dto.KeyUsage != null && dto.KeyUsage.Any())
        {
            var usageBits = 0;
            foreach (var ku in dto.KeyUsage)
            {
                switch (ku)
                {
                    case KeyUsageValue.DigitalSignature:
                        usageBits |= KeyUsage.DigitalSignature;
                        break;
                    case KeyUsageValue.NonRepudiation:
                        usageBits |= KeyUsage.NonRepudiation;
                        break;
                    case KeyUsageValue.KeyEncipherment:
                        usageBits |= KeyUsage.KeyEncipherment;
                        break;
                    case KeyUsageValue.DataEncipherment:
                        usageBits |= KeyUsage.DataEncipherment;
                        break;
                    case KeyUsageValue.KeyAgreement:
                        usageBits |= KeyUsage.KeyAgreement;
                        break;
                    case KeyUsageValue.CertificateSigning:
                        usageBits |= KeyUsage.KeyCertSign;
                        break;
                    case KeyUsageValue.CrlSigning:
                        usageBits |= KeyUsage.CrlSign;
                        break;
                    case KeyUsageValue.EncipherOnly:
                        usageBits |= KeyUsage.EncipherOnly;
                        break;
                    case KeyUsageValue.DecipherOnly:
                        usageBits |= KeyUsage.DecipherOnly;
                        break;
                }
            }
            certGen.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(usageBits));
        }

        if (dto.ExtendedKeyUsage != null && dto.ExtendedKeyUsage.Any())
        {
            var ekuOids = new List<DerObjectIdentifier>();
            foreach (var eku in dto.ExtendedKeyUsage)
            {
                switch (eku)
                {
                    case ExtendedKeyUsageValue.ServerAuthentication:
                        ekuOids.Add(KeyPurposeID.id_kp_serverAuth);
                        break;
                    case ExtendedKeyUsageValue.ClientAuthentication:
                        ekuOids.Add(KeyPurposeID.id_kp_clientAuth);
                        break;
                    case ExtendedKeyUsageValue.CodeSigning:
                        ekuOids.Add(KeyPurposeID.id_kp_codeSigning);
                        break;
                    case ExtendedKeyUsageValue.EmailProtection:
                        ekuOids.Add(KeyPurposeID.id_kp_emailProtection);
                        break;
                    case ExtendedKeyUsageValue.IpSecEndSystem:
                        ekuOids.Add(KeyPurposeID.id_kp_ipsecEndSystem);
                        break;
                    case ExtendedKeyUsageValue.IpSecTunnel:
                        ekuOids.Add(KeyPurposeID.id_kp_ipsecTunnel);
                        break;
                    case ExtendedKeyUsageValue.IpSecUser:
                        ekuOids.Add(KeyPurposeID.id_kp_ipsecUser);
                        break;
                    case ExtendedKeyUsageValue.TimeStamping:
                        ekuOids.Add(KeyPurposeID.id_kp_timeStamping);
                        break;
                    case ExtendedKeyUsageValue.OcspSigning:
                        ekuOids.Add(KeyPurposeID.id_kp_OCSPSigning);
                        break;
                    case ExtendedKeyUsageValue.Dvcs:
                        ekuOids.Add(KeyPurposeID.id_kp_dvcs);
                        break;
                }
            }
            certGen.AddExtension(X509Extensions.ExtendedKeyUsage, false, new ExtendedKeyUsage(ekuOids.ToArray()));
        }

        if (dto.SubjectAlternativeNames != null)
        {
            var san = BuildGeneralNames(dto.SubjectAlternativeNames);
            certGen.AddExtension(X509Extensions.SubjectAlternativeName, false, san);
        }

        if (dto.IssuerAlternativeNames != null)
        {
            var ian = BuildGeneralNames(dto.IssuerAlternativeNames);
            certGen.AddExtension(X509Extensions.IssuerAlternativeName, false, ian);
        }

        if (dto.NameConstraints != null)
        {
            var permitted = BuildGeneralSubtrees(dto.NameConstraints);
            var excluded = Array.Empty<GeneralSubtree>();
            var nameConstraints = new NameConstraints(permitted.Length > 0 ? permitted : null, excluded.Length > 0 ? excluded : null);
            certGen.AddExtension(X509Extensions.NameConstraints, true, nameConstraints);
        }

        if (dto.CertificatePolicies != null && dto.CertificatePolicies.Policies != null && dto.CertificatePolicies.Policies.Any())
        {
            var policyInfos = dto.CertificatePolicies.Policies.Select(p => new PolicyInformation(new DerObjectIdentifier(p.Oid))).ToArray();
            certGen.AddExtension(X509Extensions.CertificatePolicies, false, new DerSequence(policyInfos));
        }

        var sigAlg = "SHA256WithRSA";
        var signer = new Asn1SignatureFactory(sigAlg, issuerPrivateKey, new SecureRandom());
        var certificate = certGen.Generate(signer);

        var encoded = certificate.GetEncoded();

        return (certificate, subjectKeyPair, encoded);
    }

    private static BigInteger CreateSerialNumber()
    {
        var random = new SecureRandom();
        var bytes = new byte[16];
        random.NextBytes(bytes);
        return new BigInteger(1, bytes);
    }

    private static X509Name BuildX509Name(CreateCertificateDto dto)
    {
        var attrs = new List<DerObjectIdentifier>();
        var values = new List<string>();
        attrs.Add(X509Name.CN);
        values.Add(dto.CommonName);
        if (!string.IsNullOrEmpty(dto.Organization))
        {
            attrs.Add(X509Name.O);
            values.Add(dto.Organization);
        }
        if (!string.IsNullOrEmpty(dto.OrganizationalUnit))
        {
            attrs.Add(X509Name.OU);
            values.Add(dto.OrganizationalUnit);
        }
        if (!string.IsNullOrEmpty(dto.Email))
        {
            attrs.Add(X509Name.EmailAddress);
            values.Add(dto.Email);
        }
        if (!string.IsNullOrEmpty(dto.Country))
        {
            attrs.Add(X509Name.C);
            values.Add(dto.Country);
        }
        return new X509Name(attrs, values);
    }

    private static GeneralNames BuildGeneralNames(AlternativeNames alt)
    {
        var names = new List<GeneralName>();
        if (alt.DnsNames != null)
        {
            foreach (var d in alt.DnsNames) names.Add(new GeneralName(GeneralName.DnsName, d));
        }
        if (alt.Rfc822Names != null)
        {
            foreach (var r in alt.Rfc822Names) names.Add(new GeneralName(GeneralName.Rfc822Name, r));
        }
        if (alt.UriNames != null)
        {
            foreach (var u in alt.UriNames) names.Add(new GeneralName(GeneralName.UniformResourceIdentifier, u));
        }
        if (alt.IpAddresses != null)
        {
            foreach (var ip in alt.IpAddresses) names.Add(new GeneralName(GeneralName.IPAddress, ip));
        }
        if (alt.DirectoryNames != null)
        {
            foreach (var dn in alt.DirectoryNames) names.Add(new GeneralName(GeneralName.DirectoryName, new X509Name(dn)));
        }
        return new GeneralNames(names.ToArray());
    }

    private static GeneralSubtree[] BuildGeneralSubtrees(AlternativeNames constraints)
    {
        var names = new List<GeneralSubtree>();
        if (constraints.DnsNames != null)
        {
            foreach (var d in constraints.DnsNames)
            {
                var gn = new GeneralName(GeneralName.DnsName, d);
                names.Add(new GeneralSubtree(gn));
            }
        }
        if (constraints.IpAddresses != null)
        {
            foreach (var ip in constraints.IpAddresses)
            {
                var gn = new GeneralName(GeneralName.IPAddress, ip);
                names.Add(new GeneralSubtree(gn));
            }
        }
        if (constraints.UriNames != null)
        {
            foreach (var u in constraints.UriNames)
            {
                var gn = new GeneralName(GeneralName.UniformResourceIdentifier, u);
                names.Add(new GeneralSubtree(gn));
            }
        }
        return names.ToArray();
    }
}