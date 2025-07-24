using JobBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Persistence.Configuration;

public class CvConfig : IEntityTypeConfiguration<Cv>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cv> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.City).HasMaxLength(50).IsRequired();
        builder.Property(c => c.FullAddress).HasMaxLength(300);
        builder.Property(c => c.FullName).HasMaxLength(200);



    }
}
