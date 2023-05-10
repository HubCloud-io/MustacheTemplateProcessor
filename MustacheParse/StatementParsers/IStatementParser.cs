using MustacheParse.Models;

namespace MustacheParse.StatementParsers;

public interface IStatementParser
{
    string? Process(StatementContext statementContext);
}