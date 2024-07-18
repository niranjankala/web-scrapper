using HtmlAgilityPack;
using Microsoft.FSharp.Data.UnitSystems.SI.UnitNames;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace CrawlyScraper
{
    public partial class ScrapDataForm : Form
    {
        public ScrapDataForm()
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

            textBoxContent.Clear();

            List<Product> products = new List<Product>();

            for (int i = 1; i <= pages; i++)
            {
                string url = $"{baseUrl}?page={i}";
                var productsOnPage = await Task.Run(() => CrawlWebsite(url));
                products.AddRange(productsOnPage);
            }

            // Ask the user to select a directory to save images
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string targetDirectory = folderDialog.SelectedPath;

                    // Download images
                    await DownloadImagesAsync(products, targetDirectory);

                    // Generate Excel file
                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = true
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveDialog.FileName;

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

                                for (int i = 0; i < columns.Count; i++)
                                {
                                    worksheet.Cells[1, i + 1].Value = columns[i];
                                }

                                int row = 2;
                                foreach (var product in products)
                                {
                                    worksheet.Cells[row, 1].Value = product.ProductName;
                                    worksheet.Cells[row, 2].Value = product.ProductLink;
                                    worksheet.Cells[row, 3].Value = product.ProductImage;
                                    worksheet.Cells[row, 4].Value = product.DownloadImage;
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
            }
        }


        private List<Product> CrawlWebsite(string url)
        {
            List<Product> products = new List<Product>();
            try
            {
                Uri uri = new Uri(url);
                string baseUrl = $"{uri.Scheme}://{uri.Host}";
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
                        if (productLink != "N/A")
                        {
                            productLink = $@"{baseUrl}{productLink}";
                        }


                        var productImageNode = node.SelectSingleNode(".//img[@class='AH_LazyLoadImg']");
                        var productImage = productImageNode != null ? productImageNode.GetAttributeValue("data-original", "N/A") : "N/A";
                        productImage = productImage.Replace(@"//static1.industrybuying.com/products", "catalog/default/product");

                        var dowloadImageNode = node.SelectSingleNode(".//img[@class='AH_LazyLoadImg']");
                        var downloadImage = dowloadImageNode != null ? dowloadImageNode.GetAttributeValue("data-original", "N/A") : "N/A";
                        downloadImage = downloadImage.Replace(@"//", "https://");

                        var productPriceNode = node.SelectSingleNode(".//span[@class='rs']");
                        var productPrice = productPriceNode != null ? productPriceNode.InnerText.Trim() : "N/A";
                        bool isProductsGroup = false;
                        if (productPrice == "N/A")
                        {
                            // Try another node for price if the first one is not found
                            productPriceNode = node.SelectSingleNode(".//div[contains(@class,'proPriceSpan')]");
                            productPrice = productPriceNode != null ? ParseProductPrice(productPriceNode.InnerText.Trim()) : "N/A";
                            isProductsGroup = true;
                        }

                        var productAvailabilityNode = node.SelectSingleNode(".//div[@class='express-delivery-in']");
                        var productAvailability = productAvailabilityNode != null ? "Ships within 24 hrs" : "N/A";

                        var productManufacturerNode = node.SelectSingleNode(".//span[@class='brand']");
                        var productManufacturer = productManufacturerNode != null ? productManufacturerNode.InnerText.Trim() : "N/A";


                        var product = new Product
                        {
                            ProductName = productName,
                            ProductLink = productLink,
                            ProductImage = productImage,
                            DownloadImage = downloadImage,
                            ProductPrice = productPrice,
                            Availability = productAvailability,
                            Brand = productManufacturer
                        };

                        var productsGroup = GetProductDetails(product, isProductsGroup);
                        products.AddRange(productsGroup);

                    }
                }
                else
                {
                    products.Add(new Product
                    {
                        ProductName = "No products found.",
                        ProductLink = "N/A",
                        ProductImage = "N/A",
                        DownloadImage = "N/A",
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

        private List<Product> GetProductDetails(Product product, bool isProductsGroup)
        {
            List<Product> products = new List<Product>();

            if (!isProductsGroup)
            {
                var productDetails = GetProductDetails(product);
                products.Add(product);
            }
            else
            {
                //Process the page that has list of products by finding the URL of the products
                var productDetails = GetProductDetails(product);
                products.Add(product);

            }
            return products;
        }

        private Product GetProductDetails(Product product)
        {
            HtmlWeb web = new HtmlWeb();
            var document = web.Load(product.ProductLink);

            var descriptionNode = document.DocumentNode.SelectSingleNode(".//div[@class='descriptionContent']");
            product.Description = descriptionNode != null ? descriptionNode.InnerHtml.Trim() : "";

            var gstInclusivePriceNode = document.DocumentNode.SelectSingleNode(".//span[@class='AH_PricePerPiece']");
            var gstInclusivePrice = gstInclusivePriceNode != null ? gstInclusivePriceNode.InnerText.Trim() : "N/A";

            var gstExclusivePriceNode = document.DocumentNode.SelectSingleNode(".//div[@class='mainPrice']/span[@class='price']");
            var gstExclusivePrice = gstExclusivePriceNode != null ? gstExclusivePriceNode.InnerText.Trim() : "N/A";

            product.ProductDetails.Add("GST Inclusive Price", gstInclusivePrice);
            product.ProductDetails.Add("GST Exclusive Price", gstExclusivePrice);


            var rowNodes = document.DocumentNode.SelectNodes("//div[@id='productSpecifications']/div[contains(@class,'tabDetailsContainer')]//div[contains(@class,'filterRow')]");

            if (rowNodes != null)
            {
                foreach (var node in rowNodes)
                {
                    var featureNameNode = node.SelectSingleNode(".//div[@class='featureNamePr']");
                    var featureName = featureNameNode != null ? featureNameNode.InnerText.Trim() : "N/A";

                    var featureValueNode = node.SelectSingleNode(".//div[@class='featureValuePr']");
                    var featureValue = featureValueNode != null ? featureValueNode.InnerText.Trim() : "N/A";
                    featureValue = featureValue.TrimStart(' ', ':');

                    if (!product.ProductDetails.ContainsKey(featureName))
                    {
                        product.ProductDetails.Add(featureName, featureValue);
                    }
                }
            }

            return product;
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


        private async Task DownloadImagesAsync(List<Product> products, string targetDirectory)
        {
            using (HttpClient client = new HttpClient())
            {
                foreach (var product in products)
                {
                    if (!string.IsNullOrWhiteSpace(product.DownloadImage) && product.DownloadImage != "N/A")
                    {
                        try
                        {
                            string imageUrl = product.DownloadImage;
                            string fileName = Path.Combine(targetDirectory, GetFileNameFromUrl(imageUrl));

                            using (HttpResponseMessage response = await client.GetAsync(imageUrl))
                            {
                                response.EnsureSuccessStatusCode();

                                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                                await File.WriteAllBytesAsync(fileName, imageBytes);
                                product.DownloadImage = fileName; // Update the product with the local file path
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log or handle the exception appropriately
                            MessageBox.Show($"Error downloading image {product.DownloadImage}: {ex.Message}");
                        }
                    }
                }
            }
        }
        private string GetFileNameFromUrl(string url)
        {
            return Path.GetFileName(new Uri(url).AbsolutePath);
        }

        private class Product
        {
            public string ProductName { get; set; }
            public string ProductLink { get; set; }
            public string ProductImage { get; set; }
            public string DownloadImage { get; set; }
            public string ProductPrice { get; set; }
            public string PriceIncludingGST { get; set; }
            public string PriceExcludingGST { get; set; }
            public string Availability { get; set; }
            public string Brand { get; set; }
            public string Description { get; set; }

            public Dictionary<string, string> ProductDetails { get; set; } = new Dictionary<string, string>();
        }

        private class ProductDetail
        {

            public string SKU { get; set; }
            public string BrandName { get; set; }
            public string EnginePower { get; set; }
            public string CountryofOrigin { get; set; }
            public string TypeofProduct { get; set; }
            public string Weight { get; set; }
            public string Dimension { get; set; }
            public string ModelNo { get; set; }
            public string NoofGears { get; set; }
            public string UsageApplication { get; set; }
            public string NoofBlades { get; set; }
            public string OperationMethod { get; set; }
            public string AutomationGrade { get; set; }
            public string NameofManufacturer_Packer_Importer { get; set; }






        }

    
    }
}