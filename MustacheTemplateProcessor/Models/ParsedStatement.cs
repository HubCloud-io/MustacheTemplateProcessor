using MustacheTemplateProcessor.Common;

namespace MustacheTemplateProcessor.Models
{
    public class ParsedStatement
    {
        public StatementType Type { get; set; }
        public string Statement { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public string PureStatement => Statement
            ?.Replace(Statements.StartSymbol, "")
            .Replace(Statements.EndSymbol, "")
            .Trim();

        public override string ToString()
            => $"Statement = {Statement}, StartIndex = {StartIndex}, EndIndex = {EndIndex}";
    }
}