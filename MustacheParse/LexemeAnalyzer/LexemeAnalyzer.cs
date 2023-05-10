﻿namespace MustacheParse.LexemeAnalyzer;

public class LexemeAnalyzer
{
    public IEnumerable<Lexeme> GetLexemes(string expression)
    {
        if (string.IsNullOrEmpty(expression))
            return Enumerable.Empty<Lexeme>();
        
        var lexemeList = new List<Lexeme>();

        var current = string.Empty;
        var isLexeme = false;
        var endBracket = 0;

        var currentIndex = 0;
        var currentLexemeStartIndex = 0;
        foreach (var ch in expression)
        {
            if (ch == '{' && !isLexeme)
            {
                isLexeme = true;
                lexemeList.Add(new Lexeme
                {
                    Value = current,
                    Type = GetType(current),
                    StartIndex = currentLexemeStartIndex,
                    EndIndex = currentIndex - 1
                });

                current = string.Empty;
                currentLexemeStartIndex = currentIndex;
            }

            if (ch == '}' && endBracket < 2)
                endBracket++;

            current += ch;
            
            if (endBracket == 2)
            {
                endBracket = 0;
                isLexeme = false;
                lexemeList.Add(new Lexeme
                {
                    Value = current,
                    Type = GetType(current),
                    StartIndex = currentLexemeStartIndex,
                    EndIndex = currentIndex
                });
                
                current = string.Empty;
                currentLexemeStartIndex = currentIndex + 1;
            }

            currentIndex++;
        }

        if (!string.IsNullOrEmpty(current))
        {
            lexemeList.Add(new Lexeme
            {
                Value = current,
                Type = GetType(current),
                StartIndex = currentLexemeStartIndex,
                EndIndex = currentIndex
            });
        }

        return lexemeList;
    }

    private LexemeType GetType(string value)
    {
        if (value.IndexOf("{{", StringComparison.InvariantCulture) == -1)
            return LexemeType.PlainText;

        if (value.IndexOf("for", StringComparison.InvariantCulture) != -1)
            return LexemeType.ForStatement;
        
        if (value.IndexOf("if", StringComparison.InvariantCulture) != -1)
            return LexemeType.IfStatement;
        
        if (value.IndexOf("end", StringComparison.InvariantCulture) != -1)
            return LexemeType.EndStatement;

        return LexemeType.ValueStatement;
    }
}