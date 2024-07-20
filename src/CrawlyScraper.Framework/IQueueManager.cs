namespace CrawlyScraper.Framework
{
    public interface IQueueManager<T>
    {
        void Enqueue(T item);
        T Dequeue();
        bool TryDequeue(out T item);
        int Count { get; }
    }
}