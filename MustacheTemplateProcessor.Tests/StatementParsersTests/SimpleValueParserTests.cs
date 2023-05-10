using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers;
using MustacheTemplateProcessor.Tests.Models;

namespace MustacheTemplateProcessor.Tests.StatementParsersTests;

public class SimpleValueParserTests
{
    [Test]
    public void SimpleValueParser_Test()
    {
        var statementContext = new StatementContext
        {
            StartStatement = new ParsedStatement
            {
                Statement = @"{{item.Id}}"
            },
            Context = new ItemModel
            {
                Id = 42
            }
        };

        var reference = "42";
        
        var parser = new SimpleValueParser();
        var result = parser.Process(statementContext);
        
        Assert.That(result, Is.EqualTo(reference));
    }

    [Test]
    public void SimpleValueParser_NoStatement_Test()
    {
        var statementContext = new StatementContext
        {
            StartStatement = new ParsedStatement
            {
                Statement = @"Hello world!"
            },
            Context = new ItemModel
            {
                Id = 42
            }
        };

        var reference = "Hello world!";
        
        var parser = new SimpleValueParser();
        var result = parser.Process(statementContext);
        
        Assert.That(result, Is.EqualTo(reference));
    }

    [Test]
    public void SimpleValueParser_WrongContextPropertyName_Test()
    {
        var statementContext = new StatementContext
        {
            StartStatement = new ParsedStatement
            {
                Statement = @"{{item.Uid}}"
            },
            Context = new ItemModel
            {
                Id = 42
            }
        };
        
        var reference = string.Empty;
        
        var parser = new SimpleValueParser();
        var result = parser.Process(statementContext);
        
        Assert.That(result, Is.EqualTo(reference));
    }
}