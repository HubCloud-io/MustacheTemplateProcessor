using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers;
using MustacheTemplateProcessor.StatementParsers.Base;

namespace MustacheTemplateProcessor
{
    public class MustacheParser : IMustacheParser
    {
        private readonly StatementHelper _statementHelper = new StatementHelper();

        public static bool TemplateContainsExpressions(string template)
            => template?.Contains(Statements.StartSymbol) ?? false;

        public string Process(string expression,
            Dictionary<string, object> context,
            ILogger logger = null,
            int maxIterationCount = 1000)
        {
            var output = string.Empty;

            logger?.LogInformation($"Process started, Expression = {expression}");
            var currentIteration = 0;
            do
            {
                ParsedStatement startStatement;
                try
                {
                    startStatement = _statementHelper.GetStartStatement(expression);
                    logger?.LogInformation($"StartStatement = {startStatement}");
                }
                catch (NoStatementException)
                {
                    logger?.LogInformation($"Expression = {expression}, throw NoStatementException");
                    output += expression;
                    break;
                }

                output += expression.Substring(0, startStatement.StartIndex);
                var endStatement = _statementHelper.GetEndStatement(expression, startStatement);
                logger?.LogInformation($"EndStatement = {endStatement}");
                var statementContext = new StatementContext
                {
                    StartStatement = startStatement,
                    EndStatement = endStatement,
                    Body = GetBody(expression, startStatement, endStatement),
                    Context = context
                };
                logger?.LogInformation($"StatementContext Body = {statementContext.Body}");

                var statementValue = GetStatementValue(statementContext, startStatement.Type);
                output += statementValue;
                logger?.LogInformation($"StatementValue = {statementValue}");

                expression = expression.Substring(endStatement.EndIndex + 1, expression.Length - endStatement.EndIndex - 1);

                currentIteration++;

                // Emergency exit
                if (expression.Any() && currentIteration > maxIterationCount)
                {
                    logger?.LogWarning($"Emergency exit:: currentIteration={currentIteration}, MaxIterationCount = {maxIterationCount}");
                    output += expression;
                    break;
                }
            } while (expression.Any());
            
            logger?.LogInformation($"Process ended");
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
        
        private string GetBody(string expression, ParsedStatement startStatement, ParsedStatement endStatement)
        {
            if (startStatement.Type == StatementType.If ||
                startStatement.Type == StatementType.For)
                expression = expression.Substring(startStatement.EndIndex + 1, endStatement.StartIndex - startStatement.EndIndex - 1);

            return expression;
        }

        private string TrimStartNewLineSymbols(string expression)
        {
            do
            {
                if (expression[0] == '\r' || expression[0] == '\n')
                    expression = expression.Substring(1, expression.Length - 1);
                    
                if (string.IsNullOrEmpty(expression))
                    break;

            } while (expression[0] == '\r' || expression[0] == '\n');
            
            return expression;
        }
    }
}