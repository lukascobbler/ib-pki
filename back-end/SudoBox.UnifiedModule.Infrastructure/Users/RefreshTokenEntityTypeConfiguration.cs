using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SudoBox.UnifiedModule.Domain.Users;

public sealed class RefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("refresh_tokens");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasColumnType("uuid")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.UserId).HasColumnType("uuid").IsRequired();
        b.Property(x => x.TokenHashHex).HasMaxLength(128).IsRequired();
        b.HasIndex(x => x.TokenHashHex).IsUnique();

        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.ExpiresAt).IsRequired();

        b.Property(x => x.ConsumedAt);
        b.Property(x => x.RevokedAt);
        b.Property(x => x.ReplacedByHashHex).HasMaxLength(128);

        b.Property(x => x.DeviceId).HasMaxLength(128);
        b.Property(x => x.Ip).HasMaxLength(64);
        b.Property(x => x.UserAgent).HasMaxLength(256);

        b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
