using Lexicon.Domain.Entities;

namespace Lexicon.Domain.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
