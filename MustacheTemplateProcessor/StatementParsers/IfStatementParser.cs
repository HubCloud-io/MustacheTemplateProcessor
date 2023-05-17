using System;
using System.Collections.Generic;
using System.Linq;
using EvalEngine.Engine;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers.Base;

namespace MustacheTemplateProcessor.StatementParsers
{
    public class IfStatementBodies
    {
        public string TrueStateBody { get; set; }
        public string FalseStateBody { get; set; }
    }

    public class IfStatementParser : BaseStatementParser, IStatementParser
    {
        private readonly MustacheParser _parser = new MustacheParser();

        public string Process(StatementContext statementContext)
        {
            if (!IsValidStatementContext(statementContext) || string.IsNullOrEmpty(statementContext.Body))
                return string.Empty;

            if (!IsValidStartStatement(statementContext))
                return statementContext.StartStatement.Statement;

            var condition = statementContext.StartStatement.PureStatement
                .Replace(Statements.If + " ", "")
                .Trim();

            var bodies = GetBodies(statementContext.Body);
            var evaluator = new FormulaEvaluator(new Dictionary<string, object>(statementContext.Context));
            try
            {
                string value;
                var state = evaluator.Eval<bool>(condition);
                if (state)
                    value = _parser.Process(bodies.TrueStateBody,
                        new Dictionary<string, object>(statementContext.Context));
                else
                    value = _parser.Process(bodies.FalseStateBody,
                        new Dictionary<string, object>(statementContext.Context));

                return value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public IfStatementBodies GetBodies(string body)
        {
            var fullElseStatement = "{{" + Statements.Else + "}}";
            
            var elseStartIndex = body.IndexOf(fullElseStatement, StringComparison.InvariantCultureIgnoreCase);
            if (elseStartIndex == -1)
                return new IfStatementBodies
                {
                    TrueStateBody = body,
                    FalseStateBody = string.Empty
                };

            var endIndex = elseStartIndex + fullElseStatement.Length;
            return new IfStatementBodies
            {
                TrueStateBody = body.Substring(0, elseStartIndex),
                FalseStateBody = body.Substring(endIndex, body.Length - endIndex)
            };
        }
    }
}