using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor.StatementParsers
{
    public interface IStatementParser
    {
        string Process(StatementContext statementContext);
    }
}