using Microsoft.EntityFrameworkCore;
using SpicoVita.Domain.Catalog;
namespace SpicoVita.Application.Common;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
