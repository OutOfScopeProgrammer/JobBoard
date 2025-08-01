
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace JobBoard.Infrastructure.Persistence.Postgres.Intercepters;

public class ConcurrencyInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            UpdateConcurrencyToken(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            UpdateConcurrencyToken(eventData.Context);

        return base.SavingChanges(eventData, result);
    }



    private void UpdateConcurrencyToken(DbContext? context)
    {
        if (context is null) return;
        var propertyName = "ConcurrencyToken";
        var modifiedEntries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);
        foreach (var entry in modifiedEntries)
        {
            if (entry.Metadata.FindProperty(propertyName)?.IsConcurrencyToken == true)
            {
                entry.Property(propertyName).CurrentValue = Guid.NewGuid().ToByteArray();
            }
        }

    }

}
