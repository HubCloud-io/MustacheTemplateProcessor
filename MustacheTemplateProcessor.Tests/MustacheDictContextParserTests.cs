using MustacheTemplateProcessor.Tests.Models;

namespace MustacheTemplateProcessor.Tests;

public class MustacheDictContextParserTests
{
    private MustacheDictContextParser GetParser() => new ();
    
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
            { "SimpleValue", "42" }
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
            { "SimpleValue", new ItemModel { Id = 42} }
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
            { "Items", items }
        };

        var parser = GetParser();
        
        // Act
        var output = parser.Parse(expression, context);

        // Assert
        Assert.That(output, Is.EqualTo(reference));
    }
}