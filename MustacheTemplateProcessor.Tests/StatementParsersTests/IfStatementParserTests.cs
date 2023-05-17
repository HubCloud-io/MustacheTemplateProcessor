using System.Reflection;
using MustacheTemplateProcessor.StatementParsers;
using NUnit.Framework;

namespace MustacheTemplateProcessor.Tests.StatementParsersTests
{
    [TestFixture]
    public class IfStatementParserTests
    {
        private IfStatementBodies GetBodies(string expression)
        {
            var parser = new IfStatementParser();
            var methodInfo = typeof(IfStatementParser).GetMethod("GetBodies", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = {expression};
            
            return methodInfo?.Invoke(parser, parameters) as IfStatementBodies;
        }
        
        [Test]
        public void SimpleElse_Test()
        {
            // Arrange
            var expression = "<span>State = 1</span>" +
                             "{{else}}" +
                             "<span>State <> 1</span>";
            // Act
            var output = GetBodies(expression);

            // Assert
            Assert.That(output?.TrueStateBody, Is.EqualTo("<span>State = 1</span>"));
            Assert.That(output?.FalseStateBody, Is.EqualTo("<span>State <> 1</span>"));
        }
        
        [Test]
        public void InnerIf_Test()
        {
            // Arrange
            var expression = "<span>State = 1</span>" +
                             "{{if item = 10}}" +
                             "10" +
                             "{{end}}" +
                             "{{else}}" +
                             "<span>State <> 1</span>";
        
            // Act
            var output = GetBodies(expression);
        
            // Assert
            Assert.That(output.TrueStateBody, Is.EqualTo("<span>State = 1</span>{{if item = 10}}10{{end}}"));
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
        
            // Act
            var output = GetBodies(expression);
        
            // Assert
            Assert.That(output.TrueStateBody, Is.EqualTo("<span>State = 1</span>{{if item = 10}}10{{else}}0{{end}}"));
            Assert.That(output.FalseStateBody, Is.EqualTo("<span>State <> 1</span>"));
        }
        
        [Test]
        public void InnerFor_Test()
        {
            // Arrange
            var expression = "<span>State = 1</span>" +
                             "{{for item in items}}" +
                             "foo" +
                             "{{end}}" +
                             "{{else}}" +
                             "<span>State <> 1</span>";
        
            // Act
            var output = GetBodies(expression);
        
            // Assert
            Assert.That(output.TrueStateBody, Is.EqualTo("<span>State = 1</span>{{for item in items}}foo{{end}}"));
            Assert.That(output.FalseStateBody, Is.EqualTo("<span>State <> 1</span>"));
        }
        
        [Test]
        public void InnerIfElse_2_Test()
        {
            // Arrange
            var expression = "<span>State = 1</span>" +
                             "{{else}}" +
                             "{{if item = 10}}" +
                             "10" +
                             "{{else}}" +
                             "0" +
                             "{{end}}" +
                             "<span>State <> 1</span>";
        
            // Act
            var output = GetBodies(expression);
        
            // Assert
            Assert.That(output.TrueStateBody, Is.EqualTo("<span>State = 1</span>"));
            Assert.That(output.FalseStateBody, Is.EqualTo("{{if item = 10}}10{{else}}0{{end}}<span>State <> 1</span>"));
        }
    }
}