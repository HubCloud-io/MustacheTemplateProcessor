using MustacheTemplateProcessor.LexemeAnalyzer;

namespace MustacheTemplateProcessor.Tests;

[TestFixture]
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

    [TestCase("{{if flag = true }}", "IfStatement")]
    [TestCase("{{ if flag = true }}", "IfStatement")]
    [TestCase("{{ IF flag = true }}", "IfStatement")]
    [TestCase("{{iffy flag = true }}", "PlainText")]
    [TestCase("iffy flag = true", "PlainText")]
    [TestCase("iffy means", "PlainText")]
    public void GetLexemes_TemplateWithOneLexeme_Lexemes(string expression, string lexemeString)
    {
        
        var analyser = new LexemeAnalyzer.LexemeAnalyzer();
        var lexemes = analyser.GetLexemes(expression).ToArray();

        var lexemeCheck = Enum.Parse<LexemeType>(lexemeString);

        Assert.AreEqual(2, lexemes.Length);
        Assert.AreEqual(lexemeCheck, lexemes[1].Type);

    }
}