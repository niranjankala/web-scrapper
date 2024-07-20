namespace CrawlyScraper.Framework
{
    public interface IProgressReporter
    {
        void ReportProgress(int percentComplete);
    }
}