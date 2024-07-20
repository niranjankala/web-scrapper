namespace CrawlyScraper.Core.Interfaces
{
    public interface IQueueManager
    {
        void EnqueueTask(Func<Task> task);
        Task<bool> IsQueueEmptyAsync();
        Task ProcessQueueAsync(IProgress<int> progress);
    }
}