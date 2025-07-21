using JobBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Shared.Persistence.Seeder;

public static class Seeder
{
    public static void Initialize(AppDbContext dbContext, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Seeder");
        logger.LogInformation("Applying migrations ...");
        dbContext.Database.Migrate();

        if (!dbContext.Roles.Any())
        {
            logger.LogInformation("Seeding ....");

            var admin = new Role { Id = Guid.NewGuid(), RoleName = "ADMIN" };
            var applicant = new Role { Id = Guid.NewGuid(), RoleName = "APPLICANT" };
            var employee = new Role { Id = Guid.NewGuid(), RoleName = "EMPLOYEE" };
            dbContext.AddRange(admin, applicant, employee);
            dbContext.SaveChanges();
            logger.LogInformation("Seeding Finished");

        }
    }

}
