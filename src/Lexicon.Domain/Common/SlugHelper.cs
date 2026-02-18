using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexicon.Domain.Common;

public static class SlugHelper
{
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Remove diacritics (accents)
        text = RemoveDiacritics(text);

        // Replace spaces with hyphens
        text = text.Replace(" ", "-");

        // Remove special characters except hyphens and alphanumeric
        // We keep ASCII alphanumeric and hyphens
        text = Regex.Replace(text, @"[^a-z0-9\-]", "");

        // Remove multiple hyphens
        text = Regex.Replace(text, @"-+", "-");

        // Trim hyphens from ends
        text = text.Trim('-');

        return text;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
