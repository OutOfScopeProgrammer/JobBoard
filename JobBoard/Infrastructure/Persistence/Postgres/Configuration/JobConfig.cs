
using JobBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Postgres.Configuration;

public class JobConfig : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);
        builder.HasOne<User>()
        .WithOne().HasForeignKey<Job>(j => j.EmployeeId);

        builder.HasMany(j => j.Applications).WithOne().HasForeignKey(a => a.JobId);

        builder.Property(j => j.Description).HasMaxLength(200).IsRequired();
        builder.Property(j => j.Title).HasMaxLength(100).IsRequired();
        builder.Property(j => j.Salary).IsRequired();

        builder.Property(j => j.ConcurrencyToken).IsConcurrencyToken();

    }
}
