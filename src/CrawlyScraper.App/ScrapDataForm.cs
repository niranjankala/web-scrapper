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
using System.Linq;


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
                                    worksheet.Cells[row, 3].Value = product.ProductImages.Any()? product.ProductImages.First():"N/A";
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
                        var downloadImage = productImage.Replace(@"//", "https://");
                        productImage = productImage.Replace(@"//static1.industrybuying.com/products", "catalog/default/product");

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
                            ProductPrice = productPrice,
                            Availability = productAvailability,
                            Brand = productManufacturer,                            
                        };
                        //product.ProductImages.Add(productImage);
                        //product.DownloadImages.Add(downloadImage);                            
                        var productsGroup = GetProductDetails(product, baseUrl, isProductsGroup);

                        if(!product.ProductDetails.Any())
                        {
                            product.ProductImages.Add(productImage);
                        }
                        product.DownloadImages = product.ProductImages.Select(img => img.Replace(@"//", "https://")).ToList();
                        product.ProductImages = product.ProductImages.Select(img => img.Replace(@"//static1.industrybuying.com/products", "catalog/default/product")).ToList();
                        products.AddRange(productsGroup);

                    }
                }
                else
                {
                    products.Add(new Product
                    {
                        ProductName = "No products found.",
                        ProductLink = "N/A",                       
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

        private List<Product> GetProductDetails(Product product, string baseUrl, bool isProductsGroup)
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
                var productDetails = GetGroupProducts(product, baseUrl);
                products.AddRange(productDetails);

            }
            return products;
        }

        private IEnumerable<Product> GetGroupProducts(Product product, string baseUrl)
        {
            List<Product> subProducts = new List<Product>();
            HtmlWeb web = new HtmlWeb();
            var document = web.Load(product.ProductLink);
            var productRefNodes = document.DocumentNode.SelectNodes("//table[@id='family-table']/tbody/tr/td[1]/a[1]/@href");
            if (productRefNodes != null)
            {
                foreach (var productRef in productRefNodes)
                {
                    string productUrl = productRef.GetAttributeValue("href", "N/A");
                    if (!string.IsNullOrEmpty(productUrl))
                    {
                        productUrl = $"{baseUrl}{productUrl.Trim()}";
                        Product subProduct = new Product()
                        {
                            ProductLink = productUrl
                        };
                        subProduct.ProductName = product.ProductName;
                        subProduct.Brand = product.Brand;
                        GetProductDetails(subProduct);
                        subProduct.ProductPrice = subProduct.PriceIncludingGST;                        
                        if(subProduct.ProductDetails.ContainsKey("Brand Name") && product.Brand != subProduct.ProductDetails["Brand Name"].Trim())
                        {
                            subProduct.Brand = subProduct.ProductDetails["Brand Name"].Trim();
                        }
                        subProducts.Add(subProduct);
                    }
                }
            }
                    return subProducts;
        }
        
        private Product GetProductDetails(Product product)
        {
            HtmlWeb web = new HtmlWeb();
            var document = web.Load(product.ProductLink);

            // Fetch the product title
            var productTitleNode = document.DocumentNode.SelectSingleNode("//div[@class='heading']//span[contains(@class,'productTitle')]//h1");
            var fetchedProductTitle = productTitleNode != null ? productTitleNode.InnerText.Trim() : "";

            // Update product.ProductName if it does not match the fetched value
            if (!string.IsNullOrEmpty(fetchedProductTitle) && product.ProductName != fetchedProductTitle)
            {
                product.ProductName = fetchedProductTitle;
            }

            var descriptionNode = document.DocumentNode.SelectSingleNode(".//div[@class='descriptionContent']");
            product.Description = descriptionNode != null ? descriptionNode.InnerHtml.Trim() : "";

            var gstInclusivePriceNode = document.DocumentNode.SelectSingleNode(".//span[@class='AH_PricePerPiece']");
            var gstInclusivePrice = gstInclusivePriceNode != null ? gstInclusivePriceNode.InnerText.Trim() : "N/A";

            var gstExclusivePriceNode = document.DocumentNode.SelectSingleNode(".//div[@class='mainPrice']/span[@class='price']");
            var gstExclusivePrice = gstExclusivePriceNode != null ? gstExclusivePriceNode.InnerText.Trim() : "N/A";

            product.ProductDetails.Add("GST Inclusive Price", gstInclusivePrice);
            product.ProductDetails.Add("GST Exclusive Price", gstExclusivePrice);

            // Fetch product images
            var imageNodes = document.DocumentNode.SelectNodes("//ul[@class='thumbsArea']//img");
            if (imageNodes != null)
            {                
                foreach (var node in imageNodes)
                {
                    var imageUrl = node.GetAttributeValue("data-zoom-image", null);
                    if (!string.IsNullOrEmpty(imageUrl) && imageUrl.Contains("industrybuying.com") && !product.ProductImages.Contains(imageUrl))
                    {
                        product.ProductImages.Add(imageUrl);
                    }
                }
            }

            var rowNodes = document.DocumentNode.SelectNodes("//div[@class='features']//table//tr[not(td/@colspan)]");
            if (rowNodes != null)
            {
                foreach (var node in rowNodes)
                {
                    // Select the first and second td elements within the current row
                    var featureNameNode = node.SelectSingleNode(".//td[1]");
                    var featureValueNode = node.SelectSingleNode(".//td[2]");

                    // Get the text content of the td elements, or set to "N/A" if not found
                    var featureName = featureNameNode != null ? featureNameNode.InnerText.Trim() : "N/A";
                    var featureValue = featureValueNode != null ? featureValueNode.InnerText.Trim() : "N/A";

                    // Remove leading spaces and colons from the feature value
                    featureName = featureName.TrimEnd(' ', ':');
                    featureValue = featureValue.TrimStart(' ', ':');

                    // Add the feature name and value to the dictionary if the feature name is not "N/A" and not already present in the dictionary
                    if (featureName != "N/A" && !product.ProductDetails.ContainsKey(featureName))
                    {
                        product.ProductDetails.Add(featureName, featureValue);
                    }
                }
            }

            rowNodes = document.DocumentNode.SelectNodes("//div[@id='productSpecifications']/div[contains(@class,'tabDetailsContainer')]//div[contains(@class,'filterRow')]");
            
            if (rowNodes != null)
            {
                foreach (var node in rowNodes)
                {
                    var featureNameNode = node.SelectSingleNode(".//div[@class='featureNamePr']");
                    var featureName = featureNameNode != null ? featureNameNode.InnerText.Trim() : "N/A";

                    var featureValueNode = node.SelectSingleNode(".//div[@class='featureValuePr']");
                    var featureValue = featureValueNode != null ? featureValueNode.InnerText.Trim() : "N/A";
                    featureValue = featureValue.TrimStart(' ', ':');

                    if (featureName != "N/A" && !product.ProductDetails.ContainsKey(featureName))
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
            foreach (var product in products)
            {
                await DownloadImagesAsync(product.DownloadImages, targetDirectory);
            }
        }
        private async Task DownloadImagesAsync(List<string> downloadImages, string targetDirectory)
        {
            using (HttpClient client = new HttpClient())
            {
                foreach (var downloadImage in downloadImages)
                {
                    if (!string.IsNullOrWhiteSpace(downloadImage) && downloadImage != "N/A")
                    {
                        try
                        {
                            string imageUrl = downloadImage;
                            string relativePath = GetRelativePathFromUrl(imageUrl);
                            string fullPath = Path.Combine(targetDirectory, relativePath);

                            // Ensure the directory exists
                            string directoryPath = Path.GetDirectoryName(fullPath);
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            using (HttpResponseMessage response = await client.GetAsync(imageUrl))
                            {
                                response.EnsureSuccessStatusCode();

                                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                                await File.WriteAllBytesAsync(fullPath, imageBytes);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log or handle the exception appropriately
                            MessageBox.Show($"Error downloading image {downloadImage}: {ex.Message}");
                        }
                    }
                }
            }
        }

        private string GetRelativePathFromUrl(string url)
        {
            // This method will convert the URL to a relative path
            Uri uri = new Uri(url);
            string path = uri.AbsolutePath.TrimStart('/');
            // Replace any URL-safe characters to ensure valid file paths
            path = path.Replace('/', Path.DirectorySeparatorChar);
            return path;
        }
        //private async Task DownloadImagesAsync(List<string> downloadImages, string targetDirectory)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        foreach (var downloadImage in downloadImages)
        //        {
        //            if (!string.IsNullOrWhiteSpace(downloadImage) && downloadImage != "N/A")
        //            {
        //                try
        //                {
        //                    string imageUrl = downloadImage;
        //                    string fileName = Path.Combine(targetDirectory, GetFileNameFromUrl(imageUrl));

        //                    using (HttpResponseMessage response = await client.GetAsync(imageUrl))
        //                    {
        //                        response.EnsureSuccessStatusCode();

        //                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

        //                        await File.WriteAllBytesAsync(fileName, imageBytes);                                
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Log or handle the exception appropriately
        //                    MessageBox.Show($"Error downloading image {downloadImage}: {ex.Message}");
        //                }
        //            }
        //        }
        //    }
        //}

        private string GetFileNameFromUrl(string url)
        {
            return Path.GetFileName(new Uri(url).AbsolutePath);
        }

        private class Product
        {
            public string ProductName { get; set; }
            public string ProductLink { get; set; }
            public List<string> ProductImages { get; set; } = new List<string>();
            public List<string> DownloadImages { get; set; } = new List<string>();
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