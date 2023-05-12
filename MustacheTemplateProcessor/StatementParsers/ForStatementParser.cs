using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor.StatementParsers;

public class ForStatementParser : IStatementParser
{
    private readonly MustacheDictContextParser _parser = new ();
    
    public string? Process(StatementContext statementContext)
    {
        var context = statementContext.Context;
        if (context is null || string.IsNullOrEmpty(statementContext.StartStatement?.Statement) || string.IsNullOrEmpty(statementContext.Body))
            return string.Empty;
        
        if (statementContext.StartStatement.Statement.IndexOf("{{", StringComparison.InvariantCulture) == -1 ||
            statementContext.StartStatement.Statement.IndexOf("}}", StringComparison.InvariantCulture) == -1)
            return statementContext.StartStatement.Statement;

        var collectionName = GetCollectionName(statementContext.StartStatement);
        if (collectionName is null)
            return string.Empty;
        
        var itemName = GetItemName(statementContext.StartStatement);
        if (itemName is null)
            return string.Empty;
        
        var collection = context.GetType()
            .GetProperty(collectionName)
            .GetValue(context, null);
        
        if (!(collection is IEnumerable<dynamic> items))
            throw new DataContextException();

        var output = string.Empty;
        foreach (var item in items)
        {
            output += statementContext.Body;
        }
        
        return output;
    }

    public string? Process(StatementDictContext statementContext)
    {
        var context = statementContext.Context;
        if (context is null || string.IsNullOrEmpty(statementContext.StartStatement?.Statement) || string.IsNullOrEmpty(statementContext.Body))
            return string.Empty;
        
        if (statementContext.StartStatement.Statement.IndexOf("{{", StringComparison.InvariantCulture) == -1 ||
            statementContext.StartStatement.Statement.IndexOf("}}", StringComparison.InvariantCulture) == -1)
            return statementContext.StartStatement.Statement;

        var collectionName = GetCollectionName(statementContext.StartStatement);
        if (collectionName is null)
            return string.Empty;
        
        var itemName = GetItemName(statementContext.StartStatement);
        if (itemName is null)
            return string.Empty;

        if (!context.TryGetValue(collectionName, out var collection) || !(collection is IEnumerable<dynamic> items))
            return string.Empty;
        
        var output = string.Empty;
        var innerContext = new Dictionary<string, object>(context);
        foreach (var item in items)
        {
            innerContext[itemName] = item;
            var val = _parser.Parse(statementContext.Body, innerContext);
            output += val;
        }
        
        return output;
    }

    private string? GetItemName(ParsedStatement statement)
    {
        if (string.IsNullOrEmpty(statement.PureStatement))
            return null;
        
        var statementArray = statement.PureStatement.Split();
        if (statementArray.Length != 4)
            return null;
        
        return statementArray[1];
    }

    private string? GetCollectionName(ParsedStatement statement)
    {
        if (string.IsNullOrEmpty(statement.PureStatement))
            return null;
        
        var statementArray = statement.PureStatement.Split();
        if (statementArray.Length != 4)
            return null;
        
        return statementArray.Last();
    }
}