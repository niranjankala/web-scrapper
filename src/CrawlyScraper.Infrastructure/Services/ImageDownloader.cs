using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CrawlyScraper.Core.Interfaces;
using CrawlyScraper.Core.Models;
using Microsoft.Extensions.Logging;

namespace CrawlyScraper.Infrastructure.Services
{
    public class ImageDownloader : IImageDownloader
    {
        private readonly ILogger<ImageDownloader> _logger;
        private readonly HttpClient _httpClient;

        public ImageDownloader(ILogger<ImageDownloader> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task DownloadImagesAsync(List<Product> products, string targetDirectory)
        {
            foreach (var product in products)
            {
                await DownloadImagesForProductAsync(product, targetDirectory);
            }
        }

        private async Task DownloadImagesForProductAsync(Product product, string targetDirectory)
        {
            foreach (var imageUrl in product.ProductImages)
            {
                if (!string.IsNullOrWhiteSpace(imageUrl) && imageUrl != "N/A")
                {
                    try
                    {
                        string relativePath = GetRelativePathFromUrl(imageUrl);
                        string fullPath = Path.Combine(targetDirectory, relativePath);

                        string directoryPath = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        using (var response = await _httpClient.GetAsync(imageUrl))
                        {
                            response.EnsureSuccessStatusCode();
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var fileStream = new FileStream(fullPath, FileMode.Create))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }

                        _logger.LogInformation("Downloaded image: {ImageUrl}", imageUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error downloading image: {ImageUrl}", imageUrl);
                    }
                }
            }
        }

        private string GetRelativePathFromUrl(string url)
        {
            Uri uri = new Uri(url);
            string path = uri.AbsolutePath.TrimStart('/');
            return path.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}