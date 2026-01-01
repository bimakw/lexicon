using Ganss.Xss;
using Lexicon.Application.Interfaces;

namespace Lexicon.Infrastructure.Security;

public class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer _sanitizer;

    public HtmlSanitizerService()
    {
        _sanitizer = new HtmlSanitizer();

        // Configure allowed tags for blog content
        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedTags.Add("p");
        _sanitizer.AllowedTags.Add("br");
        _sanitizer.AllowedTags.Add("strong");
        _sanitizer.AllowedTags.Add("b");
        _sanitizer.AllowedTags.Add("em");
        _sanitizer.AllowedTags.Add("i");
        _sanitizer.AllowedTags.Add("u");
        _sanitizer.AllowedTags.Add("s");
        _sanitizer.AllowedTags.Add("strike");
        _sanitizer.AllowedTags.Add("h1");
        _sanitizer.AllowedTags.Add("h2");
        _sanitizer.AllowedTags.Add("h3");
        _sanitizer.AllowedTags.Add("h4");
        _sanitizer.AllowedTags.Add("h5");
        _sanitizer.AllowedTags.Add("h6");
        _sanitizer.AllowedTags.Add("ul");
        _sanitizer.AllowedTags.Add("ol");
        _sanitizer.AllowedTags.Add("li");
        _sanitizer.AllowedTags.Add("a");
        _sanitizer.AllowedTags.Add("img");
        _sanitizer.AllowedTags.Add("blockquote");
        _sanitizer.AllowedTags.Add("pre");
        _sanitizer.AllowedTags.Add("code");
        _sanitizer.AllowedTags.Add("table");
        _sanitizer.AllowedTags.Add("thead");
        _sanitizer.AllowedTags.Add("tbody");
        _sanitizer.AllowedTags.Add("tr");
        _sanitizer.AllowedTags.Add("th");
        _sanitizer.AllowedTags.Add("td");
        _sanitizer.AllowedTags.Add("div");
        _sanitizer.AllowedTags.Add("span");
        _sanitizer.AllowedTags.Add("figure");
        _sanitizer.AllowedTags.Add("figcaption");
        _sanitizer.AllowedTags.Add("hr");

        // Configure allowed attributes
        _sanitizer.AllowedAttributes.Clear();
        _sanitizer.AllowedAttributes.Add("href");
        _sanitizer.AllowedAttributes.Add("src");
        _sanitizer.AllowedAttributes.Add("alt");
        _sanitizer.AllowedAttributes.Add("title");
        _sanitizer.AllowedAttributes.Add("class");
        _sanitizer.AllowedAttributes.Add("id");
        _sanitizer.AllowedAttributes.Add("target");
        _sanitizer.AllowedAttributes.Add("rel");
        _sanitizer.AllowedAttributes.Add("width");
        _sanitizer.AllowedAttributes.Add("height");

        // Only allow safe URL schemes
        _sanitizer.AllowedSchemes.Clear();
        _sanitizer.AllowedSchemes.Add("http");
        _sanitizer.AllowedSchemes.Add("https");
        _sanitizer.AllowedSchemes.Add("mailto");

        // Force rel="noopener noreferrer" on external links
        _sanitizer.PostProcessNode += (s, e) =>
        {
            if (e.Node is AngleSharp.Html.Dom.IHtmlAnchorElement anchor)
            {
                var href = anchor.GetAttribute("href");
                if (!string.IsNullOrEmpty(href) && (href.StartsWith("http://") || href.StartsWith("https://")))
                {
                    anchor.SetAttribute("rel", "noopener noreferrer");
                    anchor.SetAttribute("target", "_blank");
                }
            }
        };
    }

    public string Sanitize(string html)
    {
        if (string.IsNullOrEmpty(html))
            return html;

        return _sanitizer.Sanitize(html);
    }

    public string SanitizeWithAllowedTags(string html, params string[] allowedTags)
    {
        if (string.IsNullOrEmpty(html))
            return html;

        var customSanitizer = new HtmlSanitizer();
        customSanitizer.AllowedTags.Clear();

        foreach (var tag in allowedTags)
        {
            customSanitizer.AllowedTags.Add(tag);
        }

        return customSanitizer.Sanitize(html);
    }
}
