using MustacheTemplateProcessor.Common;

namespace MustacheTemplateProcessor.Models
{
    public class ParsedStatement
    {
        public string Statement { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public string PureStatement => Statement
            ?.Trim()
            .Replace(Statements.StartSymbol, "")
            .Replace(Statements.EndSymbol, "");

        public override string ToString()
            => $"Statement = {Statement}, StartIndex = {StartIndex}, EndIndex = {EndIndex}";
    }
}