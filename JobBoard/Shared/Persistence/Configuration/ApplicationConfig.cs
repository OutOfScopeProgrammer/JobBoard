using JobBoard.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Shared.Persistence.Configuration;

public class ApplicationConfig : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasOne<User>().WithOne().HasForeignKey<Application>(a => a.ApplicantId);
    }
}
