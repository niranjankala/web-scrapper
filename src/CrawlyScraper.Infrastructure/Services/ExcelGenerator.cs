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
                                "Download Image",
                                "Product Price",
                                "Availability",
                                "Brand",
                                "Description"
                            };

                    products.SelectMany(p => p.ProductDetails.Keys)
                        .Distinct()
                        .ToList()
                        .ForEach(pName =>
                        {
                            if (!columns.Contains(pName))
                            {
                                columns.Add(pName);
                            }
                        });

                    // Write headers
                    for (int i = 0; i < columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = columns[i];
                    }

                    // Write data
                    int row = 2;
                    foreach (var product in products)
                    {
                        worksheet.Cells[row, 1].Value = product.ProductName;
                        worksheet.Cells[row, 2].Value = product.ProductLink;
                        worksheet.Cells[row, 3].Value = product.ProductImages.Any() ? product.ProductImages.First() : "N/A";
                        worksheet.Cells[row, 4].Value = product.DownloadImages.Any() ? product.DownloadImages.First() : "N/A";
                        worksheet.Cells[row, 5].Value = product.ProductPrice;
                        worksheet.Cells[row, 6].Value = product.Availability;
                        worksheet.Cells[row, 7].Value = product.Brand;
                        worksheet.Cells[row, 8].Value = product.Description;
                        foreach (string specName in product.ProductDetails.Keys)
                        {
                            worksheet.Cells[row, columns.IndexOf(specName) + 1].Value = product.ProductDetails[specName];
                        }
                        row++;
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