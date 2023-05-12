using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.LexemeAnalyzer;
using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor;

public class StatementHelper
{
    public StatementType GetStatementType(ParsedStatement? statement)
    {
        if (string.IsNullOrEmpty(statement?.Statement))
            return StatementType.Undefined;

        if (statement.Statement.IndexOf("for", StringComparison.InvariantCulture) != -1)
            return StatementType.For;

        if (statement.Statement.IndexOf("if", StringComparison.InvariantCulture) != -1)
            return StatementType.If;

        return StatementType.Value;
    }

    public ParsedStatement GetStartStatement(string expression)
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
}