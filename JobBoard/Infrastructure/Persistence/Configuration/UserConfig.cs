
using JobBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configuration;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.Role)
        .WithMany(r => r.Users)
        .HasForeignKey(u => u.RoleId);

        builder.HasMany(u => u.Jobs)
        .WithOne()
        .HasForeignKey(j => j.EmployeeId);

        builder.HasMany(u => u.Applications)
        .WithOne()
        .HasForeignKey(a => a.ApplicantId);

        builder.HasOne(u => u.Cv)
        .WithOne()
        .HasForeignKey<Cv>(c => c.UserId);

        builder.Property(u => u.Name).HasMaxLength(70);
    }
}
