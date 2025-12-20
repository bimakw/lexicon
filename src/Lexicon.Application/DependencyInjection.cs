using FluentValidation;
using Lexicon.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lexicon.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ICommentService, CommentService>();

        // Register validators
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
