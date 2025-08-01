using System.Diagnostics;
using JobBoard.Domain.Entities;
using JobBoard.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Shared.Persistence.Postgres.Seeder;

public static class Seeder
{
    public static void Initialize(AppDbContext dbContext, ILoggerFactory loggerFactory)
    {
        Debug.WriteLine("applying migrations ...");
        dbContext.Database.Migrate();

        if (!dbContext.Roles.Any())
        {
            Debug.WriteLine("seeding ....");

            var admin = new Role { Id = Guid.NewGuid(), RoleName = ApplicationRoles.ADMIN.ToString() };
            var applicant = new Role { Id = Guid.NewGuid(), RoleName = ApplicationRoles.APPLICANT.ToString() };
            var employee = new Role { Id = Guid.NewGuid(), RoleName = ApplicationRoles.EMPLOYEE.ToString() };
            dbContext.AddRange(admin, applicant, employee);
            dbContext.SaveChanges();
            Debug.WriteLine("seeding Finished");

        }
        Debug.WriteLine("migrations End");

    }

}
