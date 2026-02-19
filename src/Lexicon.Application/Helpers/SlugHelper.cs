namespace Lexicon.Application.Helpers;

public static class SlugHelper
{
    public static string GenerateSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        return name.ToLowerInvariant()
            .Trim()
            .Replace(" ", "-") // spasi jadi dash
            .Replace("--", "-") // double dash jadi single
            .Trim('-'); // hapus dash di awal/akhir
    }
}
