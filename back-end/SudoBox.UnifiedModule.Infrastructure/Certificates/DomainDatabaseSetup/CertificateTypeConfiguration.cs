using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SudoBox.UnifiedModule.Domain.Certificates;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.DomainDatabaseSetup;

public sealed class CertificateTypeConfiguration : IEntityTypeConfiguration<Certificate> {
    public void Configure(EntityTypeBuilder<Certificate> b) {
        b.ToTable("certificates");
        b.HasKey(x => x.SerialNumber);

        b.Property(x => x.SerialNumber).HasConversion(TypeConverters.BigIntConverter).HasColumnType("text");
        b.Property(x => x.EncodedValue).HasMaxLength(65536).IsRequired(false);
        b.Property(x => x.IsApproved);
        b.Property(x => x.IssuedBy);
        b.Property(x => x.IssuedTo);
        b.Property(x => x.NotAfter);
        b.Property(x => x.NotBefore);
        b.Property(x => x.CanSign);
        b.Property(x => x.PathLen);

        b.Property(e => e.PrivateKey)
            .HasConversion(TypeConverters.KeyConverter)
            .HasColumnType("text")
            .IsRequired(false);

        b.HasOne(x => x.SigningCertificate)
            .WithMany()
            .HasForeignKey("SigningCertificateSerialNumber")
            .IsRequired(false);

        b.HasOne(x => x.SignedBy)
            .WithMany();
    }
}
