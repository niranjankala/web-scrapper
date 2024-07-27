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
            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Get the worksheets from the original file
                    var worksheets = package.Workbook.Worksheets;

                    // Process each worksheet
                    foreach (var worksheet in worksheets)
                    {
                        if (worksheet.Name == "ProductSEOKeywords")
                        {
                            RemoveDuplicates(worksheet);
                        }
                    }

                    // Define headers for all sheets
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

                    // Determine the total number of rows for each sheet
                    var rowCounts = new Dictionary<string, int>();

                    foreach (var worksheet in worksheets)
                    {
                        rowCounts[worksheet.Name] = worksheet.Dimension.End.Row;
                    }

                    // Calculate the number of rows per file
                    var rowsPerFile = (int)Math.Ceiling((double)rowCounts.Values.Max() / numFiles);

                    for (int fileIndex = 0; fileIndex < numFiles; fileIndex++)
                    {
                        var newFilePath = Path.Combine(targetFolder, $"Part_{fileIndex + 1}.xlsx");

                        using (var newPackage = new ExcelPackage())
                        {
                            foreach (var worksheet in worksheets)
                            {
                                var newWorksheet = newPackage.Workbook.Worksheets.Add(worksheet.Name);

                                // Add headers
                                if (headers.ContainsKey(worksheet.Name))
                                {
                                    var headerList = headers[worksheet.Name];
                                    for (int col = 1; col <= headerList.Count; col++)
                                    {
                                        newWorksheet.Cells[1, col].Value = headerList[col - 1];
                                    }
                                }

                                // Add data rows
                                var startRow = fileIndex * rowsPerFile + 2; // Start row (skip header)
                                var endRow = Math.Min(startRow + rowsPerFile - 1, rowCounts[worksheet.Name]);

                                for (int row = startRow; row <= endRow; row++)
                                {
                                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                                    {
                                        newWorksheet.Cells[row - startRow + 2, col].Value = worksheet.Cells[row, col].Text;
                                    }
                                }
                            }

                            // Save the new file
                            File.WriteAllBytes(newFilePath, newPackage.GetAsByteArray());
                        }
                    }
                }

                MessageBox.Show("Data splitting completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveDuplicates(ExcelWorksheet worksheet)
        {
            // Dictionary to keep track of unique rows based on the combination of all columns
            var uniqueRows = new HashSet<string>();
            var rowCount = worksheet.Dimension.End.Row;
            var columnCount = worksheet.Dimension.End.Column;

            // To store unique rows
            var rowsToKeep = new List<int>();

            for (int row = 2; row <= rowCount; row++) // Start from row 2 to skip header
            {
                var rowValues = new List<string>();

                for (int col = 1; col <= columnCount; col++)
                {
                    rowValues.Add(worksheet.Cells[row, col].Text);
                }

                var rowKey = string.Join("|", rowValues);

                if (!uniqueRows.Contains(rowKey))
                {
                    uniqueRows.Add(rowKey);
                    rowsToKeep.Add(row);
                }
            }

            // Create a new worksheet to keep unique rows
            var newWorksheet = worksheet.Workbook.Worksheets.Add(worksheet.Name + "_Unique");

            // Copy header
            for (int col = 1; col <= columnCount; col++)
            {
                newWorksheet.Cells[1, col].Value = worksheet.Cells[1, col].Text;
            }

            // Copy unique rows
            int newRowIndex = 2; // Start from row 2 to keep header
            foreach (var rowIndex in rowsToKeep)
            {
                for (int col = 1; col <= columnCount; col++)
                {
                    newWorksheet.Cells[newRowIndex, col].Value = worksheet.Cells[rowIndex, col].Text;
                }
                newRowIndex++;
            }

            // Replace old worksheet with the new one
            worksheet.Workbook.Worksheets.Delete(worksheet.Name);
            newWorksheet.Name = worksheet.Name;
        }



    }
}
