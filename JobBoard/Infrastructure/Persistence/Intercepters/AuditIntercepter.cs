
using JobBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace JobBoard.Infrastructure.Persistence.Intercepters;

public class AuditIntercepter : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
     InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) throw new Exception("Context is null");
        UpdateTimestamps(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null) throw new Exception("Context is null");
        UpdateTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }




    private void UpdateTimestamps(DbContext dbContext)
    {
        var entries = dbContext.ChangeTracker.Entries<Auditable>();
        foreach (var entry in entries)
        {
            Console.WriteLine($"the entry state: {entry.State}");
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }

}
