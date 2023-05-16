﻿using System;
using System.Linq;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.LexemeAnalyzer;
using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor
{
    public class StatementHelper
    {
        private StatementType GetStatementType(string statement)
        {
            if (string.IsNullOrEmpty(statement))
                return StatementType.Undefined;

            if (statement.IndexOf(Statements.For, StringComparison.InvariantCulture) != -1)
                return StatementType.For;

            if (statement.IndexOf(Statements.If, StringComparison.InvariantCulture) != -1)
                return StatementType.If;

            return StatementType.Value;
        }

        public ParsedStatement GetStartStatement(string expression)
        {
            var statementStart = expression.IndexOf(Statements.StartSymbol, StringComparison.InvariantCulture);
            if (statementStart == -1)
                throw new NoStatementException();

            var statementEnd = expression.IndexOf(Statements.EndSymbol, StringComparison.InvariantCulture);
            if (statementEnd == -1)
                throw new StatementParseException();

            var statement = expression.Substring(statementStart, statementEnd + 2 - statementStart);

            return new ParsedStatement
            {
                Statement = statement,
                StartIndex = statementStart,
                EndIndex = statementEnd + 1,
                Type = GetStatementType(statement)
            };
        }

        public ParsedStatement GetEndStatement(string expression, ParsedStatement startStatement)
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
}