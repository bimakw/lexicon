using Lexicon.Domain.Interfaces;
using Lexicon.Infrastructure.Data;

namespace Lexicon.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LexiconDbContext _context;
    private IPostRepository? _posts;
    private ICategoryRepository? _categories;
    private ITagRepository? _tags;
    private IAuthorRepository? _authors;
    private ICommentRepository? _comments;
    private IMediaRepository? _media;

    public UnitOfWork(LexiconDbContext context)
    {
        _context = context;
    }

    public IPostRepository Posts => _posts ??= new PostRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public ITagRepository Tags => _tags ??= new TagRepository(_context);
    public IAuthorRepository Authors => _authors ??= new AuthorRepository(_context);
    public ICommentRepository Comments => _comments ??= new CommentRepository(_context);
    public IMediaRepository Media => _media ??= new MediaRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
