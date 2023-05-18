using System.Collections.Generic;
using System.Linq;
using EvalEngine.Engine;
using MustacheTemplateProcessor.Abstractions;

namespace MustacheTemplateProcessor.Tests.Mocks
{
    public class EvaluatorMock : IEvaluator
    {
        public T Eval<T>(string expression, Dictionary<string, object> context)
        {
            var evaluator = new FormulaEvaluator(context);
            return evaluator.Eval<T>(expression);
        }

        public object Eval(string expression, Dictionary<string, object> context)
        {
            var evaluator = new FormulaEvaluator(context);
            return evaluator.Eval(expression);
        }
    }
}