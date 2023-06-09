﻿using System.Collections.Generic;
using MustacheTemplateProcessor.StatementParsers;
using MustacheTemplateProcessor.Tests.Mocks;
using MustacheTemplateProcessor.Tests.Models;
using NUnit.Framework;

namespace MustacheTemplateProcessor.Tests
{
    [TestFixture]
    public class MustacheParserTests
    {
        private MustacheParser GetParser() => new MustacheParser(new EvaluatorMock());

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
            var output = parser.Process(expression, emptyContext);

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
            var output = parser.Process(expression, context);

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
            var output = parser.Process(expression, context);

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
                new ItemModel(),
                new ItemModel()
            };
            var context = new Dictionary<string, object>
            {
                {"Items", items}
            };

            var parser = GetParser();

            // Act
            var output = parser.Process(expression, context);

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
                new ItemModel {Id = 1},
                new ItemModel {Id = 2}
            };
            var context = new Dictionary<string, object>
            {
                {"Items", items}
            };

            var parser = GetParser();

            // Act
            var output = parser.Process(expression, context);

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
                new ItemModel {Id = 1},
                new ItemModel {Id = 2}
            };
            var secondItems = new List<ItemModel>
            {
                new ItemModel {Id = 3},
                new ItemModel {Id = 4}
            };
            var context = new Dictionary<string, object>
            {
                {"FirstItems", firstItems},
                {"SecondItems", secondItems}
            };

            var parser = GetParser();

            // Act
            var output = parser.Process(expression, context);

            // Assert
            Assert.That(output, Is.EqualTo(reference));
        }

        [Test]
        public void ForStatement_InnerFor_Test()
        {
            // Arrange
            var expression = "<head>" +
                             "{{for item in Items}}" +
                             "<div class=\"col-sm-4\">" +
                             "<span>{{item.Id}}</span>" +
                             "{{for innerItem in item.InnerItems}}" +
                             "<span>{{innerItem.Id}}</span>" +
                             "{{end}}" +
                             "</div>" +
                             "{{end}}" +
                             "</head>";

            var reference = "<head>" +
                            "<div class=\"col-sm-4\">" +
                            "<span>1</span>" +
                            "<span>20</span>" +
                            "<span>30</span>" +
                            "</div>" +
                            "</head>";

            var items = new List<InnerItemModel>
            {
                new InnerItemModel
                {
                    Id = 1,
                    InnerItems = new List<ItemModel>
                    {
                        new ItemModel {Id = 20},
                        new ItemModel {Id = 30}
                    }
                }
            };
            var context = new Dictionary<string, object>
            {
                {"Items", items}
            };

            var parser = GetParser();

            // Act
            var output = parser.Process(expression, context);

            // Assert
            Assert.That(output, Is.EqualTo(reference));
        }

        [Test]
        public void IfStatement_Test()
        {
            // Arrange
            var expression = "<head>" +
                             "{{if state == 1}}" +
                             "<span>Hello world</span>" +
                             "{{end}}" +
                             "</head>";

            var reference = "<head>" +
                            "<span>Hello world</span>" +
                            "</head>";

            var context = new Dictionary<string, object>
            {
                {"state", 1}
            };

            var parser = GetParser();

            // Act
            var output = parser.Process(expression, context);

            // Assert
            Assert.That(output, Is.EqualTo(reference));
        }

        [TestCase(1, "<head><span>State = 1</span></head>")]
        [TestCase(2, "<head><span>State <> 1</span></head>")]
        public void IfElseStatement_Test(int value, string reference)
        {
            // Arrange
            var expression = "<head>" +
                             "{{if state == 1}}" +
                             "<span>State = 1</span>" +
                             "{{else}}" +
                             "<span>State <> 1</span>" +
                             "{{end}}" +
                             "</head>";

            var context = new Dictionary<string, object>
            {
                {"state", value}
            };

            var parser = GetParser();

            // Act
            var output = parser.Process(expression, context);

            // Assert
            Assert.That(output, Is.EqualTo(reference));
        }

        [Test]
        public void IfStatement_InnerFor_Test()
        {
            // Arrange
            var expression = "<head>" +
                             "{{if state == 1}}" +
                             "{{for item in Items}}" +
                             "<span>Id = {{item.Id}}</span>" +
                             "{{end}}" +
                             "{{end}}" +
                             "</head>";

            var reference = "<head>" +
                            "<span>Id = 20</span>" +
                            "<span>Id = 30</span>" +
                            "</head>";

            var context = new Dictionary<string, object>
            {
                {"state", 1},
                {
                    "Items", new List<ItemModel>
                    {
                        new ItemModel {Id = 20},
                        new ItemModel {Id = 30}
                    }
                }
            };

            var parser = GetParser();

            // Act
            var output = parser.Process(expression, context);

            // Assert
            Assert.That(output, Is.EqualTo(reference));
        }

        [Test]
        public void IfStatement_SomeIfs_Test()
        {
            // Arrange
            var expression = "<head>" +
                             "{{if state1 == 1}}" +
                             "<span>State 1</span>" +
                             "{{end}}" +
                             "{{if state2 == 2}}" +
                             "<span>State 2</span>" +
                             "{{end}}" +
                             "{{if state3 == 3}}" +
                             "<span>State 3</span>" +
                             "{{end}}" +
                             "</head>";

            var reference = "<head>" +
                            "<span>State 1</span>" +
                            "<span>State 2</span>" +
                            "</head>";

            var context = new Dictionary<string, object>
            {
                {"state1", 1},
                {"state2", 2},
                {"state3", 0}
            };

            var parser = GetParser();

            // Act
            var output = parser.Process(expression, context);

            // Assert
            Assert.That(output, Is.EqualTo(reference));
        }

        [Test]
        public void IfStatement_InnerIf_Test()
        {
            // Arrange
            var expression = "<head>" +
                             "{{if state1 == 1}}" +
                             "{{if state2 == 2}}" +
                             "<span>Hello world!</span>" +
                             "{{end}}" +
                             "{{end}}" +
                             "</head>";

            var reference = "<head>" +
                            "<span>Hello world!</span>" +
                            "</head>";

            var context = new Dictionary<string, object>
            {
                {"state1", 1},
                {"state2", 2}
            };

            var parser = GetParser();

            // Act
            var output = parser.Process(expression, context);

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
                new ItemModel {Id = 42}
            };
            var secondItems = new List<InnerItemModel>
            {
                new InnerItemModel
                {
                    Id = 142,
                    InnerItems = new List<ItemModel>
                    {
                        new ItemModel {Id = 55},
                        new ItemModel {Id = 66}
                    }
                }
            };
            var context = new Dictionary<string, object>
            {
                {"FirstItems", firstItems},
                {"SecondItems", secondItems}
            };

            var parser = GetParser();
            var output = parser.Process(expression, context);

            Assert.That(output, Is.EqualTo(reference));
        }
    }
}