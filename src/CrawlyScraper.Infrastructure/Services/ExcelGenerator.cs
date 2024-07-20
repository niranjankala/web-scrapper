using CrawlyScraper.Core.Interfaces;
using CrawlyScraper.Core.Models;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.ComponentModel;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace CrawlyScraper.Infrastructure.Services
{
    public class ExcelGenerator : IExcelGenerator
    {
        private readonly ILogger<ExcelGenerator> _logger;

        public ExcelGenerator(ILogger<ExcelGenerator> logger)
        {
            _logger = logger;
        }

        public async Task GenerateExcelFileAsync(List<Product> products, string filePath)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Products");

                    List<string> columns = new List<string>
                    {
                        "Product Name",
                        "Product Link",
                        "Product Image",
                        "Product Price",
                        "Availability",
                        "Brand"
                    };

                    // Add dynamic columns for product details
                    var allProductDetails = products.SelectMany(p => p.ProductDetails.Keys).Distinct().ToList();
                    columns.AddRange(allProductDetails);

                    // Write headers
                    for (int i = 0; i < columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = columns[i];
                    }

                    // Write data
                    for (int i = 0; i < products.Count; i++)
                    {
                        var product = products[i];
                        int row = i + 2;

                        worksheet.Cells[row, 1].Value = product.ProductName;
                        worksheet.Cells[row, 2].Value = product.ProductLink;
                        worksheet.Cells[row, 3].Value = product.ProductImages.FirstOrDefault() ?? "N/A";
                        worksheet.Cells[row, 4].Value = product.ProductPrice;
                        worksheet.Cells[row, 5].Value = product.Availability;
                        worksheet.Cells[row, 6].Value = product.Brand;

                        // Write dynamic product details
                        for (int j = 0; j < allProductDetails.Count; j++)
                        {
                            string detailKey = allProductDetails[j];
                            worksheet.Cells[row, j + 7].Value = product.ProductDetails.TryGetValue(detailKey, out string detailValue) ? detailValue : "";
                        }
                    }

                    worksheet.Cells.AutoFitColumns();

                    await package.SaveAsAsync(new FileInfo(filePath));
                }

                _logger.LogInformation("Excel file generated successfully: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel file: {FilePath}", filePath);
                throw;
            }
        }
    }
}