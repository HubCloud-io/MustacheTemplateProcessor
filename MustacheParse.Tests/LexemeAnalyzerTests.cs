namespace MustacheParse.Tests;

public class LexemeAnalyzerTests
{
    [Test]
    public void LexemeAnalyzer_Test()
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

        var analyzer = new LexemeAnalyzer.LexemeAnalyzer();
        var lexemes = (analyzer.GetLexemes(expression))?.ToArray();

        Assert.That(lexemes, Is.Not.Null);
        Assert.That(lexemes!.Length, Is.EqualTo(17));
        Assert.That(lexemes![1].Value, Is.EqualTo("{{for item in FirstItems}}"));
        Assert.That(lexemes![15].Value, Is.EqualTo("{{end}}"));
    }
}