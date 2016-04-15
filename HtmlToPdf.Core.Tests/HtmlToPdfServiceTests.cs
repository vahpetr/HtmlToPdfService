using HtmlToPdf.Core.Tests.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlToPdf.Core.Tests
{
    [TestClass]
    public class HtmlToPdfServiceTests
    {
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

        [TestMethod]
        public void RenderTest_01()
        {
            var service = new HtmlToPdfService();
            var view = service.Render("{{Title}} spends {{Calc}}", vm);
            Assert.AreEqual(view, "Joe spends 6");
        }

        [TestMethod]
        public void RenderTest_02()
        {
            var service = new HtmlToPdfService();
            var view = service.Render("{{#Cities}}{{.}}, {{/Cities}}", vm);
            Assert.AreEqual(view, "Perm, Moscow, ");
        }

        [TestMethod]
        public void RenderTest_03()
        {
            var service = new HtmlToPdfService();
            var view = service.Render("{{^Places}}No places{{/Places}}", vm);
            Assert.AreEqual(view, "No places");
        }

        [TestMethod]
        public void RenderTest_04()
        {
            var service = new HtmlToPdfService();
            var view = service.Render("Today{{! ignore me }}.", vm);
            Assert.AreEqual(view, "Today.");
        }

        [TestMethod]
        public void RenderTest_05()
        {
            var service = new HtmlToPdfService();
            var view = service.Render("{{#Peoples}}{{Name}}, {{/Peoples}}", vm);
            Assert.AreEqual(view, "John Lennon, Paul McCartney, ");
        }

        [TestMethod]
        public void RenderTest_06()
        {
            var service = new HtmlToPdfService();
            var view = service.Render("{{#Active}}Is Active{{/Active}}", vm);
            Assert.AreEqual(view, "Is Active");
        }

        [TestMethod]
        public void ConvertTest()
        {
            var service = new HtmlToPdfService();;
            var pdf = service.Convert("{{Title}} spends {{Calc}}", vm);
            Assert.IsTrue(pdf.Length == 7275);
        }
    }
}