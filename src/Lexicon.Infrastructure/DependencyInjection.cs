using Lexicon.Application.Identity;
using Lexicon.Application.Interfaces;
using Lexicon.Domain.Interfaces;
using Lexicon.Infrastructure.Data;
using Lexicon.Infrastructure.Identity;
using Lexicon.Infrastructure.Repositories;
using Lexicon.Infrastructure.Security;
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

        // Identity Services
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // Security Services
        services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
