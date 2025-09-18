using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SudoBox.UnifiedModule.Domain.Users;

namespace SudoBox.UnifiedModule.Infrastructure.Users.DomainDatabaseSetup;

internal sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Email)
            .IsRequired()
            .HasColumnType("citext"); // enables case-insensitive comparisons
        b.HasIndex(x => x.Email).IsUnique();

        b.Property(x => x.EmailConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        b.Property(x => x.HashedPassword)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.RefreshToken)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.Name).HasMaxLength(100);
        b.Property(x => x.Surname).HasMaxLength(100);
        b.Property(x => x.Organization).HasMaxLength(150);
        b.Property(x => x.RefreshTokenExpiresAt);

        // Store Role as string for readability
        b.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
    }
}
