using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor.StatementParsers;

public class SimpleValueParser : IStatementParser
{
    public string? Process(StatementContext statementContext)
    {
        var context = statementContext.Context;
        if (context is null || string.IsNullOrEmpty(statementContext.StartStatement?.Statement))
            return string.Empty;

        if (statementContext.StartStatement.Statement.IndexOf("{{", StringComparison.InvariantCulture) == -1 ||
            statementContext.StartStatement.Statement.IndexOf("}}", StringComparison.InvariantCulture) == -1)
            return statementContext.StartStatement.Statement;

        try
        {
            var statementArray = statementContext.StartStatement.PureStatement!.Split('.');
            for (var i = 0; i < statementArray.Length - 1; i++)
            {
                var item = statementArray[i];
                context = context.GetType()
                    .GetProperty(item)
                    .GetValue(context, null);
            }
            
            var statementValue = context.GetType()
                .GetProperty(statementArray.Last())
                .GetValue(context, null);

            return statementValue?.ToString();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}