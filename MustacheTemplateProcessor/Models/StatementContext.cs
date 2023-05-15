using System.Collections.Generic;

namespace MustacheTemplateProcessor.Models
{
    public class StatementContext
    {
        public ParsedStatement StartStatement { get; set; }
        public ParsedStatement EndStatement { get; set; }
        public string Body { get; set; }
        public IDictionary<string, object> Context { get; set; }
    }
}