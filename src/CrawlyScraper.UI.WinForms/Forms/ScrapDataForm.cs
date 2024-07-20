using CrawlyScraper.Core.Services;
using CrawlyScraper.Framework;
using Microsoft.Extensions.Logging;

namespace CrawlyScraper
{
    public partial class ScrapDataForm : Form
    {
        private readonly IScraperService _scraperService;
        private readonly ILogger<ScrapDataForm> _logger;

        public ScrapDataForm(IScraperService scraperService, ILogger<ScrapDataForm> logger)
        {
            InitializeComponent();
            _scraperService = scraperService;
            _logger = logger;
        }

        private async void btnScrapData_Click(object sender, EventArgs e)
        {
            string baseUrl = textBoxBaseUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(baseUrl) || !int.TryParse(textBoxPages.Text, out int pages) || pages < 1)
            {
                MessageBox.Show("Please enter valid input.");
                return;
            }

            // Generate Excel file
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            string filePath;
            string targetDirectory;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {

                filePath = saveDialog.FileName;
                targetDirectory = Path.GetDirectoryName(saveDialog.FileName);
            }
            else
            { return; }




                        var progress = new Progress<int>(UpdateProgressBar);
                        var progressReporter = new ProgressReporter(progress);

                        try
                        {
                            await _scraperService.ScrapDataAsync(baseUrl, pages, targetDirectory, filePath, progressReporter);
                            MessageBox.Show("Task completed successfully.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error occurred during scraping process");
                            MessageBox.Show($"An error occurred: {ex.Message}");
                        }
                   
        }

        private void UpdateProgressBar(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                //progressBar.Value = value;
            }
        }
    }
}