using HtmlAgilityPack;
using OfficeOpenXml;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace CrawlyScraper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnScrapData_Click(object sender, EventArgs e)
        {
            string baseUrl = textBoxBaseUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                MessageBox.Show("Please enter a valid Base URL.");
                return;
            }

            if (!int.TryParse(textBoxPages.Text, out int pages) || pages < 1)
            {
                MessageBox.Show("Please enter a valid number of pages.");
                return;
            }

            // Clear previous content
            textBoxContent.Clear();

            // Use List to store products
            List<Product> products = new List<Product>();

            // Crawl all pages
            for (int i = 1; i <= pages; i++)
            {
                string url = $"{baseUrl}?page={i}";
                var productsOnPage = await Task.Run(() => CrawlWebsite(url));
                products.AddRange(productsOnPage);
            }

            // Generate Excel file
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveDialog.FilterIndex = 1;
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveDialog.FileName;

                try
                {
                    // Set EPPlus license context
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Products");

                        // Headers
                        worksheet.Cells[1, 1].Value = "Product Name";
                        worksheet.Cells[1, 2].Value = "Product Link";
                        worksheet.Cells[1, 3].Value = "Product Image";
                        worksheet.Cells[1, 4].Value = "Product Price";
                        worksheet.Cells[1, 5].Value = "Availability";
                        worksheet.Cells[1, 6].Value = "Brand";

                        // Data
                        int row = 2;
                        foreach (var product in products)
                        {
                            worksheet.Cells[row, 1].Value = product.ProductName;
                            worksheet.Cells[row, 2].Value = product.ProductLink;
                            worksheet.Cells[row, 3].Value = product.ProductImage;
                            worksheet.Cells[row, 4].Value = product.ProductPrice;
                            worksheet.Cells[row, 5].Value = product.Availability;
                            worksheet.Cells[row, 6].Value = product.Brand;
                            row++;
                        }

                        // Auto-fit columns for better readability
                        worksheet.Cells.AutoFitColumns();

                        // Save Excel package
                        package.SaveAs(new FileInfo(filePath));
                        MessageBox.Show("Excel file saved successfully.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving Excel file: {ex.Message}");
                }
            }
        }

        private List<Product> CrawlWebsite(string url)
        {
            List<Product> products = new List<Product>();
            try
            {
                HtmlWeb web = new HtmlWeb();
                var document = web.Load(url);

                var productNodes = document.DocumentNode.SelectNodes("//div[@class='AH_ProductView col-lg-3 col-md-3 col-sm-6 col-xs-6 productThumbnails']");
                if (productNodes != null)
                {
                    foreach (var node in productNodes)
                    {
                        var productNameNode = node.SelectSingleNode(".//a[@class='prFeatureName']");
                        var productName = productNameNode != null ? productNameNode.InnerText.Trim() : "N/A";

                        var productLinkNode = node.SelectSingleNode(".//a[@class='prFeatureName']");
                        var productLink = productLinkNode != null ? productLinkNode.GetAttributeValue("href", "N/A") : "N/A";

                        var productImageNode = node.SelectSingleNode(".//img[@class='AH_LazyLoadImg']");
                        var productImage = productImageNode != null ? productImageNode.GetAttributeValue("data-original", "N/A") : "N/A";

                        var productPriceNode = node.SelectSingleNode(".//span[@class='rs']");
                        var productPrice = productPriceNode != null ? productPriceNode.InnerText.Trim() : "N/A";

                        if (productPrice == "N/A")
                        {
                            // Try another node for price if the first one is not found
                            productPriceNode = node.SelectSingleNode(".//div[@class='proPriceSpan']");
                            productPrice = productPriceNode != null ? ParseProductPrice(productPriceNode.InnerText.Trim()) : "N/A";
                        }

                        var productAvailabilityNode = node.SelectSingleNode(".//div[@class='express-delivery-in']");
                        var productAvailability = productAvailabilityNode != null ? "Ships within 24 hrs" : "N/A";

                        var productManufacturerNode = node.SelectSingleNode(".//span[@class='brand']");
                        var productManufacturer = productManufacturerNode != null ? productManufacturerNode.InnerText.Trim() : "N/A";

                        products.Add(new Product
                        {
                            ProductName = productName,
                            ProductLink = productLink,
                            ProductImage = productImage,
                            ProductPrice = productPrice,
                            Availability = productAvailability,
                             Brand = productManufacturer
                        });
                    }
                }
                else
                {
                    products.Add(new Product
                    {
                        ProductName = "No products found.",
                        ProductLink = "N/A",
                        ProductImage = "N/A",
                        ProductPrice = "N/A",
                        Availability = "N/A",
                         Brand = "N/A"
                    });
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                MessageBox.Show($"Error crawling {url}: {ex.Message}");
            }

            return products;
        }

        private string ParseProductPrice(string priceText)
        {
            // Check if the price text contains a '-' separator
            if (priceText.Contains('-'))
            {
                // Split and take the first part as the price
                return priceText.Split('-')[0].Trim();
            }
            else
            {
                // If no '-', return the original price
                return priceText;
            }
        }
        private class Product
        {
            public string ProductName { get; set; }
            public string ProductLink { get; set; }
            public string ProductImage { get; set; }
            public string ProductPrice { get; set; }
            public string Availability { get; set; }
            public string Brand { get; set; }
        }
    }
}