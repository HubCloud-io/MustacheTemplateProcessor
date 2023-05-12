using EvalEngine.Engine;
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

        var expression = statementContext.StartStatement.PureStatement!.Trim();

        try
        {
            var evaluator = new FormulaEvaluator(new Dictionary<string, object>(context));
            var result = evaluator.Eval(expression);
            return result?.ToString();
        }
        catch (Exception)
        {
            return null;
        }
    }
}