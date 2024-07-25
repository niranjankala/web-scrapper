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
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Eventing.Reader;


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
            var additionalImages = new Dictionary<string, List<(string, string)>>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheetProducts = package.Workbook.Worksheets[0];
                if (package.Workbook.Worksheets.Count > 1)
                {
                    var worksheetAdditionalImages = package.Workbook.Worksheets[1];

                    for (int row = 2; row <= worksheetAdditionalImages.Dimension.Rows; row++)
                    {
                        string key = worksheetAdditionalImages.Cells[row, 1].Text;
                        string image = worksheetAdditionalImages.Cells[row, 2].Text;
                        string order = worksheetAdditionalImages.Cells[row, 3].Text;
                        if (additionalImages.ContainsKey(key))
                        {
                            additionalImages[key].Add((image, order));
                        }
                        else
                        {
                            additionalImages.Add(key, new List<(string, string)>() { (image, order) });
                        }
                    }
                }

                int rowCount = worksheetProducts.Dimension.Rows;
                int colCount = Math.Min(worksheetProducts.Dimension.Columns, 20); // Get the first 20 columns

                var headers = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    headers.Add(worksheetProducts.Cells[1, col].Text);
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    var product = new Dictionary<string, string>();
                    string productId = string.Empty;
                    for (int col = 1; col <= colCount; col++)
                    {
                        string cellValue = worksheetProducts.Cells[row, col].Text;
                        if (col == 1 && headers[col - 1] == "product_id")
                        {
                            productId = cellValue;
                        }
                        if (col == 2 && !string.IsNullOrEmpty(cellValue))
                        {
                            cellValue = cellValue.Replace("&amp;", "&");
                        }

                        product[headers[col - 1]] = cellValue;
                    }

                    if (!string.IsNullOrEmpty(productId) && additionalImages.ContainsKey(product["product_id"]))
                    {
                        product.Add("AdditionalImages", string.Join("|", additionalImages[productId].Select(r => $"{r.Item1}!{r.Item2}")));
                    }

                    products.Add(product);
                }
            }

            return products;
        }

        private void MergeProducts(string directoryPath, string outputPath, List<Category> categories, ProgressReporter progressReporter)
        {
            var mergedProducts = new List<Dictionary<string, string>>();
            var additionalImages = new Dictionary<string, List<(string, string)>>();
            var parentCategories = categories.Where(c => c.ParentId == "0");
            int productId = 1;
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
                        var existingProduct = mergedProducts.FirstOrDefault(p => p["Product Name"] == product["Product Name"] && p["Product Link"] == product["Product Link"]);
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
                            if (product.ContainsKey("product_id"))
                            {
                                product["product_id"] = productId.ToString();
                            }
                            productId++;
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
                var worksheetAdditionalImages = package.Workbook.Worksheets.Add("AdditionalImages");
                var worksheetProductSEOKeywords = package.Workbook.Worksheets.Add("ProductSEOKeywords");

                // Define column mappings
                Dictionary<string, string> columnMapping = new Dictionary<string, string>()
        {
            {"product_id", "product_id" },
            { "name(en-gb)", "Product Name" },
            { "categories", "Categories" },
            { "sku", "SKU" },
            { "upc", "" },
            { "ean", "" },
            { "jan", "" },
            { "isbn", "" },
            { "mpn", "" },
            { "location", "Country of Origin" },
            { "quantity", "100" },
            { "model", "Model No" },
            { "manufacturer", "Brand Name" },
            { "image_name", "Product Image" },
            { "shipping", "Yes" },
            { "price", "GST Inclusive Price" },
            { "points", "10" },
            { "date_added", "date_added" },
            { "date_modified", "date_modified" },
            { "date_available", "date_available" },
            { "weight", "Weight" },
            { "weight_unit", "kg" },
            { "length", "Length" },
            { "width", "0" },
            { "height", "Height" },
            { "length_unit", "cm" },
            { "status", "true" },
            { "tax_class_id", "0" },
            { "description(en-gb)", "Description" },
            { "meta_title(en-gb)", "Product Name" },
            { "meta_description(en-gb)", "" },
            { "meta_keywords(en-gb)", "" },
            { "stock_status_id", "6" },
            { "store_ids", "" },
            { "layout", "0:Category" },
            { "related_ids", "" },
            { "tags(en-gb)", "" },
            { "sort_order", "1" },
            { "subtract", "true" },
            { "minimum", "1" }
        };

                var headers = columnMapping.Keys.ToList();

                // Set headers in the Excel worksheetProducts
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Set headers in the Excel worksheetAdditionalImages
                worksheetAdditionalImages.Cells[1, 1].Value = "product_id";
                worksheetAdditionalImages.Cells[1, 2].Value = "image";
                worksheetAdditionalImages.Cells[1, 3].Value = "sort_order";

                // Set headers in the Excel worksheetProductSEOKeywords
                worksheetProductSEOKeywords.Cells[1, 1].Value = "product_id";
                worksheetProductSEOKeywords.Cells[1, 2].Value = "store_id";
                worksheetProductSEOKeywords.Cells[1, 3].Value = "keyword(en-gb)";

                int imagesRowIndex = 2;
                // Populate data rows
                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    string productId = string.Empty;
                    string productName = string.Empty;
                    for (int j = 0; j < headers.Count; j++)
                    {
                        string column = columnMapping[headers[j]];
                        string value = string.Empty;

                        if (product.ContainsKey(column))
                        {
                            value = product[column];
                        }
                        else
                        {
                            // Set default value for "location" if not present
                            if (column == "Country of Origin")
                            {
                                value = "India";
                            }
                            else if (column == "date_available" || column == "date_modified" || column == "date_added")
                            {
                                value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (column == "Height" || column == "Weight")
                            {
                                value = "0";
                            }
                            else
                            {
                                value = columnMapping[headers[j]];
                            }
                        }

                        if (column == "product_id")
                            productId = value;
                        if (column == "Product Name")
                            productName = value;

                        worksheet.Cells[i + 2, j + 1].Value = value;
                    }

                    if (!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(productName))
                    {
                        worksheetProductSEOKeywords.Cells[i + 2, 1].Value = productId;
                        worksheetProductSEOKeywords.Cells[i + 2, 2].Value = "0";
                        worksheetProductSEOKeywords.Cells[i + 2, 3].Value = productName;
                    }
                    if (!string.IsNullOrEmpty(productId) && product.ContainsKey("AdditionalImages"))
                    {
                        foreach (var imgRow in product["AdditionalImages"].Split("|"))
                        {
                            int colIndex = 2;
                            worksheetAdditionalImages.Cells[imagesRowIndex, 1].Value = productId;
                            foreach (var value in imgRow.Split("!"))
                            {
                                worksheetAdditionalImages.Cells[imagesRowIndex, colIndex].Value = value;
                                colIndex++;
                            }
                            imagesRowIndex++;
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
