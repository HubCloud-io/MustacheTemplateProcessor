using System;
using System.Collections.Generic;
using EvalEngine.Engine;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers.Base;

namespace MustacheTemplateProcessor.StatementParsers
{

    public class IfStatementParser : BaseStatementParser, IStatementParser
    {
        private readonly MustacheParser _parser = new MustacheParser();

        public string Process(StatementContext statementContext)
        {
            if (!IsValidStatementContext(statementContext) || string.IsNullOrEmpty(statementContext.Body))
                return string.Empty;

            if (!IsValidStartStatement(statementContext))
                return statementContext.StartStatement.Statement;

            var expression = statementContext.StartStatement.PureStatement
                .Replace(Statements.If + " ", "")
                .Trim();

            var evaluator = new FormulaEvaluator(new Dictionary<string, object>(statementContext.Context));
            try
            {
                var state = evaluator.Eval<bool>(expression);
                if (!state)
                    return string.Empty;

                var val = _parser.Process(statementContext.Body, new Dictionary<string, object>(statementContext.Context));
                return val;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}