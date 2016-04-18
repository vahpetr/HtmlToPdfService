using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using HtmlToPdf.Core.Tests.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlToPdf.Core.Tests
{
    [TestClass]
    public class HtmlToPdfServiceTests
    {
        private readonly string appPath;
        private ViewModel vm = new ViewModel
        {
            Title = "Joe",
            Calc = 6,
            Cities = new[] { "Perm", "Moscow" },
            Places = new string[] { },
            Peoples = new[]
            {
                new People { FirstName = "John", LastName = "Lennon" },
                new People { FirstName = "Paul", LastName = "McCartney" }
            },
            Active = true
        };

        public HtmlToPdfServiceTests()
        {
            var codeBase = Assembly.GetAssembly(typeof (HtmlToPdfService)).CodeBase;
            appPath = Path.GetDirectoryName(codeBase).Replace(@"file:\", "");
        }

        [TestMethod]
        public void RenderTest_01()
        {
            var view = HtmlToPdfService.Render("{{Title}} spends {{Calc}}", vm);
            Assert.AreEqual(view, "Joe spends 6");
        }

        [TestMethod]
        public void RenderTest_02()
        {
            var view = HtmlToPdfService.Render("{{#Cities}}{{.}}, {{/Cities}}", vm);
            Assert.AreEqual(view, "Perm, Moscow, ");
        }

        [TestMethod]
        public void RenderTest_03()
        {
            var view = HtmlToPdfService.Render("{{^Places}}No places{{/Places}}", vm);
            Assert.AreEqual(view, "No places");
        }

        [TestMethod]
        public void RenderTest_04()
        {
            var view = HtmlToPdfService.Render("Today{{! ignore me }}.", vm);
            Assert.AreEqual(view, "Today.");
        }

        [TestMethod]
        public void RenderTest_05()
        {
            var view = HtmlToPdfService.Render("{{#Peoples}}{{Name}}, {{/Peoples}}", vm);
            Assert.AreEqual(view, "John Lennon, Paul McCartney, ");
        }

        [TestMethod]
        public void RenderTest_06()
        {
            var view = HtmlToPdfService.Render("{{#Active}}Is Active{{/Active}}", vm);
            Assert.AreEqual(view, "Is Active");
        }

        [TestMethod]
        public void ConvertTest()
        {
            using (var service = new HtmlToPdfService())
            {
                var bytes = service.Convert("{{Title}} spends {{Calc}}", vm);
                File.WriteAllBytes("Convert.pdf", bytes);
                Assert.IsTrue(bytes.Any());
            }
        }

        [TestMethod]
        public void ConvertMultipleTest()
        {
            using (var service = new HtmlToPdfService())
            {
                for (var i = 0; i < 10; i++)
                {
                    var bytes = service.Convert("Hello World " + i);
                    File.WriteAllBytes("ConvertMultiple " + i +".pdf", bytes);
                    Assert.IsTrue(bytes.Any());
                }
            }
        }

        [TestMethod]
        public void Template1ToPdfTest()
        {
            var start = DateTime.Now;
            var path = Path.Combine(appPath, "Fixtures/Template1.html");
            var template = File.ReadAllText(path);

            using (var service = new HtmlToPdfService())
            {
                var bytes = service.Convert(template, vm);
                File.WriteAllBytes("Template1ToPdf.pdf", bytes);
            }

            var end = DateTime.Now - start;
            Trace.WriteLine(string.Format("Minutes:{0}, Seconds:{1}, Milliseconds:{2}, Ticks:{3}", end.Minutes, end.Seconds, end.Milliseconds, end.Ticks));
            Assert.IsTrue(true);
        }

        //count 100 - Minutes:0, Seconds:9, Milliseconds:878, Ticks:98787042
        [TestMethod]
        public void StressTest()
        {
            var start = DateTime.Now;
            var path = Path.Combine(appPath, "Fixtures/Template1.html");
            var template = File.ReadAllText(path);
            using (var service = new HtmlToPdfService())
            {
                for (var i = 0; i < 100; i++)
                {
                    var bytes = service.Convert(template, vm);
                    File.WriteAllBytes("Stress " + i + ".pdf", bytes);
                }
            }

            var end = DateTime.Now - start;
            Trace.WriteLine(string.Format("Minutes:{0}, Seconds:{1}, Milliseconds:{2}, Ticks:{3}", end.Minutes, end.Seconds, end.Milliseconds, end.Ticks));
            Assert.IsTrue(true);
        }
    }
}