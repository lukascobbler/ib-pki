using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SudoBox.UnifiedModule.Domain.CertificateRequests;

namespace SudoBox.UnifiedModule.Infrastructure.CertificateRequests.DomainDatabaseSetup;

public sealed class CertificateRequestTypeConfiguration : IEntityTypeConfiguration<CertificateRequest> {
    public void Configure(EntityTypeBuilder<CertificateRequest> b) {
        b.ToTable("certificate_requests");

        b.Property(x => x.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.EncodedCSR).HasMaxLength(65536);
        b.Property(x => x.NotBefore).IsRequired(false);
        b.Property(x => x.NotAfter).IsRequired(false);
        b.HasOne(x => x.RequestedFrom).WithMany();
        b.HasOne(x => x.RequestedFor).WithMany();
        b.Property(x => x.SubmittedOn);
    }
}
