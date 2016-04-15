using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using MustacheCs;
using Newtonsoft.Json;

namespace HtmlToPdf.Core
{
    /// <summary>
    /// Сервис преоброзования HTML в PDF
    /// </summary>
    public class HtmlToPdfService : IDisposable
    {
        private static string _appPath;
        private static string _phantomjsPath;
        private static string _htmlToPdfPath;
        private Process process;

        static HtmlToPdfService()
        {
            var codeBase = Assembly.GetAssembly(typeof (HtmlToPdfService)).CodeBase;
            _appPath = Path.GetDirectoryName(codeBase).Replace(@"file:\", "");
            _phantomjsPath = Path.Combine(_appPath, "phantomjs.exe");
            _htmlToPdfPath = Path.Combine(_appPath, "HtmlToPdf.js");
        }

        public HtmlToPdfService()
        {
            var args = new object[] {_htmlToPdfPath}
                .Select(p => "\"" + p + "\"")
                .Aggregate("", (p1, p2) => p1 + " " + p2);
            var info = new ProcessStartInfo(_phantomjsPath, args)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                //RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process = Process.Start(info);
        }

        /// <summary>
        /// Заполнить шаблон Mustache
        /// </summary>
        /// <param name="template">Шаблон</param>
        /// <param name="vm"></param>
        /// <returns>Готовый шаблон</returns>
        public static string Render(string template, object vm = null)
        {
            var content = Mustache.render(template, vm);
            return content;
        }

        /// <summary>
        /// Сконвертировать готовый шаблон в PDF
        /// </summary>
        /// <param name="content">Готовый шаблон</param>
        /// <returns>PDF</returns>
        public byte[] Convert(string content)
        {
            var name = Guid.NewGuid().ToString("N");
            var json = JsonConvert.SerializeObject(new
            {
                content,
                name
            });
            process.StandardInput.WriteLine(json);

            //ожидание выполнения
            var stdout = process.StandardOutput.ReadLine();
            switch (stdout)
            {
                case "converted":
                {
                    var path = Path.Combine(_appPath, name + ".pdf");
                    var bytes = File.ReadAllBytes(path);
                    File.Delete(path);
                    return bytes;
                }
                case "empty":
                {
                    return new byte[] {};
                }
                default:
                {
                    throw new NotImplementedException(stdout);
                }
            }
        }

        /// <summary>
        /// Заполнить шаблон Mustache и сконвертировать в PDF
        /// </summary>
        /// <param name="template">Шаблон</param>
        /// <param name="vm"></param>
        /// <returns>PDF</returns>
        public byte[] Convert(string template, object vm)
        {
            var content = Render(template, vm);
            var bytes = Convert(content);
            return bytes;
        }

        public void Dispose()
        {
            if (process == null) return;
            process.StandardInput.WriteLine("exit");
            var stdout = process.StandardOutput.ReadLine();
            switch (stdout)
            {
                case "exit":
                {
                    process.WaitForExit();
                    process.Close();
                    process.Dispose();
                    process = null;
                    break;
                }
                default:
                {
                    process.Close();
                    process.Dispose();
                    process = null;
                    throw new NotImplementedException(stdout);
                }
            }
        }
    }
}