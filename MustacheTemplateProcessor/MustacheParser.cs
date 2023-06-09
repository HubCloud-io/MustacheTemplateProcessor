﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using MustacheTemplateProcessor.Abstractions;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers;
using MustacheTemplateProcessor.StatementParsers.Base;

namespace MustacheTemplateProcessor
{
    public class MustacheParser : IMustacheParser
    {
        private readonly IEvaluator _evaluator;

        public MustacheParser(IEvaluator evaluator)
        {
            _evaluator = evaluator;
        }

        public static bool TemplateContainsExpressions(string template)
            => template?.Contains(Statements.StartSymbol) ?? false;

        public string Process(string expression,
            Dictionary<string, object> context,
            ILogger logger = null,
            int maxIterationCount = 1000)
        {
            var output = new StringBuilder();

            logger?.LogInformation($"Process started, Expression = {expression}");
            var currentIteration = 0;
            do
            {
                var startStatement = StatementHelper.GetStartStatement(expression);
                logger?.LogInformation($"StartStatement = {startStatement}");
                if (startStatement is null)
                {
                    output.Append(expression);
                    break;
                }
                
                output.Append(expression.Substring(0, startStatement.StartIndex));
                var endStatement = StatementHelper.GetEndStatement(expression, startStatement);
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
                output.Append(statementValue);
                logger?.LogInformation($"StatementValue = {statementValue}");

                expression = expression.Substring(endStatement.EndIndex + 1,
                    expression.Length - endStatement.EndIndex - 1);

                currentIteration++;

                // Emergency exit
                if (expression.Any() && currentIteration > maxIterationCount)
                {
                    logger?.LogWarning(
                        $"Emergency exit:: currentIteration={currentIteration}, MaxIterationCount = {maxIterationCount}");
                    output.Append(expression);
                    break;
                }
            } while (expression.Any());

            logger?.LogInformation($"Process ended");
            return output.ToString();
        }

        private string GetStatementValue(StatementContext statementContext, StatementType type)
        {
            var result = string.Empty;
            IStatementParser parser;
            switch (type)
            {
                case StatementType.If:
                    parser = new IfStatementParser(_evaluator);
                    break;
                case StatementType.For:
                    parser = new ForStatementParser(_evaluator);
                    break;
                case StatementType.Value:
                    parser = new SimpleValueParser(_evaluator);
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
                expression = expression.Substring(startStatement.EndIndex + 1,
                    endStatement.StartIndex - startStatement.EndIndex - 1);

            return expression;
        }
    }
}