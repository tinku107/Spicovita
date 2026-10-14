using Microsoft.EntityFrameworkCore;
using SpicoVita.Application.Common;
using SpicoVita.Domain.Catalog;
using SpicoVita.Domain.Common;
namespace SpicoVita.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("catalog");
        b.Entity<Product>(e =>
        {
            e.HasIndex(x => new { x.CompanyId, x.Sku }).IsUnique();
            e.Property(x => x.Mrp).HasPrecision(18, 2);
            e.Property(x => x.GstRatePercent).HasPrecision(5, 2);
            e.Property(x => x.RowVersion).IsRowVersion();
            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        foreach (var en in ChangeTracker.Entries<AuditableEntity>())
        {
            if (en.State == EntityState.Added) en.Entity.CreatedAtUtc = now;
            else if (en.State == EntityState.Modified) en.Entity.UpdatedAtUtc = now;
        }
        return base.SaveChangesAsync(ct);
    }
}
