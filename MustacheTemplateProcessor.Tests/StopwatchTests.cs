using System;
using System.Collections.Generic;
using System.Diagnostics;
using MustacheTemplateProcessor.Tests.Mocks;
using NUnit.Framework;

namespace MustacheTemplateProcessor.Tests
{
    [TestFixture]
    public class StopwatchTests
    {
        private MustacheParser GetParser() => new MustacheParser(new EvaluatorMock());
        private MustacheParser GetParser2() => new MustacheParser(new EvaluatorMock2());
        
        [Test]
        public void Simple_Test()
        {
            var expression = "<span>Start</span>" +
                             "{{for item in Items}}" +
                             "{{if item = 1}}" +
                             "<span>Item = 1</span>" +
                             "{{else}}" +
                             "<span>Item != 1</span>" +
                             "{{end}}" +
                             "{{end}}" +
                             "<span>End</span>";
            
            var reference = "<span>Start</span>" +
                            "<span>Item = 1</span>" +
                            "<span>Item != 1</span>" +
                            "<span>End</span>";

            var items = new List<int> { 1, 2 };
            
            var context = new Dictionary<string, object>
            {
                {"Items", items}
            };

            var parser = GetParser();

            var sw = new Stopwatch();
            sw.Start();
            var output = parser.Process(expression, context);
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;

            Console.WriteLine($@"parser.Process:: Elapsed: {elapsed}ms");
            Assert.That(output, Is.EqualTo(reference));
        }

        private class Foo
        {
            public int Id { get; set; }
        }
        
        [TestCase(1)]
        [TestCase(10000)]
        [TestCase(20000)]
        [TestCase(40000)]
        public void Hard_Test(int itemsCount)
        {
            var expression = "<span>Start</span>" +
                             "{{for item in Items}}" +
                             "<span>{{item.Id}}</span>" +
                             "{{end}}" +
                             "<span>End</span>";

            
            var items = new List<Foo>();
            var reference = "<span>Start</span>";
            for (var i = 0; i < itemsCount; i++)
            {
                items.Add(new Foo { Id = i });
                reference += $"<span>{i}</span>";
            }
            
            reference += "<span>End</span>";
            
            var context = new Dictionary<string, object>
            {
                {"Items", items}
            };
            
            var parser = GetParser();

            var sw = new Stopwatch();
            sw.Start();
            var output = parser.Process(expression, context);
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;

            Console.WriteLine($@"parser.Process:: Items: {itemsCount}, Elapsed: {elapsed}ms");
            Assert.That(output, Is.EqualTo(reference));
        }

        [TestCase(1)]
        [TestCase(10000)]
        [TestCase(20000)]
        [TestCase(40000)]
        public void Hard_Test2(int itemsCount)
        {
            var expression = "<span>Start</span>" +
                             "{{for item in Items}}" +
                             "<span>{{item.Id}}</span>" +
                             "{{end}}" +
                             "<span>End</span>";


            var items = new List<Foo>();
            var reference = "<span>Start</span>";
            for (var i = 0; i < itemsCount; i++)
            {
                items.Add(new Foo { Id = i });
                reference += $"<span>{i}</span>";
            }

            reference += "<span>End</span>";

            var context = new Dictionary<string, object>
            {
                {"Items", items}
            };

            var parser = GetParser2();

            var sw = new Stopwatch();
            sw.Start();
            var output = parser.Process(expression, context);
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;

            Console.WriteLine($@"parser.Process:: Items: {itemsCount}, Elapsed: {elapsed}ms");
            Assert.That(output, Is.EqualTo(reference));
        }
    }
}