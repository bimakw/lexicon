namespace Lexicon.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPostRepository Posts { get; }
    ICategoryRepository Categories { get; }
    ITagRepository Tags { get; }
    IAuthorRepository Authors { get; }
    ICommentRepository Comments { get; }
    IMediaRepository Media { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
