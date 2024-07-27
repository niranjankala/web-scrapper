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

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheets = package.Workbook.Worksheets;

                    // Process each worksheet
                    foreach (var worksheet in worksheets)
                    {
                        EnsureHeaders(worksheet, headers);

                        if (worksheet.Name == "Products")
                        {
                            int rowCount = worksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                // Process price column
                                var priceCell = worksheet.Cells[row, 16];
                                if (priceCell.Text.Contains(","))
                                {
                                    priceCell.Value = priceCell.Text.Replace(",", "");
                                }

                                // Process weight column
                                var weightMatch = weightRegex.Match(worksheet.Cells[row, 21].Text);
                                if (weightMatch.Success)
                                {
                                    string weight = weightMatch.Groups[1].Value;
                                    string unit = weightMatch.Groups[2].Value;

                                    worksheet.Cells[row, 21].Value = int.Parse(weight); // Assuming weight is integer
                                    worksheet.Cells[row, 22].Value = unit; // Directly use the unit text
                                }
                            }
                        }

                        if (worksheet.Name == "ProductSEOKeywords")
                        {
                            RemoveDuplicateKeywords(worksheet);
                        }
                    }

                    // Calculate number of rows per file
                    var rowCounts = worksheets.ToDictionary(ws => ws.Name, ws => ws.Dimension.End.Row);
                    var rowsPerFile = (int)Math.Ceiling((double)rowCounts.Values.Max() / numFiles);

                    foreach (var worksheet in worksheets)
                    {
                        SplitAndSave(worksheet, numFiles, targetFolder, filePath, rowsPerFile);
                    }
                }

                MessageBox.Show("Data splitting completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnsureHeaders(ExcelWorksheet worksheet, Dictionary<string, List<string>> headers)
        {
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

        private void RemoveDuplicateKeywords(ExcelWorksheet worksheet)
        {
            var seenKeywords = new HashSet<string>();
            var rowsToRemove = new List<int>();

            for (int row = worksheet.Dimension.Rows; row >= 2; row--) // Start from bottom to avoid index shift issues
            {
                var keyword = worksheet.Cells[row, 3].Text; // Keyword is in the 3rd column
                if (seenKeywords.Contains(keyword))
                {
                    rowsToRemove.Add(row);
                }
                else
                {
                    seenKeywords.Add(keyword);
                }
            }

            foreach (var row in rowsToRemove)
            {
                worksheet.DeleteRow(row);
            }
        }

        private void SplitAndSave(ExcelWorksheet worksheet, int numFiles, string targetFolder, string filePath, int rowsPerFile)
        {
            int rowCount = worksheet.Dimension.Rows;

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
