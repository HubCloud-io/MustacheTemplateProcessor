using System.Collections.Generic;
using System.Linq;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers;
using MustacheTemplateProcessor.StatementParsers.Base;

namespace MustacheTemplateProcessor
{
    public class MustacheParser : IMustacheParser
    {
        private const int MaxIterationCount = 1000;
        private readonly StatementHelper _statementHelper = new StatementHelper();

        public static bool TemplateContainsExpressions(string template)
            => template?.Contains(Statements.StartSymbol) ?? false;

        private string GetBody(string expression, ParsedStatement startStatement, ParsedStatement endStatement)
        {
            if (startStatement.Type == StatementType.If ||
                startStatement.Type == StatementType.For)
                return expression.Substring(startStatement.EndIndex + 1,
                    endStatement.StartIndex - startStatement.EndIndex - 1);

            return null;
        }

        public string Process(string expression, Dictionary<string, object> context)
        {
            var output = string.Empty;

            var currentIteration = 0;
            do
            {
                ParsedStatement startStatement;

                try
                {
                    startStatement = _statementHelper.GetStartStatement(expression);
                }
                catch (NoStatementException)
                {
                    output += expression;
                    break;
                }

                output += expression.Substring(0, startStatement.StartIndex);
                var endStatement = _statementHelper.GetEndStatement(expression, startStatement);
                var statementContext = new StatementContext
                {
                    StartStatement = startStatement,
                    EndStatement = endStatement,
                    Body = GetBody(expression, startStatement, endStatement),
                    Context = context
                };

                output += GetStatementValue(statementContext, startStatement.Type);

                expression = expression.Substring(endStatement.EndIndex + 1, expression.Length - endStatement.EndIndex - 1);

                currentIteration++;

                // Emergency exit
                if (expression.Any() && currentIteration > MaxIterationCount)
                {
                    output += expression;
                    break;
                }
            } while (expression.Any());

            return output;
        }

        private string GetStatementValue(StatementContext statementContext, StatementType type)
        {
            var result = string.Empty;
            IStatementParser parser;
            switch (type)
            {
                case StatementType.If:
                    parser = new IfStatementParser();
                    break;
                case StatementType.For:
                    parser = new ForStatementParser();
                    break;
                case StatementType.Value:
                    parser = new SimpleValueParser();
                    break;
                default:
                    return result;
            }

            result = parser.Process(statementContext);
            return result;
        }
    }
}