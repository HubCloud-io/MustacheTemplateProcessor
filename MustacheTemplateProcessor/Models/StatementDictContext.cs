namespace MustacheTemplateProcessor.Models;

public class StatementDictContext : StatementContextBase
{
    public IDictionary<string, object>? Context { get; set; }
}