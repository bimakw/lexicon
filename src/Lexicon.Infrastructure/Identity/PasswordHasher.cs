using Lexicon.Application.Identity;

namespace Lexicon.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public bool NeedsRehash(string hash)
    {
        return BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, WorkFactor);
    }
}
