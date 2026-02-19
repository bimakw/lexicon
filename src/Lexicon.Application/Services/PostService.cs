using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lexicon.Application.Services;

<<<<<<< Updated upstream
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
=======
public class PostService(
    IUnitOfWork unitOfWork,
    ILogger<PostService> logger) : IPostService
{
    public async Task<Result<PostDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var post = await unitOfWork.Posts.GetByIdAsync(id, ct);
        if (post == null) 
            return Result<PostDto>.Failure("Post tidak ditemukan.");

        var tags = await unitOfWork.Tags.GetByPostIdAsync(id, ct);
        return Result<PostDto>.Success(MapToDto(post, tags));
    }

    public async Task<Result<PostDto>> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var post = await unitOfWork.Posts.GetBySlugAsync(slug, ct);
        if (post == null)
            return Result<PostDto>.Failure("Post tidak ditemukan.");

        var tags = await unitOfWork.Tags.GetByPostIdAsync(post.Id, ct);
        return Result<PostDto>.Success(MapToDto(post, tags));
>>>>>>> Stashed changes
    }

    public async Task<PagedResult<PostListDto>> GetPagedAsync(
        int page = 1,
        int pageSize = 10,
        PostStatus? status = null,
        Guid? categoryId = null,
        Guid? authorId = null,
        string? searchTerm = null,
        CancellationToken ct = default)
    {
        var (posts, totalCount) = await unitOfWork.Posts.GetPagedAsync(
            page, pageSize, status, categoryId, authorId, searchTerm, ct);

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

<<<<<<< Updated upstream
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
=======
    public async Task<Result<PostDto>> CreateAsync(CreatePostDto dto, CancellationToken ct = default)
    {
        try 
>>>>>>> Stashed changes
        {
            var slug = SlugHelper.GenerateSlug(dto.Title);

            // pastikan slug post tidak duplikat
            var existing = await unitOfWork.Posts.GetBySlugAsync(slug, ct);
            if (existing != null)
                return Result<PostDto>.Failure("Judul atau slug post sudah digunakan.");

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Slug = slug,
                Content = dto.Content,
                Excerpt = dto.Excerpt,
                FeaturedImage = dto.FeaturedImage,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                Status = PostStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Posts.AddAsync(post, ct);

            if (dto.TagIds?.Any() == true)
            {
                foreach (var tagId in dto.TagIds)
                    post.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });
            }

            await unitOfWork.SaveChangesAsync(ct);

            var tags = dto.TagIds?.Any() == true
                ? await unitOfWork.Tags.FindAsync(t => dto.TagIds.Contains(t.Id), ct)
                : Enumerable.Empty<Tag>();

<<<<<<< Updated upstream
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
=======
            return Result<PostDto>.Success(MapToDto(post, tags));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error pas bikin post baru: {Title}", dto.Title);
            return Result<PostDto>.Failure("Gagal memproses pembuatan post baru.");
        }
    }

    public async Task<Result<PostDto>> UpdateAsync(Guid id, UpdatePostDto dto, CancellationToken ct = default)
    {
        try 
        {
            var post = await unitOfWork.Posts.GetByIdAsync(id, ct);
            if (post == null)
                return Result<PostDto>.Failure("Post yang akan diperbarui tidak ditemukan.");

            if (post.Title != dto.Title)
            {
                var newSlug = SlugHelper.GenerateSlug(dto.Title);
                
                // cek slug baru
                var existing = await unitOfWork.Posts.GetBySlugAsync(newSlug, ct);
                if (existing != null && existing.Id != id)
                    return Result<PostDto>.Failure("Judul atau slug tersebut sudah digunakan oleh post lain.");

                post.Title = dto.Title;
                post.Slug = newSlug;
            }
>>>>>>> Stashed changes

            post.Content = dto.Content;
            post.Excerpt = dto.Excerpt;
            post.FeaturedImage = dto.FeaturedImage;
            post.CategoryId = dto.CategoryId;
            post.UpdatedAt = DateTime.UtcNow;

            // update tags (reset dulu baru isi lagi)
            post.PostTags.Clear();
            if (dto.TagIds?.Any() == true)
            {
                foreach (var tagId in dto.TagIds)
                    post.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });
            }

            await unitOfWork.Posts.UpdateAsync(post, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var tags = dto.TagIds?.Any() == true
                ? await unitOfWork.Tags.FindAsync(t => dto.TagIds.Contains(t.Id), ct)
                : Enumerable.Empty<Tag>();

<<<<<<< Updated upstream
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
=======
            return Result<PostDto>.Success(MapToDto(post, tags));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gagal update post {PostId}", id);
            return Result<PostDto>.Failure("Gagal memperbarui data post karena kendala teknis.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try 
        {
            var post = await unitOfWork.Posts.GetByIdAsync(id, ct);
            if (post == null)
                return Result<bool>.Failure("Post yang ingin dihapus tidak ditemukan.");

            await unitOfWork.Posts.DeleteAsync(post, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gagal hapus post {PostId}", id);
            return Result<bool>.Failure("Gagal menghapus post. Silakan coba beberapa saat lagi.");
        }
    }

    public async Task<Result<PostDto>> PublishAsync(Guid id, CancellationToken ct = default)
    {
        var post = await unitOfWork.Posts.GetByIdAsync(id, ct);
        if (post == null)
            return Result<PostDto>.Failure("Post tidak ditemukan.");
>>>>>>> Stashed changes

        // update status jadi publish
        post.Status = PostStatus.Published;
        post.PublishedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Posts.UpdateAsync(post, ct);
        await unitOfWork.SaveChangesAsync(ct);

<<<<<<< Updated upstream
        var tags = await _unitOfWork.Tags.GetByPostIdAsync(id, cancellationToken);
        return MapToDto(post, tags);
    }

    public async Task<PostDto?> UnpublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id, cancellationToken);
        if (post == null) return null;
=======
        var tags = await unitOfWork.Tags.GetByPostIdAsync(id, ct);
        return Result<PostDto>.Success(MapToDto(post, tags));
    }

    public async Task<Result<PostDto>> UnpublishAsync(Guid id, CancellationToken ct = default)
    {
        var post = await unitOfWork.Posts.GetByIdAsync(id, ct);
        if (post == null)
            return Result<PostDto>.Failure("Post tidak ditemukan.");
>>>>>>> Stashed changes

        post.Status = PostStatus.Draft;
        post.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Posts.UpdateAsync(post, ct);
        await unitOfWork.SaveChangesAsync(ct);

<<<<<<< Updated upstream
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
=======
        var tags = await unitOfWork.Tags.GetByPostIdAsync(id, ct);
        return Result<PostDto>.Success(MapToDto(post, tags));
    }

    private static PostDto MapToDto(Post post, IEnumerable<Tag> tags) => new(
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
        tags.Select(t => new TagDto(t.Id, t.Name, t.Slug, t.PostTags?.Count ?? 0, t.CreatedAt)),
        post.CreatedAt,
        post.UpdatedAt
    );
>>>>>>> Stashed changes
}

