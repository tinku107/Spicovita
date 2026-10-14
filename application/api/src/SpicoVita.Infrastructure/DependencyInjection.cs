using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpicoVita.Application.Common;
using SpicoVita.Infrastructure.Persistence;
namespace SpicoVita.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration cfg)
    {
        s.AddDbContext<AppDbContext>(o => o.UseSqlServer(cfg.GetConnectionString("Default")));
        s.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        return s;
    }
}
