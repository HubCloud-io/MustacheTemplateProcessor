using System;
using System.Collections.Generic;
using EvalEngine.Engine;
using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor.StatementParsers
{

    public class IfStatementParser : IStatementParser
    {
        private readonly MustacheParser _parser = new MustacheParser();

        public string Process(StatementContext statementContext)
        {
            var context = statementContext.Context;
            if (context is null || string.IsNullOrEmpty(statementContext.StartStatement?.Statement) ||
                string.IsNullOrEmpty(statementContext.Body))
                return string.Empty;

            if (statementContext.StartStatement.Statement.IndexOf("{{", StringComparison.InvariantCulture) == -1 ||
                statementContext.StartStatement.Statement.IndexOf("}}", StringComparison.InvariantCulture) == -1)
                return statementContext.StartStatement.Statement;

            var expression = statementContext.StartStatement.PureStatement
                .Replace("if", "")
                .Trim();

            var evaluator = new FormulaEvaluator(new Dictionary<string, object>(context));
            try
            {
                var state = evaluator.Eval<bool>(expression);
                if (!state)
                    return string.Empty;

                var val = _parser.Parse(statementContext.Body, new Dictionary<string, object>(context));
                return val;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}