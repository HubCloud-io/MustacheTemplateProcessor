using System;
using MustacheTemplateProcessor.Models;
using NUnit.Framework;

namespace MustacheTemplateProcessor.Tests
{
    [TestFixture]
    public class StatementHelperTests
    {
        [Test]
        public void GetEndStatement_Test()
        {
            var expression = "<head>" +
                             "<div class=\"first-for\">" +
                             "{{for item in FirstItems}}" +
                             "  <div class=\"inner\">" +
                             "  {{for item in SecondItems}}" +
                             "      <span>Hello world</span>" +
                             "      <span>{{item.Id}}</span>" +
                             "  {{end}}" +
                             "  </div>" +
                             "{{end}}" +
                             "</div>" +
                             "</head>";

            var startStatement = new ParsedStatement
            {
                Statement = "{{for item in FirstItems}}",
                StartIndex = expression.IndexOf("{{for item in FirstItems}}", StringComparison.InvariantCulture),
                EndIndex = expression.IndexOf("{{for item in FirstItems}}", StringComparison.InvariantCulture) +
                           "{{for item in FirstItems}}".Length
            };

            var helper = new StatementHelper();
            var endStatement = helper.GetEndStatement(expression, startStatement);

            Assert.That(endStatement, Is.Not.Null);
            Assert.That(endStatement.StartIndex, Is.EqualTo(182));
            Assert.That(endStatement.EndIndex, Is.EqualTo(188));
        }
    }
}