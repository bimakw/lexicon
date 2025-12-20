using Lexicon.Domain.Interfaces;
using Lexicon.Infrastructure.Data;
using Lexicon.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lexicon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<LexiconDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(LexiconDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
