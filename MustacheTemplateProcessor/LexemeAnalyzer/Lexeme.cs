namespace MustacheTemplateProcessor.LexemeAnalyzer
{
    public class Lexeme
    {
        public LexemeType Type { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public override string ToString()
            => $"Type = {Type.ToString()}, Value = {Value}, StartIndex = {StartIndex}, EndIndex = {EndIndex}";
    }
}