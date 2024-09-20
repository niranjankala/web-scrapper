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
            // Initialize categories list
            List<Category> categories = new List<Category>();
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);

            // Paths from UI
            string imagesExportPath = txtImageExportPath.Text;
            string exportPath = txtExportPath.Text;

            int categoryId = 0;
            int parentCategorySortOrder = 0;
            int childCategorySortOrder = 0;

            if (!string.IsNullOrEmpty(imagesExportPath))
            {
                // Step 1: Load category URLs from the text file
                string[] categoryUrls = File.ReadAllLines($"{directory}\\App_Data\\categories.txt");

                var progress = new Progress<ProgressInfo>(UpdateProgressBar);
                var progressReporter = new ProgressReporter(progress);

                // Create a list to manage the tasks for downloading images
                List<Task> downloadTasks = new List<Task>();

                // Iterate through category URLs
                foreach (var item in categoryUrls)
                {
                    if (item.StartsWith("#")) continue;

                    string[] lookup = item.Split('|');
                    string parentCategoryName = lookup[0];
                    string parentUrl = lookup[1];

                    // Step 2: Crawl the parent and its subcategories
                    string categoryImageDirectory = SanitizeDirectoryName(parentCategoryName);
                    string categoryDirectory = Path.Combine(imagesExportPath, categoryImageDirectory);

                    // Create directory for parent category if not exists
                    Directory.CreateDirectory(categoryDirectory);

                    // Add parent category
                    Category parentCategory = new Category()
                    {
                        CategoryId = (++categoryId).ToString(),
                        Name = parentCategoryName,
                        ParentId = "0", // It's a parent category
                        Columns = "1",
                        SortOrder = (++parentCategorySortOrder).ToString(),
                        Top = "1",
                        Layout = "", // Layout for parent category,
                        StoreIds = "0",
                        MetaTitle = parentCategoryName,
                        Status = "true"
                    };
                    categories.Add(parentCategory);
                    childCategorySortOrder = 0;
                    // Fetch subcategories from parent URL
                    var childCategories = await GetChildCategoriesDetail(parentUrl);
                    foreach (var childCategory in childCategories)
                    {
                        try
                        {
                            // Process child category image path
                            string imagePath = "";
                            string imageFileName = GetFileNameFromUrl(childCategory.ImageUrl);
                            string childCategoryImageDirectory = SanitizeDirectoryName(childCategory.Name);
                            string imageExportPath = Path.Join(imagesExportPath, categoryImageDirectory, childCategoryImageDirectory, imageFileName);

                            if (!string.IsNullOrEmpty(childCategory.ImageUrl))
                            {
                                imagePath = $"catalog/default/category/{categoryImageDirectory}/{childCategoryImageDirectory}/{imageFileName}";
                            }

                            // Add child category
                            Category subCategory = new Category()
                            {
                                CategoryId = (++categoryId).ToString(),
                                Name = childCategory.Name,
                                ParentId = parentCategory.CategoryId, // Set parent ID
                                Columns = "0", // Child category
                                SortOrder = (++childCategorySortOrder).ToString(),
                                Top = "0", // Child categories aren't top-level
                                Image = imagePath,
                                Layout = "0:Category", // Child categories have layout
                                StoreIds = "0",
                                MetaTitle = childCategory.Name,
                                Status = "true"
                            };
                            categories.Add(subCategory);

                            // Add image download task
                            downloadTasks.Add(DownloadCategoryImageAsync(childCategory.ImageUrl, imageExportPath));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }

                    // Update progress
                    progressReporter.ReportProgress(new ProgressInfo() { Value = 50, Message = $"Processed category: {parentCategoryName}" });
                }

                // Step 3: Export categories to Excel with three sheets
                progressReporter.ReportProgress(new ProgressInfo() { Value = 75, Message = "Exporting categories to Excel..." });
                await ExportCategoriesToExcelAsync(categories, exportPath, progress);

                // Step 4: Download category images
                progressReporter.ReportProgress(new ProgressInfo() { Value = 85, Message = "Downloading category images..." });
                await Task.WhenAll(downloadTasks);

                // Complete progress
                progressReporter.ReportProgress(new ProgressInfo() { Value = 100, Message = "Category processing completed." });

                MessageBox.Show("Categories processed and exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please specify the target directory for images.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    // Step 1: Create the "Categories" worksheet
                    ExcelWorksheet categoriesSheet = package.Workbook.Worksheets.Add("Categories");

                    // Write headers for the Categories sheet
                    categoriesSheet.Cells[1, 1].Value = "category_id";
                    categoriesSheet.Cells[1, 2].Value = "parent_id";
                    categoriesSheet.Cells[1, 3].Value = "name(en-gb)";
                    categoriesSheet.Cells[1, 4].Value = "top";
                    categoriesSheet.Cells[1, 5].Value = "columns";
                    categoriesSheet.Cells[1, 6].Value = "sort_order";
                    categoriesSheet.Cells[1, 7].Value = "image_name";
                    categoriesSheet.Cells[1, 8].Value = "date_added";
                    categoriesSheet.Cells[1, 9].Value = "date_modified";
                    categoriesSheet.Cells[1, 10].Value = "description(en-gb)";
                    categoriesSheet.Cells[1, 11].Value = "meta_title(en-gb)";
                    categoriesSheet.Cells[1, 12].Value = "meta_description(en-gb)";
                    categoriesSheet.Cells[1, 13].Value = "meta_keywords(en-gb)";
                    categoriesSheet.Cells[1, 14].Value = "store_ids";
                    categoriesSheet.Cells[1, 15].Value = "layout";
                    categoriesSheet.Cells[1, 16].Value = "status";

                    // Write data to the Categories worksheet
                    int rowIndex = 2;
                    int totalCategories = categories.Count;
                    int currentCategory = 0;

                    foreach (var category in categories)
                    {
                        categoriesSheet.Cells[rowIndex, 1].Value = category.CategoryId;
                        categoriesSheet.Cells[rowIndex, 2].Value = category.ParentId;
                        categoriesSheet.Cells[rowIndex, 3].Value = category.Name; // Assuming name(en-gb)
                        categoriesSheet.Cells[rowIndex, 4].Value = category.Top;
                        categoriesSheet.Cells[rowIndex, 5].Value = category.Columns;
                        categoriesSheet.Cells[rowIndex, 6].Value = category.SortOrder;
                        categoriesSheet.Cells[rowIndex, 7].Value = category.Image; // Updated image path
                        categoriesSheet.Cells[rowIndex, 8].Value = category.DateAdded; // Date added (if needed)
                        categoriesSheet.Cells[rowIndex, 9].Value = category.DateModified; // Date modified (if needed)
                        categoriesSheet.Cells[rowIndex, 10].Value = category.Description; // Description (if needed)
                        categoriesSheet.Cells[rowIndex, 11].Value = category.Name; // Meta title (same as name)
                        categoriesSheet.Cells[rowIndex, 12].Value = category.MetaDescription; // Meta description (if needed)
                        categoriesSheet.Cells[rowIndex, 13].Value = category.MetaKeywords; // Meta keywords (if needed)
                        categoriesSheet.Cells[rowIndex, 14].Value = "0"; // store_ids is always 0
                        categoriesSheet.Cells[rowIndex, 15].Value = category.Layout; // Layout for parent: "0:Category", others: empty
                        categoriesSheet.Cells[rowIndex, 16].Value = category.Status; // Status (true)

                        rowIndex++;

                        // Update progress
                        currentCategory++;
                        int progressPercentage = 80 + (int)((double)currentCategory / totalCategories * 20);
                        progress.Report(new ProgressInfo(progressPercentage, $"Exporting category {category.Name} ({currentCategory}/{totalCategories})..."));

                        await Task.Delay(10); // To keep UI responsive
                    }

                    // Auto fit columns
                    categoriesSheet.Cells.AutoFitColumns();

                    // Step 2: Create the "CategoryFilters" worksheet (header only, no data)
                    ExcelWorksheet filtersSheet = package.Workbook.Worksheets.Add("CategoryFilters");

                    // Write headers for the CategoryFilters sheet
                    filtersSheet.Cells[1, 1].Value = "category_id";
                    filtersSheet.Cells[1, 2].Value = "filter_group";
                    filtersSheet.Cells[1, 3].Value = "filter";

                    // Step 3: Create the "CategorySEOKeywords" worksheet
                    ExcelWorksheet seoKeywordsSheet = package.Workbook.Worksheets.Add("CategorySEOKeywords");

                    // Write headers for the SEO Keywords sheet
                    seoKeywordsSheet.Cells[1, 1].Value = "category_id";
                    seoKeywordsSheet.Cells[1, 2].Value = "store_id";
                    seoKeywordsSheet.Cells[1, 3].Value = "keyword(en-gb)";

                    // Write data for SEO Keywords sheet
                    rowIndex = 2; // Reset row index for the new sheet
                    foreach (var category in categories)
                    {
                        seoKeywordsSheet.Cells[rowIndex, 1].Value = category.CategoryId;
                        seoKeywordsSheet.Cells[rowIndex, 2].Value = "0"; // store_id is always 0
                        seoKeywordsSheet.Cells[rowIndex, 3].Value = category.Name; // Use category name for the keyword(en-gb)
                        rowIndex++;
                    }

                    // Auto fit columns for SEO sheet
                    seoKeywordsSheet.Cells.AutoFitColumns();

                    // Save the file to the export path
                    FileInfo fileInfo = new FileInfo(exportPath);
                    await package.SaveAsAsync(fileInfo);

                    // Report completion progress
                    progress.Report(new ProgressInfo(100, "Export completed successfully."));
                }
            }
            catch (Exception ex)
            {
                progress.Report(new ProgressInfo(100, $"Error during export: {ex.Message}"));
                throw; // Rethrow for higher-level error handling
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

        private string SanitizeDirectoryName(string name)
        {
            return name.Replace("&amp;", "").Replace("&", "").Replace("  ", " ").Replace(" ", "-").ToLower();
        }

    }
}
