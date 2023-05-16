using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MustacheTemplateProcessor
{
    public interface IMustacheParser
    {
        string Process(string expression, Dictionary<string, object> context, ILogger logger, int maxIterationCount);
    }
}