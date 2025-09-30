using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SudoBox.UnifiedModule.Domain.Certificates;
using Microsoft.EntityFrameworkCore;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.DomainDatabaseSetup;

public sealed class CertificateTypeConfiguration : IEntityTypeConfiguration<Certificate> {
    public void Configure(EntityTypeBuilder<Certificate> b) {
        b.ToTable("certificates");
        b.HasKey(x => x.SerialNumber);

        b.Property(x => x.SerialNumber).HasConversion(TypeConverters.BigIntConverter).HasColumnType("text");
        b.Property(x => x.EncodedValue).IsRequired().HasMaxLength(65536);
        b.Property(x => x.IssuedBy).IsRequired();
        b.Property(x => x.IssuedTo).IsRequired();
        b.Property(x => x.NotAfter).IsRequired();
        b.Property(x => x.NotBefore).IsRequired();
        b.Property(x => x.CanSign).IsRequired();
        b.Property(x => x.PathLen).IsRequired();

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
