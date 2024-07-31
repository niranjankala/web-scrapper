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
    public partial class CategoriesCrowlerForm : Form
    {
        private const int RetryCount = 5;

        public CategoriesCrowlerForm()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }

        private void CategoriesCrowlerForm_Load(object sender, EventArgs e)
        {

        }

        private async void ProcessCategoriesButton_Click(object sender, EventArgs e)
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

                var categoryPath = categoryUrls.ToDictionary(x=> x.Split('|')[0], )
                foreach (var item in categoryUrls)
                {
                    if (item.StartsWith("#"))
                        continue;
                    string[] lookup = item.Split('|');
                    string parentCategoryName = lookup[0].ToLower();
                    string parentUrl = lookup[1];

                    string categoryDirectory = Path.Combine(targetDirectory, parentCategoryName);

                    if (!Directory.Exists(categoryDirectory))
                    {
                        Directory.CreateDirectory(categoryDirectory);
                    }

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
                        SortOrder = worksheet.Cells[row, 6].Text
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
    }
}
