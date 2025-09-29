using SudoBox.UnifiedModule.Domain.Certificates.KeyManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.DomainDatabaseSetup;

public class MasterKeyTypeConfiguration : IEntityTypeConfiguration<MasterKey> {
    public void Configure(EntityTypeBuilder<MasterKey> builder) {
        builder.ToTable("master_keys");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EncryptedKey).HasColumnType("text");
    }
}