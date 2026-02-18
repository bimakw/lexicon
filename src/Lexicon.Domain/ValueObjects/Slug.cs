using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexicon.Domain.ValueObjects;

public record Slug
{
    public string Value { get; init; }

    private Slug(string value)
    {
        Value = value;
    }

    public static Slug Create(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty", nameof(text));

        var slugValue = GenerateSlug(text);
        return new Slug(slugValue);
    }

    public static Slug FromExisting(string value) => new(value);

    private static string GenerateSlug(string text)
    {
        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Remove diacritics
        text = RemoveDiacritics(text);

        // Replace spaces and special characters with hyphens
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", "-").Trim();
        
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

    public override string ToString() => Value;
    public static implicit operator string(Slug slug) => slug.Value;
}
