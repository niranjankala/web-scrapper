using CrawlyScraper.Core.Services;
using CrawlyScraper.Framework;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace CrawlyScraper
{
    public partial class ScrapDataForm : Form
    {
        private readonly IScraperService _scraperService;
        private readonly ILogger<ScrapDataForm> _logger;

        public ScrapDataForm(IScraperService scraperService, ILogger<ScrapDataForm> logger)
        {
            InitializeComponent();
            _scraperService = scraperService;
            _logger = logger;
        }

        private async void btnScrapData_Click(object sender, EventArgs e)
        {
            string baseUrl = textBoxBaseUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(baseUrl) || !int.TryParse(textBoxPages.Text, out int pages) || pages < 1)
            {
                MessageBox.Show("Please enter valid input.");
                return;
            }

            // Generate Excel file
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            string filePath;
            string targetDirectory;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {

                filePath = saveDialog.FileName;
                targetDirectory = Path.GetDirectoryName(saveDialog.FileName);
            }
            else
            { return; }




            var progress = new Progress<int>(UpdateProgressBar);
            var progressReporter = new ProgressReporter(progress);

            try
            {
                await _scraperService.ScrapDataAsync(baseUrl, pages, targetDirectory, filePath, progressReporter);
                MessageBox.Show("Task completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during scraping process");
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }

        private void UpdateProgressBar(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                progressBar.Value = value;
            }
        }

        private async void btnProcessCategories_Click(object sender, EventArgs e)
        {
            string websiteUrl = "https://www.industrybuying.com/";
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);

            string targetDirectory = string.Empty;
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    targetDirectory = folderDialog.SelectedPath;
                }
            }

            if (!string.IsNullOrEmpty(targetDirectory))
            {
                string[] categoryUrls = File.ReadAllLines($"{directory}\\App_Data\\categories.txt");
                var tasks = new List<Task>();

                foreach (var item in categoryUrls)
                {
                    if (item.StartsWith("#"))
                        continue;
                    string[] lookup = item.Split('|');
                    string parentCategoryName = lookup[0];
                    string parentUrl = lookup[1];

                    string categoryDirectory = Path.Combine(targetDirectory, parentCategoryName);

                    if (!Directory.Exists(categoryDirectory))
                    {
                        Directory.CreateDirectory(categoryDirectory);
                    }
                   

                    var childCategories = await GetChildCategoriesDetail(parentUrl);
                    foreach (var childCategory in childCategories)
                    {
                        int pages = (int)Math.Ceiling(childCategory.ProductCount / 60.0);

                        string filePath = Path.Combine(categoryDirectory, $"{childCategory.Name}.xlsx");
                        var progress = new Progress<int>(UpdateProgressBar);
                        var progressReporter = new ProgressReporter(progress);

                        //tasks.Add(_scraperService.ScrapDataAsync($"{websiteUrl}/{childCategory.Url}", pages, targetDirectory, filePath, progressReporter));

                        try
                        {
                            await _scraperService.ScrapDataAsync($"{websiteUrl}/{childCategory.Url}", pages, categoryDirectory, filePath, progressReporter);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error occurred during scraping process for category:{parentCategoryName}=>{childCategory}");
                        }
                        //tasks.Add(_scraperService.ScrapDataAsync(childCategory.Url, pages, filePath));
                    }

                    //try
                    //{
                    //    await Task.WhenAll(tasks);
                    //}
                    //catch (Exception ex)
                    //{
                    //    _logger.LogError(ex, $"Error occurred during scraping process");
                    //}

                }


            }

        }

        private async Task<List<ChildCategory>> GetChildCategoriesDetail(string parentUrl)
        {
            List<ChildCategory> childCategories = new List<ChildCategory>();

            try
            {
                HtmlWeb web = new HtmlWeb();
                var document = web.Load(parentUrl);

                var categoryNodes = document.DocumentNode.SelectNodes("//div[@class='cat-colm']");

                if (categoryNodes != null)
                {
                    foreach (var node in categoryNodes)
                    {
                        var nameNode = node.SelectSingleNode(".//p[@class='productTitle']/a");
                        var urlNode = nameNode;
                        var productCountNode = nameNode.SelectSingleNode(".//span");

                        string name = nameNode?.InnerText.Split('(')[0].Trim();
                        string url = urlNode?.GetAttributeValue("href", "").Trim();
                        int productCount = productCountNode != null ? int.Parse(productCountNode.InnerText.Trim('(', ')', ' ')) : 0;

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                        {
                            childCategories.Add(new ChildCategory
                            {
                                Name = name,
                                Url = url,
                                ProductCount = productCount
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching child categories: {ex.Message}");
                MessageBox.Show($"Error fetching child categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return childCategories;
        }
    }
    public class ChildCategory
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int ProductCount { get; set; }
    }
}