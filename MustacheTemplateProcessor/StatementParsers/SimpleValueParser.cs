using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using MustacheTemplateProcessor.Abstractions;
using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MustacheTemplateProcessor.StatementParsers
{
    public class SimpleValueParser : BaseStatementParser, IStatementParser
    {
        public SimpleValueParser(IEvaluator evaluator) : base(evaluator)
        {
        }

        public string Process(StatementContext statementContext)
        {
            if (!IsValidStatementContext(statementContext))
                return string.Empty;

            if (!IsValidStartStatement(statementContext))
                return statementContext.StartStatement.Statement;

            var expression = statementContext.StartStatement.PureStatement.Trim();

            try
            {
                var result = Evaluator.Eval(expression, new Dictionary<string, object>(statementContext.Context));

                var resultStr = result?.ToString();
                if (result is ExpandoObject || result is DataTable || result is List<object>)
                    resultStr = JsonConvert.SerializeObject(result, Formatting.None,
                        new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
                else
                    resultStr = result?.ToString();

                return resultStr;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}