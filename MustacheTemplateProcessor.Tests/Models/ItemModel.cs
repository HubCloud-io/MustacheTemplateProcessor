namespace MustacheTemplateProcessor.Tests.Models;

public class ItemModel
{
    public int Id { get; set; }
}

public class InnerItemModel
{
    public int Id { get; set; }
    public List<ItemModel>? InnerItems { get; set; }
}