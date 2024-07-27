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

                    // Remove duplicates from the "ProductSEOKeywords" sheet
                    var seoKeywordsSheet = worksheets.FirstOrDefault(ws => ws.Name == "ProductSEOKeywords");
                    if (seoKeywordsSheet != null)
                    {
                        RemoveDuplicates(seoKeywordsSheet);
                    }

                    // Dictionary to store data by product_id for each worksheet
                    var productData = new Dictionary<string, List<Dictionary<string, object>>>();
                    var productIds = new HashSet<string>();

                    // Process each worksheet and organize data by product_id
                    foreach (var worksheet in worksheets)
                    {
                        var sheetData = new List<Dictionary<string, object>>();
                        var headers = new Dictionary<int, string>();

                        // Read headers
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            headers[col] = worksheet.Cells[1, col].Text;
                        }

                        // Read data rows
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var rowData = new Dictionary<string, object>();
                            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                            {
                                rowData[headers[col]] = worksheet.Cells[row, col].Text;
                            }

                            sheetData.Add(rowData);

                            // Track product_id
                            if (headers.ContainsValue("product_id"))
                            {
                                var productId = rowData["product_id"].ToString();
                                productIds.Add(productId);
                            }
                        }

                        productData[worksheet.Name] = sheetData;
                    }

                    // Split data into multiple files
                    var productIdList = productIds.ToList();
                    var idsPerFile = (int)Math.Ceiling((double)productIdList.Count / numFiles);

                    for (int fileIndex = 0; fileIndex < numFiles; fileIndex++)
                    {
                        var newFilePath = Path.Combine(targetFolder, $"Part_{fileIndex + 1}.xlsx");

                        using (var newPackage = new ExcelPackage())
                        {
                            foreach (var worksheet in worksheets)
                            {
                                var newWorksheet = newPackage.Workbook.Worksheets.Add(worksheet.Name);

                                // Add headers
                                var headers = productData[worksheet.Name][0].Keys.ToList();
                                for (int col = 0; col < headers.Count; col++)
                                {
                                    newWorksheet.Cells[1, col + 1].Value = headers[col];
                                }

                                // Add data rows
                                int newRowIdx = 2;
                                var startIdx = fileIndex * idsPerFile;
                                var endIdx = Math.Min(startIdx + idsPerFile, productIdList.Count);

                                for (int idIdx = startIdx; idIdx < endIdx; idIdx++)
                                {
                                    var productId = productIdList[idIdx];

                                    foreach (var rowData in productData[worksheet.Name])
                                    {
                                        if (rowData.ContainsKey("product_id") && rowData["product_id"].ToString() == productId)
                                        {
                                            for (int col = 0; col < headers.Count; col++)
                                            {
                                                newWorksheet.Cells[newRowIdx, col + 1].Value = rowData[headers[col]];
                                            }
                                            newRowIdx++;
                                        }
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
            var uniqueRows = new HashSet<string>();
            var rowCount = worksheet.Dimension.End.Row;
            var columnCount = worksheet.Dimension.End.Column;

            for (int row = rowCount; row >= 2; row--) // Start from the last row to the first row
            {
                var rowValues = new List<string>();

                for (int col = 1; col <= columnCount; col++)
                {
                    if (worksheet.Name == "ProductSEOKeywords" && col != 3)
                    {
                        continue;
                    }
                    else
                    {
                        rowValues.Add(worksheet.Cells[row, col].Text);
                    }
                }

                var rowKey = string.Join("|", rowValues);

                if (!uniqueRows.Add(rowKey)) // If the row is already in the set, remove it
                {
                    worksheet.DeleteRow(row);
                }
            }
        }


    }
}
