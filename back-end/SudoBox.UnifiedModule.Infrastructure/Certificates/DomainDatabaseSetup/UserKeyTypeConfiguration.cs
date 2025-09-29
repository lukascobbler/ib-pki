using SudoBox.UnifiedModule.Domain.Certificates.KeyManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SudoBox.UnifiedModule.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.DomainDatabaseSetup;

public class UserKeyTypeConfiguration : IEntityTypeConfiguration<UserKey> {
    public void Configure(EntityTypeBuilder<UserKey> builder) {
        builder.ToTable("user_keys");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.EncryptedKey).IsRequired().HasColumnType("text");
        builder.HasOne<User>().WithOne().HasForeignKey<UserKey>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}