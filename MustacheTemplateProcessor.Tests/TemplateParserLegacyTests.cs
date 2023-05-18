using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using MustacheTemplateProcessor.StatementParsers;
using MustacheTemplateProcessor.Tests.Models;
using NUnit.Framework;

namespace MustacheTemplateProcessor.Tests
{
    [TestFixture]
    public class TemplateParserLegacyTests
    {
        private MustacheParser GetParser() => new MustacheParser();

        [Test]
        public void PrepareTemplate_ObjectContext_ReturnString()
        {
            var template = "Product id: {{item.id}}, title: {{item.title}}";
            var context = new Dictionary<string, object>();
            var expando = new ExpandoObject();

            var expandoDir = (IDictionary<string, object>) expando;
            expandoDir.Add("id", 1);
            expandoDir.Add("title", "Brick");

            context.Add("item", expando);

            var parser = GetParser();
            var resultString = parser.Process(template, context);

            var ethalon = "Product id: 1, title: Brick";
            Assert.That(resultString, Is.EqualTo(ethalon));
        }

        [Test]
        public void PrepareTemplate_TwoLines_ReturnString()
        {
            var template = @"Product id: {{item.id}}, 
title: {{item.title}}";
            var context = new Dictionary<string, object>();
            var expando = new ExpandoObject();

            var expandoDir = (IDictionary<string, object>) expando;
            expandoDir.Add("id", 1);
            expandoDir.Add("title", "Brick");

            context.Add("item", expando);

            var parser = GetParser();
            var resultString = parser.Process(template, context);

            var ethalon = @"Product id: 1, 
title: Brick";
            Assert.That(resultString, Is.EqualTo(ethalon));
        }

        [Test]
        public void PrepareTemplate_LoopOverList_ReturnString()
        {
            var template = @"Products list:
=====
{{ for item in products }}
Product id: {{item.Id}}, title: {{item.Title}}
{{end}}
=====";
            var context = new Dictionary<string, object>();
            var productList = new List<Product>();
            productList.Add(new Product {Id = 1, Title = "Product 1"});
            productList.Add(new Product {Id = 2, Title = "Product 2"});
            context.Add("products", productList);

            var parser = GetParser();
            var resultString = parser.Process(template, context);

            var ethalon = @"Products list:
=====
Product id: 1, title: Product 1
Product id: 2, title: Product 2
=====";

            Assert.That(resultString, Is.EqualTo(ethalon));
        }


        [TestCase(1, "Output:\r\n=====\r\nRender for one item\r\n=====")]
        [TestCase(2, "Output:\r\n=====\r\nRender collection\r\n=====")]
        public void PrepareTemplate_TemplateWithIf_ReturnString(int itemsCount, string ethalon)
        {
            var template = @"Output:
=====
{{if items_count = 1 }}
Render for one item
{{end}}
{{if items_count > 1 }}
Render collection
{{end}}
=====";
            var context = new Dictionary<string, object>();
            context.Add("items_count", itemsCount);

            var parser = GetParser();
            var resultString = parser.Process(template, context);

            Console.WriteLine(resultString);

            Assert.That(resultString, Is.EqualTo(ethalon));
        }


        [Test]
        public void PrepareTemplate_IfWithLoop_ReturnString()
        {
            var template = @"Output:
=====
{{if items_count = 1 }}
Render for one item
{{end}}
{{if items_count > 1 }}
{{ for item in products }}
Product id: {{item.Id}}, title: {{item.Title}}
{{end}}
{{end}}
=====";
            var context = new Dictionary<string, object>();

            var productList = new List<Product>();
            productList.Add(new Product {Id = 1, Title = "Product 1"});
            productList.Add(new Product {Id = 2, Title = "Product 2"});

            context.Add("products", productList);
            context.Add("items_count", productList.Count);

            var parser = GetParser();
            var resultString = parser.Process(template, context);

            var ethalon = @"Output:
=====
Product id: 1, title: Product 1
Product id: 2, title: Product 2
=====";

            Assert.That(resultString, Is.EqualTo(ethalon));
        }

        [Test]
        public void PrepareTemplate_InnerLoopWithCondition_ReturnContent()
        {
            var template = "<h3>Store info</h3>" +
                           "{{for store_row in header}}" +
                           "<h4 class=\"text-info\">{{store_row.store_title}}</h4>" +
                           "{{for row in rows}}" +
                           "{{if row.store = store_row.store}}" +
                           "<div><span>{{row.product_title}}</span> <span class=\"badge badge-pill badge-primary\">{{row.quantity}}</span></div>" +
                           "{{end}}" +
                           "{{end}}" +
                           "<hr/>" +
                           "{{end}}";

            // Init header table.
            var header = new DataTable();
            header.Columns.Add("store", typeof(int));
            header.Columns.Add("store_title", typeof(string));

            var headerRow1 = header.NewRow();
            headerRow1["store"] = 1;
            headerRow1["store_title"] = "Store 1";
            header.Rows.Add(headerRow1);

            var headerRow2 = header.NewRow();
            headerRow2["store"] = 2;
            headerRow2["store_title"] = "Store 2";
            header.Rows.Add(headerRow2);

            // Init rows table.
            var rows = new DataTable();
            rows.Columns.Add("store", typeof(int));
            rows.Columns.Add("product", typeof(int));
            rows.Columns.Add("product_title", typeof(string));
            rows.Columns.Add("quantity", typeof(decimal));

            var row1 = rows.NewRow();
            row1["store"] = 1;
            row1["product"] = 1;
            row1["product_title"] = "Product 1";
            row1["quantity"] = 100M;
            rows.Rows.Add(row1);

            var row2 = rows.NewRow();
            row2["store"] = 2;
            row2["product"] = 2;
            row2["product_title"] = "Product 2";
            row2["quantity"] = 200M;
            rows.Rows.Add(row2);

            var context = new Dictionary<string, object>();
            context.Add("header", header);
            context.Add("rows", rows);

            var parser = GetParser();
            var resultString = parser.Process(template, context);

            Console.WriteLine(resultString);

            Assert.Pass();
        }

        
        [TestCase("{{if items_count > 1 }}Test1{{end}}", "Test1")]
        [TestCase("{{ if items_count > 1 }}Test2{{end}}", "Test2")]
        [TestCase("{{ if if_condition = true }}Test3{{end}}", "Test3")]
        public void VariantsOfIfExpression(string template, string ethalon)
        {
            var context = new Dictionary<string, object>
            {
                {"items_count", 2},
                {"if_condition", true}
            };
            
            var parser = GetParser();
            var result = parser.Process(template, context);
        
            Assert.That(result, Is.EqualTo(ethalon));
        }
        
        [TestCase("{{for item in items1}}Test1{{end}}", "Test1Test1")]
        [TestCase("{{ for item in items1 }}Test1{{ end}}", "Test1Test1")]
        [TestCase("{{ for if_item in for_items1 }}Test1{{ end}}", "Test1Test1")]
        public void VariantsOfForExpression(string template, string ethalon)
        {
            var context = new Dictionary<string, object>
            {
                {"items1", new [] {1, 2}},
                {"for_items1", new [] {1, 2}},
            };
            
            var parser = GetParser();
            var result = parser.Process(template, context);
        
            Assert.That(result, Is.EqualTo(ethalon));
        }
        
        [TestCase(1, "Output:\r\n=====\r\nRender for one item\r\n=====")]
        [TestCase(2, "Output:\r\n=====\r\nRender collection\r\n=====")]
        public void PrepareTemplate_TemplateWithIfElse_ReturnString(int itemsCount, string ethalon)
        {
            var template = @"Output:
=====
{{if items_count = 1 }}
Render for one item
{{else}}
Render collection
{{end}}
=====";
            var context = new Dictionary<string, object>();
            context.Add("items_count", itemsCount);

            var parser = GetParser();
            var resultString = parser.Process(template, context);

            Assert.That(resultString, Is.EqualTo(ethalon));
        }

        #region DataTable test

//         [Test]
//         public void PrepareTemplate_LoopOverDataTable_ReturnString()
//         {
//             var template = @"Products list:
// =====
// {{ for item in products }}
// Product id: {{item.Id}}, title: {{item.Title}}
// {{end}}
// =====";
//             var context = new Dictionary<string, object>();
//
//             var dataTable = new DataTable();
//             dataTable.Columns.Add("Id", typeof(int));
//             dataTable.Columns.Add("Title", typeof(string));
//
//             var row1 = dataTable.NewRow();
//             row1["Id"] = 1;
//             row1["Title"] = "Product 1";
//
//             dataTable.Rows.Add(row1);
//
//             var row2 = dataTable.NewRow();
//             row2["Id"] = 2;
//             row2["Title"] = "Product 2";
//
//             dataTable.Rows.Add(row2);
//
//             context.Add("products", dataTable);
//
//             var parser = GetParser();
//             var resultString = parser.Process(template, context);
//
//             var ethalon = @"Products list:
// =====
// Product id: 1, title: Product 1
// Product id: 2, title: Product 2
// =====";
//
//             Assert.That(resultString, Is.EqualTo(ethalon));
//         }

        #endregion
    }
}