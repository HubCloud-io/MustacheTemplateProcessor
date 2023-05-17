using System;
using System.Collections.Generic;
using System.Linq;
using EvalEngine.Engine;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.LexemeAnalyzer;
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

        private IfStatementBodies GetBodies(string body)
        {
            var lexemeAnalyzer = new LexemeAnalyzer.LexemeAnalyzer();
            var lexemes = lexemeAnalyzer.GetLexemes(body)
                ?.ToList();

            if (lexemes is null || !lexemes.Any(x => x.Type == LexemeType.ElseStatement))
                return new IfStatementBodies
                {
                    TrueStateBody = body,
                    FalseStateBody = string.Empty
                };

            var stack = new Stack<Lexeme>();
            var filteredLexemes = lexemes.Where(x => x.Type != LexemeType.PlainText &&
                                                     x.Type != LexemeType.ValueStatement);
            
            foreach (var lexeme in filteredLexemes)
            {
                switch (lexeme.Type)
                {
                    case LexemeType.IfStatement:
                    case LexemeType.ForStatement:
                        stack.Push(lexeme);
                        break;
                    case LexemeType.ElseStatement:
                        stack.Push(lexeme);
                        break;
                    case LexemeType.EndStatement:
                        var top = stack.Pop();
                        if (top.Type == LexemeType.ElseStatement)
                            stack.Pop();
                        break;
                }
            }

            if (stack.Count != 1)
                return new IfStatementBodies
                {
                    TrueStateBody = body,
                    FalseStateBody = string.Empty
                };

            var currentElse = stack.Pop();
            if (currentElse.Type != LexemeType.ElseStatement)
                return new IfStatementBodies
                {
                    TrueStateBody = body,
                    FalseStateBody = string.Empty
                };

            return new IfStatementBodies
            {
                TrueStateBody = body.Substring(0, currentElse.StartIndex),
                FalseStateBody = body.Substring(currentElse.EndIndex + 1, body.Length - currentElse.EndIndex - 1)
            };
        }
    }
}