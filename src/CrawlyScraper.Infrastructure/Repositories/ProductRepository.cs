using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using CrawlyScraper.Core.Interfaces;
using CrawlyScraper.Core.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace CrawlyScraper.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ILogger<ProductRepository> logger)
        {
            _logger = logger;
        }

        public async Task<List<Product>> GetProductsAsync(string url, int pages)
        {
            List<Product> products = new List<Product>();
            for (int i = 1; i <= pages; i++)
            {
                string pageUrl = $"{url}?page={i}";
                var productsOnPage = await Task.Run(() => CrawlWebsite(pageUrl));
                products.AddRange(productsOnPage);
            }
            return products;
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
                        var productList = ExtractProductFromNode(node, baseUrl);
                        products.AddRange(productList);
                    }
                }
                else
                {
                    _logger.LogWarning("No products found on page: {Url}", url);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crawling {Url}", url);
            }
            return products;
        }

        private List<Product> ExtractProductFromNode(HtmlNode node, string baseUrl)
        {
            List<Product> products = new List<Product>();
            var productNameNode = node.SelectSingleNode(".//a[@class='prFeatureName']");
            var productName = productNameNode?.InnerText.Trim() ?? "N/A";

            var productLinkNode = node.SelectSingleNode(".//a[@class='prFeatureName']");
            var productLink = productLinkNode != null ? $"{baseUrl}{productLinkNode.GetAttributeValue("href", "")}" : "N/A";

            var productImageNode = node.SelectSingleNode(".//img[@class='AH_LazyLoadImg']");
            var productImage = productImageNode?.GetAttributeValue("data-original", "N/A") ?? "N/A";
            var downloadImage = productImage.Replace(@"//", "https://");
            productImage = productImage.Replace(@"//static1.industrybuying.com/products", "catalog/default/product");

            var productPriceNode = node.SelectSingleNode(".//span[@class='rs']");
            var productPrice = productPriceNode?.InnerText.Trim() ?? "N/A";
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
            var productManufacturer = productManufacturerNode?.InnerText.Trim() ?? "N/A";

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

            productsGroup.ForEach(p => {

                p.DownloadImages = p.ProductImages.Select(img => img.Replace(@"//", "https://")).ToList();
                p.ProductImages = p.ProductImages.Select(img => img.Replace(@"//static1.industrybuying.com/products", "catalog/default/product")).ToList();
                if (!p.ProductDetails.Any())
                {
                    p.ProductImages.Add(productImage);
                    p.DownloadImages.Add(downloadImage);
                }
            }
            );

            if (!product.ProductDetails.Any())
            {
                product.ProductImages.Add(productImage);
                product.DownloadImages.Add(downloadImage);
            }
            products.AddRange(productsGroup);

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
            try
            {


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
                            if (subProduct.ProductDetails.ContainsKey("Brand Name") && product.Brand != subProduct.ProductDetails["Brand Name"].Trim())
                            {
                                subProduct.Brand = subProduct.ProductDetails["Brand Name"].Trim();
                            }
                            subProducts.Add(subProduct);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crawling {product.ProductLink}: {ex.Message}");
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
            if (imageNodes == null)
            {
                imageNodes = document.DocumentNode.SelectNodes("//div[contains(@class,'AH_ProductDisplayImage')]//img[contains(@class,'zoom_img')]");
            }
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


            else
            {

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
    }
}