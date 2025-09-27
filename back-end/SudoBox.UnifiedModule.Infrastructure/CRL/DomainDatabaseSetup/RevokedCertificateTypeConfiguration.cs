using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SudoBox.UnifiedModule.Domain.Certificates;
using SudoBox.UnifiedModule.Domain.CRL;

namespace SudoBox.UnifiedModule.Infrastructure.Crl.DomainDatabaseSetup;

public sealed class RevokedCertificateTypeConfiguration : IEntityTypeConfiguration<RevokedCertificate>
{
    public void Configure(EntityTypeBuilder<RevokedCertificate> b)
    {
        b.ToTable("revoked_certificates");
        b.HasKey(x => x.Id);

        b.Property(x => x.RevocationReason);

        b.HasOne(x => x.Certificate)
            .WithOne()
            .HasForeignKey<Certificate>("CertificateId");
    }
}
