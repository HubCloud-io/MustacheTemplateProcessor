using MustacheParse.Models;
using MustacheParse.Tests.Models;

namespace MustacheParse.Tests;

public class MustacheParserTests
{
    [Test]
    public void PlainText_Test()
    {
        var expression = "<head>" +
                         "<div class=\"row\">" +
                         "<div class=\"col-sm-4\">" +
                         "<span>Hello world!</span>" +
                         "</div>" +
                         "</div>" +
                         "</head>";

        var emptyContext = new List<string>();
        var parser = new MustacheParser();
        var output = parser.Parse(expression, emptyContext);

        Assert.That(output, Is.EqualTo(expression));
    }

    [Test]
    public void SimpleForStatement_Test()
    {
        var expression = "<head>" +
                         "{{for item in FirstItems}}" +
                         "<div class=\"col-sm-4\">" +
                         "<span>Hello world!</span>" +
                         "</div>" +
                         "{{end}}" +
                         "</head>";

        var reference = "<head>" +
                        "<div class=\"col-sm-4\">" +
                        "<span>Hello world!</span>" +
                        "</div>" +
                        "<div class=\"col-sm-4\">" +
                        "<span>Hello world!</span>" +
                        "</div>" +
                        "</head>";

        var context = new ContextModel
        {
            FirstItems = new List<ItemModel>
            {
                new(),
                new()
            }
        };
        
        var parser = new MustacheParser();
        var output = parser.Parse(expression, context);

        Assert.That(output, Is.EqualTo(reference));
    }
    
    [Test]
    public void ForStatement_Test()
    {
        var expression = "<head>" +
                         "{{for item in FirstItems}}" +
                         "<div class=\"col-sm-4\">" +
                         "<span>{{item.Id}}</span>" +
                         "</div>" +
                         "{{end}}" +
                         "</head>";

        var reference = "<head>" +
                        "<div class=\"col-sm-4\">" +
                        "<span>1</span>" +
                        "</div>" +
                        "<div class=\"col-sm-4\">" +
                        "<span>2</span>" +
                        "</div>" +
                        "</head>";

        var context = new ContextModel
        {
            FirstItems = new List<ItemModel>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            }
        };
        
        var parser = new MustacheParser();
        var output = parser.Parse(expression, context);

        Assert.That(output, Is.EqualTo(reference));
    }

    [Test]
    public void SimpleValueStatement_Test()
    {
        var expression = "<head>" +
                         "<div class=\"col-sm-4\">" +
                         "<span>{{SimpleValue}}</span>" +
                         "</div>" +
                         "</head>";
        
        var reference = "<head>" +
                        "<div class=\"col-sm-4\">" +
                        "<span>42</span>" +
                        "</div>" +
                        "</head>";
        
        var context = new ContextModel
        {
            SimpleValue = 42
        };
        
        var parser = new MustacheParser();
        var output = parser.Parse(expression, context);
        
        Assert.That(output, Is.EqualTo(reference));
    }
    
    [Test]
    public void SimpleValueStatement_2_Test()
    {
        var expression = "<head>" +
                         "<div class=\"col-sm-4\">" +
                         "<span>{{ComplexValue.InnerValue}}</span>" +
                         "</div>" +
                         "</head>";
        
        var reference = "<head>" +
                        "<div class=\"col-sm-4\">" +
                        "<span>42</span>" +
                        "</div>" +
                        "</head>";
        
        var context = new ContextModel
        {
            ComplexValue = new ComplexValue
            {
                InnerValue = 42
            }
        };
        
        var parser = new MustacheParser();
        var output = parser.Parse(expression, context);
        
        Assert.That(output, Is.EqualTo(reference));
    }

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
            EndIndex = expression.IndexOf("{{for item in FirstItems}}", StringComparison.InvariantCulture) + "{{for item in FirstItems}}".Length
        };
            
        var parser = new MustacheParser();
        var endStatement = parser.GetEndStatement(expression, startStatement);
        
        Assert.That(endStatement, Is.Not.Null);
        Assert.That(endStatement!.StartIndex, Is.EqualTo(182));
        Assert.That(endStatement.EndIndex, Is.EqualTo(188));
    }

    [Test]
    public void ComplexStatements_Test()
    {
        var expression = "<head>" +
                         "<div class=\"first-for\">" +
                         "{{for item in FirstItems}}" +
                         "<div class=\"inner\">" +
                         "<span>Hello world</span>" +
                         "<span>{{item.Id}}</span>" +
                         "</div>" +
                         "{{end}}" +
                         "</div>" +
                         "<div class=\"second-for\">" +
                         "{{for item in SecondItems}}" +
                         "<div class=\"inner\">" +
                         "<span>Foo bar</span>" +
                         "<span>{{item.Id}}</span>" +
                         "{{for item in InnerItems}}" +
                         "<span>Inner</span>" +
                         "{{end}}" +
                         "</div>" +
                         "{{end}}" +
                         "</div>" +
                         "</head>";

        var reference = "<head>" +
                        "<div class=\"first-for\">" +
                        "<div class=\"inner\">" +
                        "<span>Hello world</span>" +
                        "<span>42</span>" +
                        "</div>" +
                        "</div>" +
                        "<div class=\"second-for\">" +
                        "<div class=\"inner\">" +
                        "<span>Foo bar</span>" +
                        "<span>142</span>" +
                        "<span>Inner</span>" +
                        "<span>Inner</span>" +
                        "<span>Inner</span>" +
                        "</div>" +
                        "</div>" +
                        "</head>";
        
        var context = new ContextModel
        {
            FirstItems = new List<ItemModel>
            {
                new() {Id = 42}
            },
            SecondItems = new List<ItemModel>
            {
                new() {Id = 142}
            },
            InnerItems = new List<ItemModel>
            {
                new(),
                new(),
                new()
            }
        };
        
        var parser = new MustacheParser();
        var output = parser.Parse(expression, context);

        Assert.That(output, Is.EqualTo(reference));
    }
}