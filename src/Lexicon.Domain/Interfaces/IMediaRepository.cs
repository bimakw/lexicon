using Lexicon.Domain.Entities;

namespace Lexicon.Domain.Interfaces;

public interface IMediaRepository : IRepository<Media>
{
    Task<IEnumerable<Media>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken = default);
}
