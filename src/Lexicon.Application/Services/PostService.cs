using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;

namespace Lexicon.Application.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;

    public PostService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PostDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id, cancellationToken);
        if (post == null) return null;

        var tags = await _unitOfWork.Tags.GetByPostIdAsync(id, cancellationToken);
        return MapToDto(post, tags);
    }

    public async Task<PostDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetBySlugAsync(slug, cancellationToken);
        if (post == null) return null;

        var tags = await _unitOfWork.Tags.GetByPostIdAsync(post.Id, cancellationToken);
        return MapToDto(post, tags);
    }

    public async Task<PagedResult<PostListDto>> GetPagedAsync(
        int page = 1,
        int pageSize = 10,
        PostStatus? status = null,
        Guid? categoryId = null,
        Guid? authorId = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var (posts, totalCount) = await _unitOfWork.Posts.GetPagedAsync(
            page, pageSize, status, categoryId, authorId, searchTerm, cancellationToken);

        var items = posts.Select(p => new PostListDto(
            p.Id,
            p.Title,
            p.Slug,
            p.Excerpt,
            p.FeaturedImage,
            p.Status.ToString(),
            p.PublishedAt,
            p.Author?.Name,
            p.Category?.Name,
            p.CreatedAt
        ));

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PagedResult<PostListDto>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<PostDto> CreateAsync(CreatePostDto dto, CancellationToken cancellationToken = default)
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Slug = GenerateSlug(dto.Title),
            Content = dto.Content,
            Excerpt = dto.Excerpt,
            FeaturedImage = dto.FeaturedImage,
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId,
            Status = PostStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Posts.AddAsync(post, cancellationToken);

        if (dto.TagIds?.Any() == true)
        {
            foreach (var tagId in dto.TagIds)
            {
                post.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var tags = dto.TagIds?.Any() == true
            ? await _unitOfWork.Tags.FindAsync(t => dto.TagIds.Contains(t.Id), cancellationToken)
            : Enumerable.Empty<Tag>();

        return MapToDto(post, tags);
    }

    public async Task<PostDto?> UpdateAsync(Guid id, UpdatePostDto dto, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id, cancellationToken);
        if (post == null) return null;

        post.Title = dto.Title;
        post.Slug = GenerateSlug(dto.Title);
        post.Content = dto.Content;
        post.Excerpt = dto.Excerpt;
        post.FeaturedImage = dto.FeaturedImage;
        post.CategoryId = dto.CategoryId;
        post.UpdatedAt = DateTime.UtcNow;

        // Update tags
        post.PostTags.Clear();
        if (dto.TagIds?.Any() == true)
        {
            foreach (var tagId in dto.TagIds)
            {
                post.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });
            }
        }

        await _unitOfWork.Posts.UpdateAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var tags = dto.TagIds?.Any() == true
            ? await _unitOfWork.Tags.FindAsync(t => dto.TagIds.Contains(t.Id), cancellationToken)
            : Enumerable.Empty<Tag>();

        return MapToDto(post, tags);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id, cancellationToken);
        if (post == null) return false;

        await _unitOfWork.Posts.DeleteAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PostDto?> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id, cancellationToken);
        if (post == null) return null;

        post.Status = PostStatus.Published;
        post.PublishedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Posts.UpdateAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var tags = await _unitOfWork.Tags.GetByPostIdAsync(id, cancellationToken);
        return MapToDto(post, tags);
    }

    public async Task<PostDto?> UnpublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id, cancellationToken);
        if (post == null) return null;

        post.Status = PostStatus.Draft;
        post.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Posts.UpdateAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var tags = await _unitOfWork.Tags.GetByPostIdAsync(id, cancellationToken);
        return MapToDto(post, tags);
    }

    private static PostDto MapToDto(Post post, IEnumerable<Tag> tags)
    {
        return new PostDto(
            post.Id,
            post.Title,
            post.Slug,
            post.Content,
            post.Excerpt,
            post.FeaturedImage,
            post.Status.ToString(),
            post.PublishedAt,
            post.AuthorId,
            post.Author?.Name,
            post.CategoryId,
            post.Category?.Name,
            tags.Select(t => new TagDto(t.Id, t.Name, t.Slug, t.PostTags.Count, t.CreatedAt)),
            post.CreatedAt,
            post.UpdatedAt
        );
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("--", "-")
            .Trim('-');
    }
}
