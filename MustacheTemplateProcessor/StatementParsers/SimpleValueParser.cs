using System;
using System.Collections.Generic;
using EvalEngine.Engine;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers.Base;

namespace MustacheTemplateProcessor.StatementParsers
{
    public class SimpleValueParser : BaseStatementParser, IStatementParser
    {
        public string Process(StatementContext statementContext)
        {
            if (!IsValidStatementContext(statementContext))
                return string.Empty;
            
            if (!IsValidStartStatement(statementContext))
                return statementContext.StartStatement.Statement;

            var expression = statementContext.StartStatement.PureStatement.Trim();

            try
            {
                var evaluator = new FormulaEvaluator(new Dictionary<string, object>(statementContext.Context));
                var result = evaluator.Eval(expression);
                return result?.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}