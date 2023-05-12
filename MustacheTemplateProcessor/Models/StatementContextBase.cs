namespace MustacheTemplateProcessor.Models;

public class StatementContextBase
{
    public ParsedStatement? StartStatement { get; set; }
    public ParsedStatement? EndStatement { get; set; }
    public string? Body { get; set; }
    
}