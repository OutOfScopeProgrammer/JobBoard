using JobBoard.Domain.Entities;
using JobBoard.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.UnitTest.IdentityFeatures.ClassFixtures;

public class InMemoryDb : IDisposable
{

    public AppDbContext context;

    public InMemoryDb()
    {


        var roles = new List<Role> {
            new() { Id = Guid.NewGuid(), RoleName = "ADMIN" },
            new() { Id = Guid.NewGuid(), RoleName = "APPLICANT" },
            new() { Id = Guid.NewGuid(), RoleName = "EMPLOYEE" }
            };
        var options = new DbContextOptionsBuilder<AppDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString())
           .Options;
        context = new AppDbContext(options);
        context.Roles.AddRange(roles);
        context.SaveChanges();
    }

    public void Dispose()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}
