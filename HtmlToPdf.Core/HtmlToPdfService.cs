using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using MustacheCs;

namespace HtmlToPdf.Core
{
    /// <summary>
    /// Сервис преоброзования HTML в PDF
    /// </summary>
    public class HtmlToPdfService
    {
        private static string _appPath;
        private static string _phantomjsPath;
        private static string _htmlToPdfPath;
        private static string _outputPath;

        static HtmlToPdfService()
        {
            var codeBase = Assembly.GetAssembly(typeof (HtmlToPdfService)).CodeBase;
            _appPath = Path.GetDirectoryName(codeBase).Replace(@"file:\", "");
            _phantomjsPath = Path.Combine(_appPath, "phantomjs.exe");
            _htmlToPdfPath = Path.Combine(_appPath, "HtmlToPdf.js");
            _outputPath = Path.Combine(_appPath, "stdout.pdf");
        }

        /// <summary>
        /// Заполнить шаблон Mustache
        /// </summary>
        /// <param name="template">Шаблон</param>
        /// <param name="vm"></param>
        /// <returns>Готовый шаблон</returns>
        public string Render(string template, object vm = null)
        {
            var view = Mustache.render(template, vm);
            return view;
        }

        /// <summary>
        /// Сконвертировать готовый шаблон в PDF
        /// </summary>
        /// <param name="view">Готовый шаблон</param>
        /// <returns>PDF</returns>
        public byte[] Convert(string view)
        {
            //http://www.lucidmotions.net/2014/05/csharp-javascript-phantomjs-screenshots.html неработает =(
            var args = new object[] {_htmlToPdfPath, view};
            var info = new ProcessStartInfo(_phantomjsPath,
                string.Join(" ", string.Join(" ", args.Select(p => "\"" + p + "\""))))
            {
                //RedirectStandardInput = true,
                //RedirectStandardOutput = true,
                //RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var process = new Process {StartInfo = info})
            {
                process.Start();
                //var error = process.StandardError.ReadToEnd();
                //var stdout = process.StandardOutput.ReadToEnd(); //приходит пустой пдф =(
                process.WaitForExit();
            }
            return File.ReadAllBytes(_outputPath);
        }

        /// <summary>
        /// Заполнить шаблон Mustache и сконвертировать в PDF
        /// </summary>
        /// <param name="template">Шаблон</param>
        /// <param name="vm"></param>
        /// <returns>PDF</returns>
        public byte[] Convert(string template, object vm)
        {
            var view = Render(template, vm);
            var pdf = Convert(view);
            return pdf;
        }
    }
}