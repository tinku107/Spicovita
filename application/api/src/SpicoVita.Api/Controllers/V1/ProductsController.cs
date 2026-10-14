using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpicoVita.Application.Common;
using SpicoVita.Domain.Catalog;
namespace SpicoVita.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController(IAppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<PagedResult<Product>> Get(int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var q = db.Products.AsNoTracking().OrderBy(p => p.Name);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Product>(items, page, pageSize, total);
    }
}
