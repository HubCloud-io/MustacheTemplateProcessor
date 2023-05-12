using MustacheTemplateProcessor.Tests.Models;

namespace MustacheTemplateProcessor.Tests;

public class MustacheDictContextParserTests
{
    private MustacheDictContextParser GetParser() => new();

    [Test]
    public void PlainText_Test()
    {
        // Arrange
        var expression = "<head>" +
                         "<div class=\"row\">" +
                         "<div class=\"col-sm-4\">" +
                         "<span>Hello world!</span>" +
                         "</div>" +
                         "</div>" +
                         "</head>";

        var emptyContext = new Dictionary<string, object>();
        var parser = GetParser();

        // Act
        var output = parser.Parse(expression, emptyContext);

        // Assert
        Assert.That(output, Is.EqualTo(expression));
    }

    [Test]
    public void SimpleValueStatement_Test()
    {
        // Arrange
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

        var context = new Dictionary<string, object>
        {
            {"SimpleValue", "42"}
        };

        var parser = GetParser();

        // Act
        var output = parser.Parse(expression, context);

        // Assert
        Assert.That(output, Is.EqualTo(reference));
    }

    [Test]
    public void SimpleValueStatement_2_Test()
    {
        // Arrange
        var expression = "<head>" +
                         "<div class=\"col-sm-4\">" +
                         "<span>{{SimpleValue.Id}}</span>" +
                         "</div>" +
                         "</head>";

        var reference = "<head>" +
                        "<div class=\"col-sm-4\">" +
                        "<span>42</span>" +
                        "</div>" +
                        "</head>";

        var context = new Dictionary<string, object>
        {
            {"SimpleValue", new ItemModel {Id = 42}}
        };

        var parser = GetParser();

        // Act
        var output = parser.Parse(expression, context);

        // Assert
        Assert.That(output, Is.EqualTo(reference));
    }

    [Test]
    public void SimpleForStatement_Test()
    {
        // Arrange
        var expression = "<head>" +
                         "{{for item in Items}}" +
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

        var items = new List<ItemModel>
        {
            new(),
            new()
        };
        var context = new Dictionary<string, object>
        {
            {"Items", items}
        };

        var parser = GetParser();

        // Act
        var output = parser.Parse(expression, context);

        // Assert
        Assert.That(output, Is.EqualTo(reference));
    }

    [Test]
    public void ForStatement_WithValue_Test()
    {
        // Arrange
        var expression = "<head>" +
                         "{{for item in Items}}" +
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

        var items = new List<ItemModel>
        {
            new() {Id = 1},
            new() {Id = 2}
        };
        var context = new Dictionary<string, object>
        {
            {"Items", items}
        };

        var parser = GetParser();

        // Act
        var output = parser.Parse(expression, context);

        // Assert
        Assert.That(output, Is.EqualTo(reference));
    }

    [Test]
    public void ForStatement_WithValue_2_Test()
    {
        // Arrange
        var expression = "<head>" +
                         "<div>" +
                         "{{for item in FirstItems}}" +
                         "<div>" +
                         "<span>First items</span>" +
                         "<span>{{item.Id}}</span>" +
                         "</div>" +
                         "{{end}}" +
                         "</div>" +
                         "---" +
                         "<div>" +
                         "{{for item in SecondItems}}" +
                         "<div>" +
                         "<span>Second items</span>" +
                         "<span>{{item.Id}}</span>" +
                         "</div>" +
                         "{{end}}" +
                         "</div>" +
                         "</head>";

        var reference = "<head>" +
                        "<div>" +
                        "<div>" +
                        "<span>First items</span>" +
                        "<span>1</span>" +
                        "</div>" +
                        "<div>" +
                        "<span>First items</span>" +
                        "<span>2</span>" +
                        "</div>" +
                        "</div>" +
                        "---" +
                        "<div>" +
                        "<div>" +
                        "<span>Second items</span>" +
                        "<span>3</span>" +
                        "</div>" +
                        "<div>" +
                        "<span>Second items</span>" +
                        "<span>4</span>" +
                        "</div>" +
                        "</div>" +
                        "</head>";

        var firstItems = new List<ItemModel>
        {
            new() {Id = 1},
            new() {Id = 2}
        };
        var secondItems = new List<ItemModel>
        {
            new() {Id = 3},
            new() {Id = 4}
        };
        var context = new Dictionary<string, object>
        {
            {"FirstItems", firstItems},
            {"SecondItems", secondItems}
        };

        var parser = GetParser();

        // Act
        var output = parser.Parse(expression, context);

        // Assert
        Assert.That(output, Is.EqualTo(reference));
    }

    [Test]
    public void ComplexStatements_Test()
    {
        var expression = "<head>" +
                         "<div>" +
                         "{{for item in FirstItems}}" +
                         "<div>" +
                         "<span>First items</span>" +
                         "<span>{{item.Id}}</span>" +
                         "</div>" +
                         "{{end}}" +
                         "</div>" +
                         "---" +
                         "<div>" +
                         "{{for item in SecondItems}}" +
                         "<div>" +
                         "<span>Second items</span>" +
                         "<span>{{item.Id}}</span>" +
                         "{{for innerItem in item.InnerItems}}" +
                         "<span>{{innerItem.Id}}</span>" +
                         "{{end}}" +
                         "</div>" +
                         "{{end}}" +
                         "</div>" +
                         "</head>";

        var reference = "<head>" +
                        "<div>" +
                        "<div>" +
                        "<span>First items</span>" +
                        "<span>42</span>" +
                        "</div>" +
                        "</div>" +
                        "---" +
                        "<div>" +
                        "<div>" +
                        "<span>Second items</span>" +
                        "<span>142</span>" +
                        "<span>55</span>" +
                        "<span>66</span>" +
                        "</div>" +
                        "</div>" +
                        "</head>";
        
        var firstItems = new List<ItemModel>
        {
            new() { Id = 42 }
        };
        var secondItems = new List<InnerItemModel>
        {
            new() 
            { 
                Id = 142,
                InnerItems = new List<ItemModel>
                {
                    new () { Id = 55 },
                    new () { Id = 66 }
                }
            }
        };
        var context = new Dictionary<string, object>
        {
            {"FirstItems", firstItems},
            {"SecondItems", secondItems}
        };

        var parser = GetParser();
        var output = parser.Parse(expression, context);
    
        Assert.That(output, Is.EqualTo(reference));
    }
}