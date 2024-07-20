using CrawlyScraper.Core.Models;

namespace CrawlyScraper.Core.Interfaces
{
    public interface IExcelGenerator
    {
        Task GenerateExcelFileAsync(List<Product> products, string filePath);
    }
}