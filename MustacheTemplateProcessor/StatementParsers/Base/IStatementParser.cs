using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor.StatementParsers.Base
{
    public interface IStatementParser
    {
        string Process(StatementContext statementContext);
    }
}