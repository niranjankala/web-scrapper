using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace CrawlyScraper.App
{
    public partial class SplitDataForm : Form
    {
        public SplitDataForm()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void buttonBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx",
                Title = "Select Excel File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = openFileDialog.FileName;
            }
        }

        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxTargetFolder.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void buttonSplitData_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxNumFiles.Text, out int numFiles) &&
                !string.IsNullOrEmpty(textBoxFilePath.Text) &&
                !string.IsNullOrEmpty(textBoxTargetFolder.Text))
            {
                statusLabel.Text = "Processing...";
                SplitExcelData(textBoxFilePath.Text, numFiles, textBoxTargetFolder.Text);
                progressBar.Value = 100;
                statusLabel.Text = "Completed!";
            }
            else
            {
                MessageBox.Show("Please enter valid inputs.");
            }
        }

        private void SplitExcelData(string filePath, int numFiles, string targetFolder)
        {
            Regex weightRegex = new Regex(@"(\d+)([a-zA-Z]+)");

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheets = package.Workbook.Worksheets;

                    foreach (var worksheet in worksheets)
                    {
                        EnsureHeaders(worksheet);

                        if (worksheet.Name == "ProductSEOKeywords")
                        {
                            RemoveInvalidAndDuplicateRows(worksheet, package.Workbook.Worksheets["Products"]);
                        }

                        var productsWorksheet = package.Workbook.Worksheets["Products"];
                        if (productsWorksheet != null)
                        {
                            int rowCount = productsWorksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                // Remove commas from the price column
                                string priceText = productsWorksheet.Cells[row, 16].Text.Replace(",", "");
                                if (decimal.TryParse(priceText, out decimal price))
                                {
                                    productsWorksheet.Cells[row, 16].Value = price;
                                }

                                // Handle weight and unit
                                var weightMatch = weightRegex.Match(productsWorksheet.Cells[row, 20].Text);
                                if (weightMatch.Success)
                                {
                                    string weight = weightMatch.Groups[1].Value;
                                    string unit = weightMatch.Groups[2].Value;

                                    productsWorksheet.Cells[row, 20].Value = int.Parse(weight); // Assuming weight is integer
                                    productsWorksheet.Cells[row, 21].Value = unit; // Directly use the unit text
                                }
                            }
                        }

                        SplitAndSave(worksheet, numFiles, targetFolder, filePath);
                    }
                }

                MessageBox.Show("Data splitting completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnsureHeaders(ExcelWorksheet worksheet)
        {
            var headers = new Dictionary<string, List<string>>
            {
                { "Products", new List<string> { "product_id", "name(en-gb)", "categories", "sku", "upc", "ean", "jan", "isbn", "mpn", "location", "quantity", "model", "manufacturer", "image_name", "shipping", "price", "points", "date_added", "date_modified", "date_available", "weight", "weight_unit", "length", "width", "height", "length_unit", "status", "tax_class_id", "description(en-gb)", "meta_title(en-gb)", "meta_description(en-gb)", "meta_keywords(en-gb)", "stock_status_id", "store_ids", "layout", "related_ids", "tags(en-gb)", "sort_order", "subtract", "minimum" } },
                { "AdditionalImages", new List<string> { "product_id", "image", "sort_order" } },
                { "Specials", new List<string> { "product_id", "customer_group", "priority", "price", "date_start", "date_end" } },
                { "Discounts", new List<string> { "product_id", "customer_group", "quantity", "priority", "price", "date_start", "date_end" } },
                { "Rewards", new List<string> { "product_id", "customer_group", "points" } },
                { "ProductOptions", new List<string> { "product_id", "option", "default_option_value", "required" } },
                { "ProductOptionValues", new List<string> { "product_id", "option", "option_value", "quantity", "subtract", "price", "price_prefix", "points", "points_prefix", "weight", "weight_prefix" } },
                { "ProductAttributes", new List<string> { "product_id", "attribute_group", "attribute", "text(en-gb)" } },
                { "ProductFilters", new List<string> { "product_id", "filter_group", "filter" } },
                { "ProductSEOKeywords", new List<string> { "product_id", "store_id", "keyword(en-gb)" } }
            };

            if (headers.ContainsKey(worksheet.Name))
            {
                var headerList = headers[worksheet.Name];
                for (int col = 1; col <= headerList.Count; col++)
                {
                    if (worksheet.Cells[1, col].Text != headerList[col - 1])
                    {
                        worksheet.Cells[1, col].Value = headerList[col - 1];
                    }
                }
            }
        }

        private void RemoveInvalidAndDuplicateRows(ExcelWorksheet worksheet, ExcelWorksheet productsWorksheet)
        {
            var validProductIds = new HashSet<string>();
            for (int row = 2; row <= productsWorksheet.Dimension.Rows; row++)
            {
                validProductIds.Add(productsWorksheet.Cells[row, 1].Text);
            }

            var seenRows = new HashSet<string>();
            for (int row = worksheet.Dimension.Rows; row >= 2; row--)
            {
                var productId = worksheet.Cells[row, 1].Text;
                var rowValues = string.Join(",", worksheet.Cells[row, 1, row, worksheet.Dimension.Columns].Select(c => c.Text));

                if (!validProductIds.Contains(productId) || seenRows.Contains(rowValues))
                {
                    worksheet.DeleteRow(row);
                }
                else
                {
                    seenRows.Add(rowValues);
                }
            }
        }

        private void SplitAndSave(ExcelWorksheet worksheet, int numFiles, string targetFolder, string filePath)
        {
            int rowCount = worksheet.Dimension.Rows;
            int rowsPerFile = (rowCount - 1) / numFiles; // excluding header row

            for (int fileIndex = 0; fileIndex < numFiles; fileIndex++)
            {
                var newPackage = new ExcelPackage();
                var newWorksheet = newPackage.Workbook.Worksheets.Add(worksheet.Name);

                // Copy headers
                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    newWorksheet.Cells[1, col].Value = worksheet.Cells[1, col].Value;
                }

                // Copy data
                int startRow = (fileIndex * rowsPerFile) + 2;
                int endRow = (fileIndex == numFiles - 1) ? rowCount : startRow + rowsPerFile - 1;

                for (int row = startRow; row <= endRow; row++)
                {
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        newWorksheet.Cells[row - startRow + 2, col].Value = worksheet.Cells[row, col].Value;
                    }
                }

                // Save new file
                var newFilePath = Path.Combine(targetFolder, $"{Path.GetFileNameWithoutExtension(filePath)}_part{fileIndex + 1}.xlsx");
                newPackage.SaveAs(new FileInfo(newFilePath));
            }
        }
    }
}
