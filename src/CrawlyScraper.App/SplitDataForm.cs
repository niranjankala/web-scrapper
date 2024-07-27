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

                    // Determine the total number of rows
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
                                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                                {
                                    newWorksheet.Cells[1, col].Value = worksheet.Cells[1, col].Text;
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
    }
}
