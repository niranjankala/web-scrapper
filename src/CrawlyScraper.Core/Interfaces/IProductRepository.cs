using CrawlyScraper.Core.Models;

namespace CrawlyScraper.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync(string url, int pages);
    }
}