using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers;

namespace MustacheTemplateProcessor;

public class MustacheParser : IMustacheParser
{
    private readonly StatementHelper _statementHelper = new();

    public string Parse(string expression, Dictionary<string, object> context)
    {
        var output = string.Empty;
        var innerExpression = expression;
        
        do
        {
            ParsedStatement? startStatement;
            ParsedStatement? endStatement = null;
            StatementContext? statementContext = null;

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

            if (type == StatementType.For || type == StatementType.If)
            {
                endStatement = _statementHelper.GetEndStatement(innerExpression, startStatement);
                if (endStatement != null)
                {
                    statementContext = new StatementContext
                    {
                        StartStatement = startStatement,
                        EndStatement = endStatement,
                        Body = innerExpression.Substring(startStatement.EndIndex, endStatement.StartIndex - startStatement.EndIndex),
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

            if (type == StatementType.For || type == StatementType.If)
                innerExpression = innerExpression.Substring(endStatement!.EndIndex + 1,
                    innerExpression.Length - endStatement.EndIndex - 1);
            else
                innerExpression = innerExpression.Substring(startStatement.EndIndex,
                    innerExpression.Length - startStatement.EndIndex);

            if (!innerExpression.Any())
                break;
        } while (true);

        return output;
    }
    
    private string? GetStatementValue(StatementContext statementContext, StatementType type)
    {
        var result = string.Empty;
        IStatementParser parser;
        switch (type)
        {
            // ToDo: StatementType.If implement
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