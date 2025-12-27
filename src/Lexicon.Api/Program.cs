using System.Text;
using AspNetCoreRateLimit;
using Lexicon.Api.Authorization;
using Lexicon.Application;
using Lexicon.Application.Identity;
using Lexicon.Infrastructure;
using Lexicon.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Lexicon API",
        Version = "v1",
        Description = "Blog/CMS REST API with Clean Architecture"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPermissionPolicies();
});

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// CORS - Proper configuration
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LexiconDbContext>();
    db.Database.Migrate();
    await SeedDataAsync(db);
}

// Security Headers Middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self';");
    }

    await next();
});

// Rate Limiting
app.UseIpRateLimiting();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lexicon API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("Default");
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Seed default roles
static async Task SeedDataAsync(LexiconDbContext db)
{
    if (!db.Roles.Any())
    {
        var roles = new[]
        {
            new Lexicon.Domain.Entities.Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Description = "Full system access",
                Permissions = string.Join(",", new[]
                {
                    "posts:read", "posts:create", "posts:update", "posts:delete", "posts:publish",
                    "categories:read", "categories:manage",
                    "tags:read", "tags:manage",
                    "comments:read", "comments:create", "comments:moderate",
                    "media:read", "media:upload", "media:delete",
                    "users:read", "users:manage",
                    "settings:manage"
                }),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Lexicon.Domain.Entities.Role
            {
                Id = Guid.NewGuid(),
                Name = "Editor",
                Description = "Can manage all content",
                Permissions = string.Join(",", new[]
                {
                    "posts:read", "posts:create", "posts:update", "posts:delete", "posts:publish",
                    "categories:read", "categories:manage",
                    "tags:read", "tags:manage",
                    "comments:read", "comments:moderate",
                    "media:read", "media:upload", "media:delete"
                }),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Lexicon.Domain.Entities.Role
            {
                Id = Guid.NewGuid(),
                Name = "Author",
                Description = "Can create and manage own content",
                Permissions = string.Join(",", new[]
                {
                    "posts:read", "posts:create", "posts:update",
                    "categories:read",
                    "tags:read",
                    "comments:read", "comments:create",
                    "media:read", "media:upload"
                }),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Lexicon.Domain.Entities.Role
            {
                Id = Guid.NewGuid(),
                Name = "Reader",
                Description = "Can read content and comment",
                Permissions = string.Join(",", new[]
                {
                    "posts:read",
                    "categories:read",
                    "tags:read",
                    "comments:read", "comments:create"
                }),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        db.Roles.AddRange(roles);
        await db.SaveChangesAsync();

        Log.Information("Seeded {Count} default roles", roles.Length);
    }
}
