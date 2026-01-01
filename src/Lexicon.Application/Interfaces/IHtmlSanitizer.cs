namespace Lexicon.Application.Interfaces;

public interface IHtmlSanitizerService
{
    string Sanitize(string html);
    string SanitizeWithAllowedTags(string html, params string[] allowedTags);
}
