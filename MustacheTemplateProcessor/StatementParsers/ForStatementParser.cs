using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using EvalEngine.FunctionLibrary;
using MustacheTemplateProcessor.Common;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers.Base;

namespace MustacheTemplateProcessor.StatementParsers
{
    public class ForStatementParser : BaseStatementParser, IStatementParser
    {
        private readonly MustacheParser _parser = new MustacheParser();

        public string Process(StatementContext statementContext)
        {
            if (!IsValidStatementContext(statementContext) || string.IsNullOrEmpty(statementContext.Body))
                return string.Empty;

            if (!IsValidStartStatement(statementContext))
                return statementContext.StartStatement.Statement;

            var context = statementContext.Context;
            var collectionName = GetCollectionName(statementContext.StartStatement);
            if (collectionName is null)
                return string.Empty;

            var itemName = GetItemName(statementContext.StartStatement);
            if (itemName is null)
                return string.Empty;

            IEnumerable<dynamic> items;
            if (collectionName.Contains("."))
            {
                var statementArray = collectionName.Split('.');
                var localContext = context[statementArray.First()];

                for (var i = 1; i < statementArray.Length; i++)
                {
                    var item = statementArray[i];
                    localContext = localContext?.GetType()
                        .GetProperty(item)
                        ?.GetValue(localContext, null);
                }

                items = GetAsDynamic(localContext);
            }
            else
            {
                if (!context.TryGetValue(collectionName, out var collection))
                    return string.Empty;

                items = GetAsDynamic(collection);
            }

            var output = string.Empty;

            if (items is null)
                return output;

            var innerContext = new Dictionary<string, object>(context);
            foreach (var item in items)
            {
                innerContext[itemName] = item;
                try
                {
                    var val = _parser.Process(statementContext.Body, innerContext);
                    output += val;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return output;
        }

        private string GetItemName(ParsedStatement statement)
        {
            if (string.IsNullOrEmpty(statement.PureStatement))
                return null;

            var statementArray = statement.PureStatement.Split();
            if (statementArray.Length != 4)
                return null;

            return statementArray[1];
        }

        private string GetCollectionName(ParsedStatement statement)
        {
            if (string.IsNullOrEmpty(statement.PureStatement))
                return null;

            var statementArray = statement.PureStatement.Split();
            if (statementArray.Length != 4)
                return null;

            return statementArray.Last();
        }
        
        private IEnumerable<dynamic> GetAsDynamic(object collection)
        {
            IEnumerable<dynamic> items;
            
            if (collection is DataTable dataTable)
            {
                items = ToDynamic(dataTable);
            }
            else if (collection is IEnumerable<dynamic>) // for enumerable with class items
            {
                items = collection as IEnumerable<dynamic>;
            }
            else // for enumerable with primitive types items (int, decimal, etc)
            {
                if (!(collection is ICollection cl))
                    return Enumerable.Empty<dynamic>();

                var tmp = new List<dynamic>();
                foreach (var item in cl)
                {
                    tmp.Add(item);
                }

                items = tmp.Select(x => x);
            }

            return items;
        }
        
        public IEnumerable<dynamic> ToDynamic(DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return dynamicDt;
        }
    }
}