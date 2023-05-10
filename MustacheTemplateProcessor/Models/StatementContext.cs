namespace MustacheTemplateProcessor.Models;

public class StatementContext
{
    public ParsedStatement? StartStatement { get; set; }
    public ParsedStatement? EndStatement { get; set; }
    public string? Body { get; set; }
    public dynamic? Context { get; set; }
}