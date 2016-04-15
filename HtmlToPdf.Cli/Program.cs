using System.IO;
using HtmlToPdf.Core;

namespace HtmlToPdf.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new HtmlToPdfService())
            {
                var pdf = service.Convert(args[0]);
                File.WriteAllBytes("result.pdf", pdf);
            }
        }
    }
}