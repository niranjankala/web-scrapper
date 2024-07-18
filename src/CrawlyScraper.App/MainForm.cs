using HtmlAgilityPack;
using System.Text;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace CrawlyScraper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{
          

        //}

        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxContent;

        private async void btnScrapData_Click(object sender, EventArgs e)
        {
            //string baseUrl = txtUrls.Text;
            //if (string.IsNullOrWhiteSpace(baseUrl))
            //{
            //    MessageBox.Show("Please enter a valid Base URL.");
            //    return;
            //}

            //if (!int.TryParse(textBoxPages.Text, out int pages) || pages < 1)
            //{
            //    MessageBox.Show("Please enter a valid number of pages.");
            //    return;
            //}

            //StringBuilder allContent = new StringBuilder();
            //for (int i = 1; i <= pages; i++)
            //{
            //    string url = $"{baseUrl}?page={i}";
            //    string content = await Task.Run(() => CrawlWebsite(url));
            //    allContent.AppendLine(content);
            //}
            //textBoxContent.Text = allContent.ToString();
        }



        private string CrawlWebsite(string url)
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = web.Load(url);

                return ExtractContent(document);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return string.Empty;
            }
        }

        private string ExtractContent(HtmlDocument document)
        {
            // Extract the desired content from the HTML document
            // This is just an example and should be modified to fit your specific needs

            var sb = new StringBuilder();
            foreach (var node in document.DocumentNode.SelectNodes("//div[@class='content']"))
            {
                sb.AppendLine(node.InnerText.Trim());
            }

            return sb.ToString();
        }

    }
}
