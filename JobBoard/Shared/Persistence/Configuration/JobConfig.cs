using JobBoard.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Shared.Persistence.Configuration;

public class JobConfig : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);
        builder.HasOne<User>()
        .WithOne().HasForeignKey<Job>(j => j.EmployeeId);

        builder.HasMany(j => j.Applications).WithOne().HasForeignKey(a => a.JobId);

    }
}
