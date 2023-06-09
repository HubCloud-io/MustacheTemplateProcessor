﻿using System.Collections.Generic;
using System.Linq;
using EvalEngine.Engine;
using MustacheTemplateProcessor.Abstractions;

namespace MustacheTemplateProcessor.Tests.Mocks
{
    public class EvaluatorMock : IEvaluator
    {
        public object Eval(string expression, Dictionary<string, object> context)
        {
            var evaluator = new FormulaEvaluator(context);
            return evaluator.Eval(expression);
        }
    }

    public class EvaluatorMock2 : IEvaluator
    {
        private readonly FormulaEvaluator _formulaEvaluator = new FormulaEvaluator();
        public object Eval(string expression, Dictionary<string, object> context)
        {
            return _formulaEvaluator.Eval(expression, context);
        }
    }
}