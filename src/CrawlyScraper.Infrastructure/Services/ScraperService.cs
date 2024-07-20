using CrawlyScraper.Core.Interfaces;
using CrawlyScraper.Core.Services;
using CrawlyScraper.Framework;
using Microsoft.Extensions.Logging;

namespace CrawlyScraper.Infrastructure
{
    public class ScraperService : IScraperService
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageDownloader _imageDownloader;
        private readonly IExcelGenerator _excelGenerator;
        private readonly IQueueManager<Func<Task>> _queueManager;
        private readonly ILogger<ScraperService> _logger;

        public ScraperService(
            IProductRepository productRepository,
            IImageDownloader imageDownloader,
            IExcelGenerator excelGenerator,
            IQueueManager<Func<Task>> queueManager,
            ILogger<ScraperService> logger)
        {
            _productRepository = productRepository;
            _imageDownloader = imageDownloader;
            _excelGenerator = excelGenerator;
            _queueManager = queueManager;
            _logger = logger;
        }

        public async Task ScrapDataAsync(string baseUrl, int pages, string targetDirectory, string excelFilePath, IProgressReporter progressReporter)
        {
            _queueManager.Enqueue(async () =>
            {
                try
                {
                    var products = await _productRepository.GetProductsAsync(baseUrl, pages);
                    progressReporter.ReportProgress(33);

                    await _imageDownloader.DownloadImagesAsync(products, targetDirectory);
                    progressReporter.ReportProgress(66);

                    await _excelGenerator.GenerateExcelFileAsync(products, excelFilePath);
                    progressReporter.ReportProgress(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during scraping process");
                    throw;
                }
            });

            while (_queueManager.Count > 0)
            {
                if (_queueManager.TryDequeue(out var task))
                {
                    await task();
                }
            }
        }
    }
}