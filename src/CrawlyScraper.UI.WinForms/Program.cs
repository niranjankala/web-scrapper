namespace CrawlyScraper.UI.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var application = new ScraperApplication();
            application.Run();
        }
    }
}