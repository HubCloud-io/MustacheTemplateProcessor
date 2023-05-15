using System.Collections.Generic;

namespace MustacheTemplateProcessor
{
    public interface IMustacheParser
    {
        string Process(string expression, Dictionary<string, object> context);
    }
}