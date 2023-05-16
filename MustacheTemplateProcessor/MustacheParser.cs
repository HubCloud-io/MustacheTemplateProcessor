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

        public string Process(string expression, Dictionary<string, object> context)
        {
            var output = string.Empty;
            var innerExpression = expression;

            var currentIteration = 0;
            do
            {
                ParsedStatement startStatement;
                ParsedStatement endStatement = null;
                StatementContext statementContext = null;

                try
                {
                    startStatement = _statementHelper.GetStartStatement(innerExpression);
                }
                catch (NoStatementException)
                {
                    output += innerExpression;
                    break;
                }

                output += innerExpression.Substring(0, startStatement.StartIndex);
                var type = _statementHelper.GetStatementType(startStatement);

                // ToDo: записывать в endStatement то же значение и для SimpleValue
                if (type == StatementType.For || type == StatementType.If)
                {
                    endStatement = _statementHelper.GetEndStatement(innerExpression, startStatement);
                    if (endStatement != null)
                    {
                        statementContext = new StatementContext
                        {
                            StartStatement = startStatement,
                            EndStatement = endStatement,
                            Body = innerExpression.Substring(startStatement.EndIndex,
                                endStatement.StartIndex - startStatement.EndIndex),
                            Context = context
                        };
                    }
                }
                else
                {
                    statementContext = new StatementContext
                    {
                        StartStatement = startStatement,
                        Context = context
                    };
                }

                if (statementContext != null)
                    output += GetStatementValue(statementContext, type);

                if ((type == StatementType.For || type == StatementType.If) && endStatement != null)
                    innerExpression = innerExpression.Substring(endStatement.EndIndex + 1,
                        innerExpression.Length - endStatement.EndIndex - 1);
                else
                    innerExpression = innerExpression.Substring(startStatement.EndIndex,
                        innerExpression.Length - startStatement.EndIndex);

                currentIteration++;
                
                // Emergency exit
                if (innerExpression.Any() && currentIteration > MaxIterationCount)
                {
                    output += innerExpression;
                    break;
                }
                    
            } while (innerExpression.Any());

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