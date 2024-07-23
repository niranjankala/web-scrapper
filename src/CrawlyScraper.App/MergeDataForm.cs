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

using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using System.Reflection;
using Microsoft.FSharp.Data.UnitSystems.SI.UnitNames;
using OfficeOpenXml.Packaging.Ionic.Zlib;


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
            var progress = new Progress<ProgressInfo>(UpdateProgressBar);
            var progressReporter = new ProgressReporter(progress);

            progressReporter.ReportProgress(new ProgressInfo(0, "Processing categories..."));
            try
            {
                progressReporter.ReportProgress(new ProgressInfo(5, "Reading categories..."));
                var categories = await Task.Run(() => ReadCategories(categoriesPath));
                progressReporter.ReportProgress(new ProgressInfo(10, "Reading categories completed successfully."));
                await Task.Run(() => MergeProducts(categoriesFolderPath, exportPath, categories, progressReporter));
                progressReporter.ReportProgress(new ProgressInfo(100, "Merge completed successfully."));               
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
                        string cellValue = worksheet.Cells[row, col].Text;
                        if (col == 1 && !string.IsNullOrEmpty(cellValue))
                        {
                            cellValue = cellValue.Replace("&amp;", "&");
                        }

                        product[headers[col - 1]] = cellValue;
                    }
                    products.Add(product);
                }
            }

            return products;
        }

        private void MergeProducts(string directoryPath, string outputPath, List<Category> categories, ProgressReporter progressReporter)
        {
            var mergedProducts = new List<Dictionary<string, string>>();
            var parentCategories = categories.Where(c => c.ParentId == "0");

            foreach (var parentCategory in parentCategories)
            {
                var parentDir = Path.Combine(directoryPath, parentCategory.Name);
                if (!Directory.Exists(parentDir))
                    continue;
                int categoryCounter = 1;
                foreach (var category in categories.Where(c => c.ParentId == parentCategory.CategoryId))
                {
                    var childFilePath = Path.Combine(parentDir, $"{category.Name.Trim()}.xlsx");
                    if (!File.Exists(childFilePath))
                    {
                        childFilePath = Path.Combine(parentDir, $"{category.Name.Trim().Replace("&", "&amp;")}.xlsx");
                        if (!File.Exists(childFilePath))
                            continue;
                    }
                    
                    

                    var products = ReadChildCategoryExcel(childFilePath);

                    foreach (var product in products)
                    {                        
                        var existingProduct = mergedProducts.FirstOrDefault(p => p[""] == product[""] && p["Product Link"] == product["Product Link"]);
                        if (existingProduct != null)
                        {
                            string[] existingCategories = existingProduct["Categories"].Split(",");
                            if (!existingCategories.Contains($"{parentCategory.CategoryId}")) 
                            {
                                existingProduct["Categories"] += $",{parentCategory.CategoryId}";
                            }
                            if (!existingCategories.Contains($",{category.CategoryId}"))
                            {
                                existingProduct["Categories"] += $",{category.CategoryId}";
                            }
                        }
                        else
                        {
                            product["Categories"] = $"{parentCategory.CategoryId},{category.CategoryId}";
                            mergedProducts.Add(product);
                        }
                    }
                    var progress = 10 + (categoryCounter / parentCategories.Count()) * 70;
                    progressReporter.ReportProgress(new ProgressInfo(progress, $"Processed Category:{category.Name}."));
                }
                

            }

            CreateOutputExcel(mergedProducts, outputPath);
        }

		private void CreateOutputExcel(List<Dictionary<string, string>> products, string outputPath)
		{
			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Products");

				// Define column mappings
				Dictionary<string, string> columnMapping = new Dictionary<string, string>()
		{
			{ "name(en-gb)", "Product Name" }, { "categories", "Categories" },
			{ "sku", "SKU" }, { "upc", "" },
			{ "ean", "" }, { "jan", "" },
			{ "isbn", "" }, { "mpn", "" },
			{ "location", "" }, { "quantity", "" },
			{ "model", "Model No" }, { "manufacturer", "" },
			{ "image_name", "Product Image" }, { "shipping", "" },
			{ "price", "Product Price" }, { "points", "" },
			{ "date_added", "" }, { "status", "" },
			{ "tax_class_id", "" }, { "description(en-gb)", "" },
			{ "meta_title(en-gb)", "" }, { "meta_description(en-gb)", "" },
			{ "meta_keywords(en-gb)", "" }, { "stock_status_id", "" },
			{ "store_ids", "" }, { "layout", "" },
			{ "related_ids", "" }, { "tags(en-gb)", "" },
			{ "sort_order", "" }, { "subtract", "" },
			{ "minimum", "" }
		};

				var headers = columnMapping.Keys.ToList();

				// Set headers in the Excel worksheet
				for (int i = 0; i < headers.Count; i++)
				{
					worksheet.Cells[1, i + 1].Value = columnMapping[headers[i]];
				}

				// Populate data rows
				for (int i = 0; i < products.Count; i++)
				{
					var product = products[i];
					for (int j = 0; j < headers.Count; j++)
					{
						if (product.ContainsKey(headers[j]))
						{
							worksheet.Cells[i + 2, j + 1].Value = product[headers[j]];
						}
						else
						{
							worksheet.Cells[i + 2, j + 1].Value = "";
						}
					}
				}

				// Save Excel package to file
				package.SaveAs(new FileInfo(outputPath));
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
                statusBar.Text += $"{progressInfo.Message}{Environment.NewLine}";
                progressBar.Value = progressInfo.Value;
            }
        }
    }

}
