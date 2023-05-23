using System;
using System.Collections.Generic;
using MustacheTemplateProcessor.Abstractions;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers.Base;

namespace MustacheTemplateProcessor.StatementParsers
{
    public class SimpleValueParser : BaseStatementParser, IStatementParser
    {
        public SimpleValueParser(IEvaluator evaluator) : base(evaluator)
        {
        }
        
        public string Process(StatementContext statementContext)
        {
            if (!IsValidStatementContext(statementContext))
                return string.Empty;
            
            if (!IsValidStartStatement(statementContext))
                return statementContext.StartStatement.Statement;

            var expression = statementContext.StartStatement.PureStatement.Trim();

            try
            {
                var result = Evaluator.Eval(expression, new Dictionary<string, object>(statementContext.Context));
                return result?.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}