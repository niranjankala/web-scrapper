using HtmlAgilityPack;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using LicenseContext = OfficeOpenXml.LicenseContext;


namespace CrawlyScraper.App
{
    public partial class CategoriesCrawlerForm : Form
    {
        private const int RetryCount = 5;

        public CategoriesCrawlerForm()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }

        private void CategoriesCrowlerForm_Load(object sender, EventArgs e)
        {

        }

        private void btnBrowseExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtExportPath.Text = saveFileDialog.FileName;
                }
            }
        }

        private void btnBrowseCategories_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open Categories excel file exported from OpenCart portal";
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtCategoriesPath.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnBrowseImageExportFolder_Click(object sender, EventArgs e)
        {
            string targetDirectory = string.Empty;
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtImageExportPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private async void ProcessCategoriesButton_Click(object sender, EventArgs e)
        {
            List<Category> categories = new List<Category>();
            string websiteUrl = "https://www.industrybuying.com";
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);

            string targetDirectory = txtImageExportPath.Text;

            int categoryId = 1;
            int parentCategorySortOrder = 0;
            int childCategorySortOrder = 0;

            if (!string.IsNullOrEmpty(targetDirectory))
            {
                string[] categoryUrls = File.ReadAllLines($"{directory}\\App_Data\\categories.txt");
                var progress = new Progress<ProgressInfo>(UpdateProgressBar);
                var progressReporter = new ProgressReporter(progress);
                // Create tasks for downloading images
                List<Task> downloadTasks = new List<Task>();
                foreach (var item in categoryUrls)
                {
                    if (item.StartsWith("#"))
                        continue;
                    string[] lookup = item.Split('|');
                    string parentCategoryName = lookup[0];
                    string parentUrl = lookup[1];

                    string categoryImageDirectory = parentCategoryName.Replace("&amp", "").Replace("&", "").Replace("  ", " ").Replace(" ", "_").ToLower();

                    string categoryDirectory = Path.Combine(targetDirectory, categoryImageDirectory);

                    if (!Directory.Exists(categoryDirectory))
                    {
                        Directory.CreateDirectory(categoryDirectory);
                    }
                    Category parentCategory = new Category()
                    {
                        CategoryId = (++categoryId).ToString(),
                        Name = parentCategoryName,
                        ParentId = "0",
                        Columns = "1",
                        SortOrder = (++parentCategorySortOrder).ToString(),
                        Top = "1"

                    };
                    categories.Add(parentCategory);
                    var childCategories = await GetChildCategoriesDetail(parentUrl);
                    foreach (var childCategory in childCategories)
                    {

                        try
                        {
                            string imagePath = "";
                            string imageFileName = GetFileNameFromUrl(childCategory.ImageUrl);
                            string childCategoryImageDirectory = childCategory.Name.Replace("&amp", "").Replace("&", "").Replace("  ", " ").Replace(" ", "_").ToLower();
                            string imageExportPath = Path.Join(targetDirectory, categoryImageDirectory, childCategoryImageDirectory, imageFileName);
                            if (!string.IsNullOrEmpty(childCategory.ImageUrl))
                            {
                                imagePath = $"catalog/default/category/{categoryImageDirectory}/{childCategoryImageDirectory}/{imageFileName}";
                            }

                            Category subCategory = new Category()
                            {
                                CategoryId = (++categoryId).ToString(),
                                Name = childCategory.Name,
                                ParentId = parentCategory.ParentId,
                                Columns = "0",
                                SortOrder = (++childCategorySortOrder).ToString(),
                                Top = "0",
                                Image = imagePath
                            };

                            categories.Add(subCategory);

                            downloadTasks.Add(DownloadCategoryImageAsync(childCategory.ImageUrl, imageExportPath));

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                }

                progressReporter.ReportProgress(new ProgressInfo() { Value = 66, Message = $"Downloading category images..." });

                // Wait for all download tasks to complete
                await Task.WhenAll(downloadTasks);


                progressReporter.ReportProgress(new ProgressInfo() { Value = 100, Message = $"Product category download completed" });
            }
        }

        private async Task DownloadCategoryImageAsync(string imageUrl, string targetDirectory, HttpClient client, SemaphoreSlim semaphore)
        {
            try
            {
                string relativePath = GetRelativePathFromUrl(imageUrl);
                string fullPath = Path.Combine(targetDirectory, relativePath);

                // Ensure the directory exists
                string directoryPath = Path.GetDirectoryName(fullPath);

                Directory.CreateDirectory(directoryPath);

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
                //MessageBox.Show($"Error downloading image {imageUrl}: {ex.Message}");
            }
            finally
            {
                semaphore.Release(); // Release semaphore after task completes
            }
        }

        private async Task DownloadCategoryImageAsync(string imageUrl, string exportPath)
        {
            try
            {

                // Ensure the directory exists
                string directoryPath = Path.GetDirectoryName(exportPath);

                Directory.CreateDirectory(directoryPath);
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(imageUrl))
                    {
                        response.EnsureSuccessStatusCode();

                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                        await File.WriteAllBytesAsync(exportPath, imageBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                //MessageBox.Show($"Error downloading image {imageUrl}: {ex.Message}");
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

        private string GetFileNameFromUrl(string url)
        {
            return Path.GetFileName(new Uri(url).AbsolutePath);
        }

        private async void UpdateCategoriesButton_Click(object sender, EventArgs e)
        {
            string categoriesPath = txtCategoriesPath.Text;
            string exportPath = txtExportPath.Text;

            if (string.IsNullOrEmpty(categoriesPath) || string.IsNullOrEmpty(exportPath))
            {
                MessageBox.Show("Please fill in all the paths.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var progress = new Progress<ProgressInfo>(UpdateProgressBar);
            var progressReporter = new ProgressReporter(progress);

            progressReporter.ReportProgress(new ProgressInfo(0, "Processing categories..."));
            try
            {
                progressReporter.ReportProgress(new ProgressInfo(5, "Reading categories..."));
                var categories = await Task.Run(() => ReadCategories(categoriesPath));

                string websiteUrl = "https://www.industrybuying.com";
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var directory = System.IO.Path.GetDirectoryName(path);

                string targetDirectory = System.IO.Path.GetDirectoryName(exportPath);

                string[] categoryUrls = File.ReadAllLines($"{directory}\\App_Data\\categories.txt");

                var categoryPath = string.Empty; //categoryUrls.ToDictionary(x=> x.Split('|')[0], )
                foreach (var item in categoryUrls)
                {
                    if (item.StartsWith("#"))
                        continue;
                    string[] lookup = item.Split('|');
                    string parentCategoryName = lookup[0].ToLower();
                    string parentUrl = lookup[1];

                    string categoryDirectory = Path.Combine(targetDirectory, parentCategoryName);

                    //if (!Directory.Exists(categoryDirectory))
                    //{
                    //    Directory.CreateDirectory(categoryDirectory);
                    //}

                    var childCategories = await GetChildCategoriesDetail(parentUrl);
                }

            }
            catch (Exception ex)
            {
                ProgressStatusLabel.Text = "Error occurred during merging.";
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Category> ReadCategories(string filePath)
        {
            var categories = new List<Category>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets["Categories"];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var category = new Category
                    {
                        CategoryId = worksheet.Cells[row, 1].Text,
                        ParentId = worksheet.Cells[row, 2].Text,
                        Name = worksheet.Cells[row, 3].Text,
                        Top = worksheet.Cells[row, 4].Text,
                        Columns = worksheet.Cells[row, 5].Text,
                        SortOrder = worksheet.Cells[row, 6].Text,
                        Image = worksheet.Cells[row, 7].Text
                    };
                    categories.Add(category);
                }
            }

            return categories;
        }

        private async Task<List<ChildCategory>> GetChildCategoriesDetail(string parentUrl)
        {
            List<ChildCategory> childCategories = new List<ChildCategory>();

            try
            {

                var document = GetHtmlDocument(parentUrl, 1);

                var categoryNodes = document.DocumentNode.SelectNodes("//div[@class='cat-colm']");

                if (categoryNodes != null)
                {
                    foreach (var node in categoryNodes)
                    {
                        var nameNode = node.SelectSingleNode(".//p[@class='productTitle']/a");
                        var urlNode = nameNode;
                        var productCountNode = nameNode.SelectSingleNode(".//span");
                        var imageNode = node.SelectSingleNode(".//div[@class='productBox']//img[contains(@class,'AH_LazyLoadImg')]");
                        string name = nameNode?.InnerText.Split('(')[0].Trim();
                        string url = urlNode?.GetAttributeValue("href", "").Trim();

                        string imageUrl = imageNode != null ? imageNode.GetAttributeValue("data-original", "N/A") : "N/A";
                        imageUrl = imageUrl.Replace(@"//", "https://");
                        int productCount = productCountNode != null ? int.Parse(productCountNode.InnerText.Trim('(', ')', ' ')) : 0;

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                        {
                            childCategories.Add(new ChildCategory
                            {
                                Name = name,
                                Url = url,
                                ImageUrl = imageUrl,
                                ProductCount = productCount
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Error fetching child categories: {ex.Message}");
                //MessageBox.Show($"Error fetching child categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return childCategories;
        }

        private HtmlDocument GetHtmlDocument(string url, int recursion)
        {
            try
            {
                HtmlDocument document = null;
                if (recursion <= RetryCount)
                {
                    HtmlWeb web = new HtmlWeb();
                    document = web.Load(url);
                }
                return document;
            }
            catch (Exception ex)
            {
                if (recursion < RetryCount)
                {
                    System.Threading.Thread.Sleep(1000);
                    return GetHtmlDocument(url, recursion + 1);
                }
                throw;
            }
        }

        private void UpdateProgressBar(ProgressInfo progressInfo)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ProgressInfo>(UpdateProgressBar), progressInfo);
            }
            else
            {
                if (!string.IsNullOrEmpty(progressInfo.Message))
                {
                    //textBoxContent.AppendText($"{progressInfo.Message}{Environment.NewLine}");
                }

                progressBar.Value = progressInfo.Value;
            }
        }
    }
}
