namespace MustacheTemplateProcessor.Models;

public class ParsedStatement
{
    public string? Statement { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }

    public string? PureStatement => Statement
        ?.Trim()
        .Replace("{{", "")
        .Replace("}}", "");
}