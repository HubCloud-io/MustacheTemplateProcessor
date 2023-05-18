using System.Collections.Generic;

namespace MustacheTemplateProcessor.Abstractions
{
    public interface IEvaluator
    {
        T Eval<T>(string expression, Dictionary<string, object> context);
        object Eval(string expression, Dictionary<string, object> context);
    }
}