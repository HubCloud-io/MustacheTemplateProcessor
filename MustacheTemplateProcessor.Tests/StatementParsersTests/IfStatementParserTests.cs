using MustacheTemplateProcessor.StatementParsers;
using NUnit.Framework;

namespace MustacheTemplateProcessor.Tests.StatementParsersTests
{
    [TestFixture]
    public class IfStatementParserTests
    {
        [Test]
        public void SimpleElse_Test()
        {
            // Arrange
            var expression = "<span>State = 1</span>" +
                             "{{else}}" +
                             "<span>State <> 1</span>";

            var parser = new IfStatementParser();

            // Act
            var output = parser.GetBodies(expression);

            // Assert
            Assert.That(output.TrueStateBody, Is.EqualTo("<span>State = 1</span>"));
            Assert.That(output.FalseStateBody, Is.EqualTo("<span>State <> 1</span>"));
        }
        
        [Test]
        public void InnerIfElse_Test()
        {
            // Arrange
            var expression = "<span>State = 1</span>" +
                             "{{if item = 10}}" +
                             "10" +
                             "{{else}}" +
                             "0" +
                             "{{end}}" +
                             "{{else}}" +
                             "<span>State <> 1</span>";

            var parser = new IfStatementParser();

            // Act
            var output = parser.GetBodies(expression);

            // Assert
            Assert.That(output.TrueStateBody, Is.EqualTo("<span>State = 1</span>{{if item = 10}}10{{else}}0{{end}}"));
            Assert.That(output.FalseStateBody, Is.EqualTo("<span>State <> 1</span>"));
        }
    }
}