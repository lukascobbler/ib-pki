using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SudoBox.UnifiedModule.Domain.Users;

namespace SudoBox.UnifiedModule.Infrastructure;

internal sealed class VerificationTokenEntityTypeConfiguration : IEntityTypeConfiguration<VerificationToken>
{
    public void Configure(EntityTypeBuilder<VerificationToken> b)
    {
        b.ToTable("verification_tokens");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(64).IsRequired();
        b.Property(x => x.TokenHashHex).HasMaxLength(128).IsRequired(); // 64 hex chars max, but keep margin
        b.Property(x => x.ExpiresAt).IsRequired();

        b.HasIndex(x => x.TokenHashHex).IsUnique();

        b.HasOne(x => x.User)
         .WithMany()                // tokens are not navigated from user for simplicity
         .HasForeignKey(x => x.UserId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
