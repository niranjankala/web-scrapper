using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;


namespace CrawlyScraper.App
{
    public partial class MergeDataForm : Form
    {
        public MergeDataForm()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {                
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtCategoriesFolderPath.Text = folderDialog.SelectedPath;
                }
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

        private async void btnMergeData_Click(object sender, EventArgs e)
        {
            string categoriesPath = txtCategoriesPath.Text;
            string categoriesFolderPath = txtCategoriesFolderPath.Text;
            string exportPath = txtExportPath.Text;

            if (string.IsNullOrEmpty(categoriesPath) || string.IsNullOrEmpty(categoriesFolderPath) || string.IsNullOrEmpty(exportPath))
            {
                MessageBox.Show("Please fill in all the paths.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            progressBar.Value = 0;
            statusBar.Text = "Processing categories...";

            try
            {
                var categories = await Task.Run(() => ReadCategories(categoriesPath));
                await Task.Run(() => MergeProducts(categoriesFolderPath, exportPath, categories));

                progressBar.Value = 100;
                statusBar.Text = "Merge completed successfully.";
                MessageBox.Show("Products have been successfully merged and saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusBar.Text = "Error occurred during merging.";
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

        private List<Dictionary<string, string>> ReadChildCategoryExcel(string filePath)
        {
            var products = new List<Dictionary<string, string>>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = Math.Min(worksheet.Dimension.Columns, 20); // Get the first 20 columns

                var headers = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    headers.Add(worksheet.Cells[1, col].Text);
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    var product = new Dictionary<string, string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        product[headers[col - 1]] = worksheet.Cells[row, col].Text;
                    }
                    products.Add(product);
                }
            }

            return products;
        }

        private void MergeProducts(string directoryPath, string outputPath, List<Category> categories)
        {
            var mergedProducts = new List<Dictionary<string, string>>();

            foreach (var parentCategory in categories.Select(c => c.ParentId).Distinct())
            {
                var parentDir = Path.Combine(directoryPath, parentCategory);
                if (!Directory.Exists(parentDir))
                    continue;

                foreach (var category in categories.Where(c => c.ParentId == parentCategory))
                {
                    var childFilePath = Path.Combine(parentDir, $"{category.Name}.xlsx");
                    if (!File.Exists(childFilePath))
                        continue;

                    var products = ReadChildCategoryExcel(childFilePath);

                    foreach (var product in products)
                    {
                        var existingProduct = mergedProducts.FirstOrDefault(p => p["Product Name"] == product["Product Name"]);
                        if (existingProduct != null)
                        {
                            existingProduct["Categories"] += $", {parentCategory} > {category.Name}";
                        }
                        else
                        {
                            product["Categories"] = $"{parentCategory} > {category.Name}";
                            mergedProducts.Add(product);
                        }
                    }
                }
            }

            CreateOutputExcel(mergedProducts, outputPath);
        }

        private void CreateOutputExcel(List<Dictionary<string, string>> products, string outputPath)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");
                var headers = products.First().Keys.ToList();

                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    for (int j = 0; j < headers.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1].Value = product[headers[j]];
                    }
                }

                package.SaveAs(new FileInfo(outputPath));
            }
        }
    }

}
