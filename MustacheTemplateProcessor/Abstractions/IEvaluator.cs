using System.Collections.Generic;

namespace MustacheTemplateProcessor.Abstractions
{
    public interface IEvaluator
    {
        object Eval(string expression, Dictionary<string, object> context);
    }
}