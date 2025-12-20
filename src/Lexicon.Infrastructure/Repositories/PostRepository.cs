using Lexicon.Domain.Common;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Lexicon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lexicon.Infrastructure.Repositories;

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(LexiconDbContext context) : base(context)
    {
    }

    public override async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Post?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByStatusAsync(PostStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByTagIdAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
            .Where(p => p.PostTags.Any(pt => pt.TagId == tagId))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Post> Posts, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        PostStatus? status = null,
        Guid? categoryId = null,
        Guid? authorId = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (authorId.HasValue)
            query = query.Where(p => p.AuthorId == authorId.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(p => p.Title.Contains(searchTerm) || (p.Content != null && p.Content.Contains(searchTerm)));

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }
}
