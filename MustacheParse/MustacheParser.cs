using MustacheParse.Common;
using MustacheParse.LexemeAnalyzer;
using MustacheParse.Models;
using MustacheParse.StatementParsers;

namespace MustacheParse;

public class MustacheParser
{
    internal enum StatementType
    {
        Undefined,
        For,
        If,
        Value
    }

    public string Parse(string expression, dynamic context)
    {
        var output = string.Empty;
        var innerExpression = expression;

        do
        {
            ParsedStatement? startStatement = null;
            ParsedStatement? endStatement = null;
            StatementContext? statementContext = null;

            try
            {
                startStatement = GetStartStatement(innerExpression);
            }
            catch (NoStatementException)
            {
                output += innerExpression;
                break;
            }

            output += innerExpression.Substring(0, startStatement.StartIndex);
            var type = GetStatementType(startStatement);

            if (type == StatementType.For || type == StatementType.If)
            {
                endStatement = GetEndStatement(innerExpression, startStatement);
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


    #region statement helpers

    private StatementType GetStatementType(ParsedStatement? statement)
    {
        if (string.IsNullOrEmpty(statement?.Statement))
            return StatementType.Undefined;

        if (statement.Statement.IndexOf("for", StringComparison.InvariantCulture) != -1)
            return StatementType.For;

        if (statement.Statement.IndexOf("if", StringComparison.InvariantCulture) != -1)
            return StatementType.If;

        return StatementType.Value;
    }

    public ParsedStatement? GetEndStatement(string expression, ParsedStatement startStatement)
    {
        var analyzer = new LexemeAnalyzer.LexemeAnalyzer();
        var lexemes = analyzer.GetLexemes(expression)
            .Where(x => x.Type == LexemeType.ForStatement || 
                        x.Type == LexemeType.IfStatement || 
                        x.Type == LexemeType.EndStatement)
            .ToList();

        if (lexemes.Count > 0)
            lexemes.Remove(lexemes.First());

        var balance = 0;
        foreach (var lexeme in lexemes)
        {
            if (lexeme.Type == LexemeType.ForStatement || lexeme.Type == LexemeType.IfStatement)
                balance++;
            
            if (lexeme.Type == LexemeType.EndStatement && balance == 0)
                return new ParsedStatement
                {
                    Statement = lexeme.Value,
                    StartIndex = lexeme.StartIndex,
                    EndIndex = lexeme.EndIndex
                };

            if (lexeme.Type == LexemeType.EndStatement && balance != 0)
                balance--;
        }

        return null;
    }

    private ParsedStatement GetStartStatement(string expression)
    {
        var statementStart = expression.IndexOf("{{", StringComparison.InvariantCulture);
        if (statementStart == -1)
            throw new NoStatementException();

        var statementEnd = expression.IndexOf("}}", StringComparison.InvariantCulture);
        if (statementEnd == -1)
            throw new StatementParseException();

        var statement = expression.Substring(statementStart, statementEnd + 2 - statementStart);

        return new ParsedStatement
        {
            Statement = statement,
            StartIndex = statementStart,
            EndIndex = statementStart + statement.Length
        };
    }

    #endregion
    
    // public string Parse(string expression, dynamic context)
    // {
    //     var output = string.Empty;
    //
    //     if (string.IsNullOrEmpty(expression?.Trim()))
    //         return output;
    //
    //     var innerExpression = expression;
    //
    //     ParsedStatement startStatement;
    //     ParsedStatement endStatement;
    //     try
    //     {
    //         startStatement = GetStartStatement(expression);
    //     }
    //     catch (NoStatementException)
    //     {
    //         return expression;
    //     }
    //
    //     try
    //     {
    //         endStatement = GetEndStatement(expression, null);
    //     }
    //     catch (NoStatementException)
    //     {
    //         return expression;
    //     }
    //
    //     var statementContext = new StatementContext
    //     {
    //         StartStatement = startStatement,
    //         EndStatement = endStatement,
    //         Body = expression.Substring(startStatement.EndIndex, endStatement.StartIndex - startStatement.EndIndex),
    //         Context = context
    //     };
    //
    //     var forStatementParser = new ForStatementParser();
    //     output += expression.Substring(0, startStatement.StartIndex);
    //     output += forStatementParser.Process(statementContext);
    //     output += expression.Substring(endStatement.EndIndex, expression.Length - endStatement.EndIndex);
    //
    //     return output;
    // }
}