namespace HtmlToPdf.Core.Tests.Fixtures
{
    public class ViewModel
    {
        public string Title { get; set; }
        public int Calc { get; set; }
        public string[] Cities { get; set; }
        public string[] Places { get; set; }
        public People[] Peoples { get; set; }
        public bool Active { get; set; }
    }

    public class People
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Name
        {
            get { return FirstName + " " + LastName; }
        }
    }
}