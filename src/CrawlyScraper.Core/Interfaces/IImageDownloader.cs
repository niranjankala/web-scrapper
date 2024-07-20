using CrawlyScraper.Core.Models;

namespace CrawlyScraper.Core.Interfaces
{
    public interface IImageDownloader
    {
        Task DownloadImagesAsync(List<Product> products, string targetDirectory);
    }
}