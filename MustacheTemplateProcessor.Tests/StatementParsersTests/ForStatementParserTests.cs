using MustacheTemplateProcessor.Models;
using MustacheTemplateProcessor.StatementParsers;
using MustacheTemplateProcessor.Tests.Models;

namespace MustacheTemplateProcessor.Tests.StatementParsersTests;

public class ForStatementParserTests
{
    [Test]
    public void ForParser_Test()
    {
        var statementContext = new StatementContext
        {
            Body = "<div class=\"inner\">" +
                   "<span>Hello world</span>" +
                   "</div>",
            StartStatement = new ParsedStatement
            {
                Statement = @"{{for item in FirstItems}}"
            },
            EndStatement = new ParsedStatement
            {
                Statement = @"{{end}}"
            },
            Context = new ContextModel
            {
                FirstItems = new List<ItemModel>
                {
                    new() {Id = 1},
                    new() {Id = 2}
                }
            }
        };

        var reference = "<div class=\"inner\">" +
                        "<span>Hello world</span>" +
                        "</div>" +
                        "<div class=\"inner\">" +
                        "<span>Hello world</span>" +
                        "</div>";
        
        var parser = new ForStatementParser();
        var result = parser.Process(statementContext);
        
        Assert.That(result, Is.EqualTo(reference));
    }

    [Test]
    public void ForParser_WithInnerValue_Test()
    {
        var statementContext = new StatementContext
        {
            Body = "<div class=\"inner\">" +
                   "<span>{{item.Id}}</span>" +
                   "</div>",
            StartStatement = new ParsedStatement
            {
                Statement = @"{{for item in FirstItems}}"
            },
            EndStatement = new ParsedStatement
            {
                Statement = @"{{end}}"
            },
            Context = new ContextModel
            {
                FirstItems = new List<ItemModel>
                {
                    new() {Id = 1},
                    new() {Id = 2}
                }
            }
        };
        
        var reference = "<div class=\"inner\">" +
                        "<span>1</span>" +
                        "</div>" +
                        "<div class=\"inner\">" +
                        "<span>2</span>" +
                        "</div>";
        
        var parser = new ForStatementParser();
        var result = parser.Process(statementContext);
        
        Assert.That(result, Is.EqualTo(reference));
    }
}