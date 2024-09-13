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
        private const string crawlingWebsiteUrl = "https://www.industrybuying.com";
        private const string noImageUrl = "/static/images/image_not_available.jpg";
        public CategoriesCrawlerForm()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
        private void CategoriesCrawlerForm_Load(object sender, EventArgs e)
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

                    string categoryImageDirectory = parentCategoryName.Replace("&amp;", "").Replace("&", "").Replace("  ", " ").Replace(" ", "-").ToLower();

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
                            string childCategoryImageDirectory = childCategory.Name.Replace("&amp;", "").Replace("&", "").Replace("  ", " ").Replace(" ", "-").ToLower();
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
        private async void UpdateCategoriesButton_Click(object sender, EventArgs e)
        {
            string categoriesPath = txtCategoriesPath.Text;
            string exportPath = txtExportPath.Text;
            string targetDirectory = txtImageExportPath.Text; // Assuming this is the image export path

            if (string.IsNullOrEmpty(categoriesPath) || string.IsNullOrEmpty(exportPath) || string.IsNullOrEmpty(targetDirectory))
            {
                MessageBox.Show("Please fill in all the paths.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var progress = new Progress<ProgressInfo>(UpdateProgressBar);
            var progressReporter = new ProgressReporter(progress);

            progressReporter.ReportProgress(new ProgressInfo(0, "Processing categories..."));

            try
            {
                // Step 1: Read all existing categories and their columns from the first sheet
                progressReporter.ReportProgress(new ProgressInfo(10, "Reading existing categories..."));
                var existingCategories = await Task.Run(() => ReadCategories(categoriesPath));

                // Load category URLs from the text file
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var directory = System.IO.Path.GetDirectoryName(path);
                string[] categoryUrls = File.ReadAllLines($"{directory}\\App_Data\\categories.txt");

                // Step 2: Iterate through the categories fetched from the text file
                progressReporter.ReportProgress(new ProgressInfo(20, "Iterating through fetched categories..."));
                List<Task> downloadTasks = new List<Task>();

                foreach (var item in categoryUrls)
                {
                    if (item.StartsWith("#"))
                        continue;

                    string[] lookup = item.Split('|');
                    string parentCategoryName = lookup[0].ToLower();
                    string parentUrl = lookup[1];

                    // Step 3: Find matching category from existing categories
                    var matchingCategory = existingCategories.FirstOrDefault(c => c.Name.ToLower() == parentCategoryName);
                    if (matchingCategory == null)
                    {
                        // If no match found, skip or add logic to create a new category if necessary
                        continue;
                    }

                    string parentCategoryImageDirectory = parentCategoryName.Replace("&amp;", "").Replace("&", "").Replace("  ", " ").Replace(" ", "-").ToLower();
                    string categoryDirectory = Path.Combine(targetDirectory, parentCategoryImageDirectory);

                    // Step 4: Maintain list of subcategories for the parent category
                    progressReporter.ReportProgress(new ProgressInfo(30, $"Processing parent category: {parentCategoryName}"));
                    var childCategoriesFromWeb = await GetChildCategoriesDetail(parentUrl);
                    var childCategories = existingCategories.Where(c => c.ParentId == matchingCategory.CategoryId);
                    // Step 5: Crawl and update subcategories
                    foreach (var categoryItem in childCategoriesFromWeb)
                    {
                        try
                        {
                            string imagePath = "";
                            string imageFileName = GetFileNameFromUrl(categoryItem.ImageUrl);
                            string imageExportPath = Path.Join(targetDirectory, parentCategoryImageDirectory, imageFileName);

                            if (!string.IsNullOrEmpty(categoryItem.ImageUrl))
                            {
                                imagePath = $"catalog/default/category/{parentCategoryImageDirectory}/{imageFileName}";
                            }

                            // Step 6: Download category images
                            downloadTasks.Add(DownloadCategoryImageAsync(categoryItem.ImageUrl, imageExportPath));

                            // Update existing category with new image path
                            var subCategory = childCategories.FirstOrDefault(c => (c.Name.ToLower() == categoryItem.Name.ToLower())
                            || (c.Name.ToLower() == categoryItem.Name.ToLower().Replace("&amp;", "&"))
                            || (c.Name.ToLower().Replace("&amp;", "&") == categoryItem.Name.ToLower()));
                            if (subCategory != null)
                            {
                                subCategory.Image = imagePath;
                            }
                            else
                            {
                                //Category newSubCategory = new Category()
                                //{
                                //    CategoryId = (existingCategories.Max(c => Int64.Parse(c.CategoryId))+1).ToString(),
                                //    Name = categoryItem.Name,
                                //    ParentId = matchingCategory.CategoryId,
                                //     Image = imagePath,
                                //      MetaTitle = categoryItem.Name,
                                //       SortOrder = (childCategories.Max(c=> Int32.Parse(c.SortOrder))+1).ToString(),
                                //       Top = "true",
                                //       Columns = "1",
                                //       StoreIds = "0",
                                //       Layout = "0:Category",
                                //       Status = "true"

                                //};
                                //existingCategories.Add(newSubCategory);

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing subcategory: {categoryItem.Name}, Error: {ex.Message}");
                        }
                    }
                }

                // Wait for all image download tasks to complete
                progressReporter.ReportProgress(new ProgressInfo(60, "Downloading images..."));
                await Task.WhenAll(downloadTasks);

                // Step 7: Export updated categories back to Excel
                progressReporter.ReportProgress(new ProgressInfo(80, "Exporting updated categories to Excel..."));
                await ExportCategoriesToExcelAsync(existingCategories, exportPath, progress);

                progressReporter.ReportProgress(new ProgressInfo(100, "Categories update process completed."));
                MessageBox.Show("Categories updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                progressReporter.ReportProgress(new ProgressInfo(100, "Error occurred during category update."));
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ExportCategoriesToExcelAsync(List<Category> categories, string exportPath, IProgress<ProgressInfo> progress)
        {
            try
            {
                // Initialize EPPlus package
                progress.Report(new ProgressInfo(80, "Preparing export..."));

                using (var package = new ExcelPackage())
                {
                    // Create a new worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Categories");

                    // Write headers matching the imported Excel file structure
                    worksheet.Cells[1, 1].Value = "category_id";
                    worksheet.Cells[1, 2].Value = "parent_id";
                    worksheet.Cells[1, 3].Value = "name(en-gb)";
                    worksheet.Cells[1, 4].Value = "top";
                    worksheet.Cells[1, 5].Value = "columns";
                    worksheet.Cells[1, 6].Value = "sort_order";
                    worksheet.Cells[1, 7].Value = "image_name"; // This is the updated image path
                    worksheet.Cells[1, 8].Value = "date_added";
                    worksheet.Cells[1, 9].Value = "date_modified";
                    worksheet.Cells[1, 10].Value = "description(en-gb)";
                    worksheet.Cells[1, 11].Value = "meta_title(en-gb)";
                    worksheet.Cells[1, 12].Value = "meta_description(en-gb)";
                    worksheet.Cells[1, 13].Value = "meta_keywords(en-gb)";
                    worksheet.Cells[1, 14].Value = "store_ids";
                    worksheet.Cells[1, 15].Value = "layout";
                    worksheet.Cells[1, 16].Value = "status";

                    // Set up the row index for the first data row
                    int rowIndex = 2;

                    // Total categories count for progress tracking
                    int totalCategories = categories.Count;
                    int currentCategory = 0;

                    // Write data to the worksheet, focusing on updated image_name (skip unnecessary fields)
                    foreach (var category in categories)
                    {
                        bool isParentCategory = string.IsNullOrEmpty(category.ParentId); // Assuming parent categories have an empty ParentId

                        worksheet.Cells[rowIndex, 1].Value = category.CategoryId;
                        worksheet.Cells[rowIndex, 2].Value = category.ParentId;
                        worksheet.Cells[rowIndex, 3].Value = category.Name; // Assuming this maps to name(en-gb)
                        worksheet.Cells[rowIndex, 4].Value = category.Top;
                        worksheet.Cells[rowIndex, 5].Value = category.Columns;
                        worksheet.Cells[rowIndex, 6].Value = category.SortOrder;
                        worksheet.Cells[rowIndex, 7].Value = category.Image; // This is the updated image path
                        worksheet.Cells[rowIndex, 8].Value = category.DateAdded; // date_added
                        worksheet.Cells[rowIndex, 9].Value = category.DateModified; // date_modified
                        worksheet.Cells[rowIndex, 10].Value = category.Description; // description(en-gb) - leave empty or populate as needed
                        worksheet.Cells[rowIndex, 11].Value = category.Name; // meta_title(en-gb) - set same as name(en-gb)
                        worksheet.Cells[rowIndex, 12].Value = category.MetaDescription; // meta_description(en-gb) - leave empty or populate as needed
                        worksheet.Cells[rowIndex, 13].Value = category.MetaKeywords; // meta_keywords(en-gb) - leave empty or populate as needed
                        worksheet.Cells[rowIndex, 14].Value = string.IsNullOrEmpty(category.StoreIds) ? "0" : category.StoreIds; // store_ids - set to 0
                        worksheet.Cells[rowIndex, 15].Value = category.Layout; //isParentCategory ? "0:Category" : ""; // layout - "0:Category" for parent, empty for others
                        worksheet.Cells[rowIndex, 16].Value = category.Status; //"true"; // status - set to true

                        rowIndex++;

                        // Update progress as data is written
                        currentCategory++;
                        int progressPercentage = 80 + (int)((double)currentCategory / totalCategories * 20);
                        progress.Report(new ProgressInfo(progressPercentage, $"Exporting category {category.Name} ({currentCategory}/{totalCategories})..."));

                        // Simulate slight delay to ensure UI remains responsive (for demo purposes)
                        await Task.Delay(10);
                    }

                    // Adjust columns to fit the data
                    worksheet.Cells.AutoFitColumns();

                    // Save the file to the export path
                    FileInfo fileInfo = new FileInfo(exportPath);
                    await package.SaveAsAsync(fileInfo);

                    progress.Report(new ProgressInfo(100, "Export completed successfully."));
                }
            }
            catch (Exception ex)
            {
                progress.Report(new ProgressInfo(100, $"Error during export: {ex.Message}"));
                throw; // Rethrow to handle it in the calling method if necessary
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
                if (!File.Exists(exportPath))
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

        private List<Category> ReadCategories(string filePath)
        {
            var categories = new List<Category>();

            try
            {
                FileInfo fileInfo = new FileInfo(filePath);

                using (var package = new ExcelPackage(fileInfo))
                {
                    // Get the first worksheet
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    int columnCount = worksheet.Dimension.Columns;

                    // Read header row (assuming headers are in the first row)
                    var headerRow = new Dictionary<int, string>();
                    for (int col = 1; col <= columnCount; col++)
                    {
                        headerRow[col] = worksheet.Cells[1, col].Text.Trim();
                    }

                    // Read data rows
                    for (int row = 2; row <= rowCount; row++) // Skip header row
                    {
                        var category = new Category
                        {
                            CategoryId = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "category_id").Key].Text.Trim(),
                            ParentId = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "parent_id").Key].Text.Trim(),
                            Name = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "name(en-gb)").Key].Text.Trim(),
                            Top = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "top").Key].Text.Trim(),
                            Columns = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "columns").Key].Text.Trim(),
                            SortOrder = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "sort_order").Key].Text.Trim(),
                            Image = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "image_name").Key].Text.Trim(),
                            DateAdded = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "date_added").Key].Text.Trim(),
                            DateModified = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "date_modified").Key].Text.Trim(),
                            Description = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "description(en-gb)").Key].Text.Trim(),
                            MetaTitle = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "meta_title(en-gb)").Key].Text.Trim(),
                            MetaDescription = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "meta_description(en-gb)").Key].Text.Trim(),
                            MetaKeywords = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "meta_keywords(en-gb)").Key].Text.Trim(),
                            StoreIds = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "store_ids").Key].Text.Trim(),
                            Layout = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "layout").Key].Text.Trim(),
                            Status = worksheet.Cells[row, headerRow.FirstOrDefault(x => x.Value == "status").Key].Text.Trim()
                        };

                        categories.Add(category);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"Error reading categories: {ex.Message}");
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

                        if (imageUrl == "N/A")
                            imageUrl = noImageUrl;

                        if (imageUrl.StartsWith(@"//"))
                        {
                            imageUrl = imageUrl.Replace(@"//", "https://");
                        }
                        else if (imageUrl.StartsWith(@"/"))
                        {
                            imageUrl = $"{crawlingWebsiteUrl}{imageUrl}";
                        }


                        int productCount = productCountNode != null ? int.Parse(productCountNode.InnerText.Trim('(', ')', ' ')) : 0;

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                        {
                            childCategories.Add(new ChildCategory
                            {
                                Name = name.Replace("&amp;", "&"),
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
