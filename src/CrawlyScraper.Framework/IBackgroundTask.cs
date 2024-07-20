namespace CrawlyScraper.Framework
{
    public interface IBackgroundTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}