using System.Collections.Generic;

namespace MustacheTemplateProcessor.Tests.Models
{
    public class InnerItemModel
    {
        public int Id { get; set; }
        public List<ItemModel> InnerItems { get; set; }
    }
}