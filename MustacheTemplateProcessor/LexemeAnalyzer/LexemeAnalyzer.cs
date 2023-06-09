﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MustacheTemplateProcessor.Common;

namespace MustacheTemplateProcessor.LexemeAnalyzer
{
    public class LexemeAnalyzer
    {
        public IEnumerable<Lexeme> GetLexemes(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return Enumerable.Empty<Lexeme>();

            var lexemeList = new List<Lexeme>();

            var current = new StringBuilder();
            var isLexeme = false;
            var endBracket = 0;

            var currentIndex = 0;
            var currentLexemeStartIndex = 0;
            foreach (var ch in expression)
            {
                if (ch == '{' && !isLexeme)
                {
                    isLexeme = true;
                    var str = current.ToString();
                    lexemeList.Add(new Lexeme
                    {
                        Value = str,
                        Type = GetType(str),
                        StartIndex = currentLexemeStartIndex,
                        EndIndex = currentIndex - 1
                    });

                    current.Clear();
                    currentLexemeStartIndex = currentIndex;
                }

                if (ch == '}' && endBracket < 2)
                    endBracket++;

                current.Append(ch);

                if (endBracket == 2)
                {
                    endBracket = 0;
                    isLexeme = false;
                    
                    var str = current.ToString();
                    lexemeList.Add(new Lexeme
                    {
                        Value = str,
                        Type = GetType(str),
                        StartIndex = currentLexemeStartIndex,
                        EndIndex = currentIndex
                    });

                    current.Clear();
                    currentLexemeStartIndex = currentIndex + 1;
                }

                currentIndex++;
            }

            // if (!string.IsNullOrEmpty(current))
            if (current.Length != 0)
            {
                var str = current.ToString();
                lexemeList.Add(new Lexeme
                {
                    Value = str,
                    Type = GetType(str),
                    StartIndex = currentLexemeStartIndex,
                    EndIndex = currentIndex
                });
            }

            return lexemeList;
        }

        private LexemeType GetType(string value)
        {
            if (value.IndexOf(Statements.StartSymbol, StringComparison.InvariantCultureIgnoreCase) == -1)
                return LexemeType.PlainText;

            if (value.IndexOf(Statements.For, StringComparison.InvariantCultureIgnoreCase) != -1)
                return LexemeType.ForStatement;

            if (value.IndexOf(Statements.If, StringComparison.InvariantCultureIgnoreCase) != -1)
                return LexemeType.IfStatement;
            
            if (value.IndexOf(Statements.Else, StringComparison.InvariantCultureIgnoreCase) != -1)
                return LexemeType.ElseStatement;

            if (value.IndexOf(Statements.End, StringComparison.InvariantCultureIgnoreCase) != -1)
                return LexemeType.EndStatement;

            return LexemeType.ValueStatement;
        }
    }
}