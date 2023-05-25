using System;
using System.Linq;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.LexemeAnalyzer;
using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor
{
    public class StatementHelper
    {
        public static ParsedStatement GetStartStatement(string expression)
        {
            var statementStart = expression.IndexOf(Statements.StartSymbol, StringComparison.InvariantCultureIgnoreCase);
            if (statementStart == -1)
                return null;

            var statementEnd = expression.IndexOf(Statements.EndSymbol, StringComparison.InvariantCultureIgnoreCase);
            if (statementEnd == -1)
                throw new StatementParseException();

            statementEnd += 1;
            var statement = expression.Substring(statementStart, statementEnd + 1 - statementStart);
            var statementType = GetStatementType(statement);
            
            if ((statementType == StatementType.If || statementType == StatementType.For) && expression.Length > statementEnd + 1)
            {
                do
                {
                    if (expression[statementEnd + 1] == '\r' || expression[statementEnd + 1] == '\n')
                        statementEnd++;
                    if (expression.Length <= statementEnd + 1)
                        break;
                } while (expression[statementEnd + 1] == '\r' || expression[statementEnd + 1] == '\n');
            }

            return new ParsedStatement
            {
                Statement = statement,
                StartIndex = statementStart,
                EndIndex = statementEnd,
                Type = statementType
            };
        }

        public static ParsedStatement GetEndStatement(string expression, ParsedStatement startStatement)
        {
            if (startStatement.Type == StatementType.Value)
                return new ParsedStatement
                {
                    Statement = startStatement.Statement,
                    StartIndex = startStatement.StartIndex,
                    EndIndex = startStatement.EndIndex
                };
            
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
                {
                    var statementType = GetStatementType(lexeme.Value);
                    var statementEnd = lexeme.EndIndex;
                    if (statementType == StatementType.End && expression.Length > statementEnd + 1)
                    {
                        do
                        {
                            if (expression[statementEnd + 1] == '\r' || expression[statementEnd + 1] == '\n')
                                statementEnd++;
                            if (expression.Length <= statementEnd + 1)
                                break;
                        } while (expression[statementEnd + 1] == '\r' || expression[statementEnd + 1] == '\n');
                    }
                    
                    var parsedStatement = new ParsedStatement
                    {
                        Statement = lexeme.Value,
                        StartIndex = lexeme.StartIndex,
                        EndIndex = statementEnd,
                        Type = statementType
                    };

                    return parsedStatement;
                }

                if (lexeme.Type == LexemeType.EndStatement && balance != 0)
                    balance--;
            }

            return null;
        }
        
        private static StatementType GetStatementType(string statement)
        {
            if (string.IsNullOrEmpty(statement))
                return StatementType.Undefined;

            if (statement.IndexOf(Statements.For, StringComparison.InvariantCultureIgnoreCase) != -1)
                return StatementType.For;

            if (statement.IndexOf(Statements.If, StringComparison.InvariantCultureIgnoreCase) != -1)
                return StatementType.If;
            
            if (statement.IndexOf(Statements.End, StringComparison.InvariantCultureIgnoreCase) != -1)
                return StatementType.End;

            return StatementType.Value;
        }
    }
}