using Fluid;
using System;

namespace MustacheParse;

internal class Program
{
    private static void Main(string[] args)
    {
        var parser = new FluidParser();
        var template = parser.Parse("{% for child in children %} * {{ child }} {% endfor %}");

        var context = new TemplateContext();
        context.SetValue("children", new[] {  "Foo1", "Foo2", "Foo4" });

        var result = template.Render(context);
    }
}