using SudoBox.UnifiedModule.Infrastructure.Certificates.DomainDatabaseSetup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SudoBox.UnifiedModule.Domain.CRL;
using Microsoft.EntityFrameworkCore;

namespace SudoBox.UnifiedModule.Infrastructure.Crl.DomainDatabaseSetup;

public sealed class RevokedCertificateTypeConfiguration : IEntityTypeConfiguration<RevokedCertificate> {
    public void Configure(EntityTypeBuilder<RevokedCertificate> b) {
        b.ToTable("revoked_certificates");
        b.HasKey(x => x.CertificateSerialNumber);
        b.Property(x => x.CertificateSerialNumber).HasConversion(TypeConverters.BigIntConverter).HasColumnType("text");

        b.Property(x => x.RevocationReason);

        b.HasOne(x => x.Certificate)
               .WithOne()
               .HasForeignKey<RevokedCertificate>(x => x.CertificateSerialNumber)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
