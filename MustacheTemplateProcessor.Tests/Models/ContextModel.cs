namespace MustacheTemplateProcessor.Tests.Models;

public class ComplexValue
{
    public int InnerValue { get; set; }
}
public class ContextModel
{
    public IEnumerable<ItemModel>? FirstItems { get; set; }
    public IEnumerable<ItemModel>? SecondItems { get; set; }
    public IEnumerable<ItemModel>? InnerItems { get; set; }
    public int SimpleValue { get; set; }
    public ComplexValue? ComplexValue { get; set; }
}