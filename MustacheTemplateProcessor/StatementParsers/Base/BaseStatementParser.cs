using System;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor.StatementParsers.Base
{
    public class BaseStatementParser
    {
        protected bool IsValidStatementContext(StatementContext statementContext)
        {
            if (statementContext?.Context is null || string.IsNullOrEmpty(statementContext.StartStatement?.Statement))
                return false;
            return true;
        }

        protected bool IsValidStartStatement(StatementContext statementContext)
        {
            if (statementContext.StartStatement.Statement.IndexOf(Statements.StartSymbol, StringComparison.InvariantCultureIgnoreCase) == -1 ||
                statementContext.StartStatement.Statement.IndexOf(Statements.EndSymbol, StringComparison.InvariantCultureIgnoreCase) == -1)
                return false;
            return true;
        }
    }
}