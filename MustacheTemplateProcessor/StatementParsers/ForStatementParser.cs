﻿using System;
using System.Collections.Generic;
using System.Linq;
using MustacheTemplateProcessor.Models;

namespace MustacheTemplateProcessor.StatementParsers
{
    public class ForStatementParser : IStatementParser
    {
        private readonly MustacheParser _parser = new MustacheParser();

        public string Process(StatementContext statementContext)
        {
            var context = statementContext.Context;
            if (context is null || string.IsNullOrEmpty(statementContext.StartStatement?.Statement) ||
                string.IsNullOrEmpty(statementContext.Body))
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

            IEnumerable<dynamic> items;
            if (!collectionName.Contains("."))
            {
                if (!context.TryGetValue(collectionName, out var collection) || !(collection is IEnumerable<dynamic>))
                    return string.Empty;
                else
                    items = collection as IEnumerable<dynamic>;
            }
            else
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

                items = localContext as IEnumerable<dynamic>;
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
                    var val = _parser.Parse(statementContext.Body, innerContext);
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
    }
}