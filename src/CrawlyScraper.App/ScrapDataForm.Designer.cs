namespace CrawlyScraper
{
    partial class ScrapDataForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            textBoxBaseUrl = new TextBox();
            buttonStart = new Button();
            textBoxContent = new TextBox();
            textBoxPages = new TextBox();
            labelBaseUrl = new Label();
            labelPages = new Label();
            SuspendLayout();
            // 
            // textBoxBaseUrl
            // 
            textBoxBaseUrl.Location = new Point(18, 54);
            textBoxBaseUrl.Margin = new Padding(4, 6, 4, 6);
            textBoxBaseUrl.Name = "textBoxBaseUrl";
            textBoxBaseUrl.Size = new Size(1160, 35);
            textBoxBaseUrl.TabIndex = 0;
            textBoxBaseUrl.Text = "https://www.industrybuying.com/agriculture-garden-landscaping-2384/harvester-2752/";
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(18, 204);
            buttonStart.Margin = new Padding(4, 6, 4, 6);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(1162, 43);
            buttonStart.TabIndex = 2;
            buttonStart.Text = "Start Crawling";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += btnScrapData_Click;
            // 
            // textBoxContent
            // 
            textBoxContent.Location = new Point(18, 276);
            textBoxContent.Margin = new Padding(4, 6, 4, 6);
            textBoxContent.Multiline = true;
            textBoxContent.Name = "textBoxContent";
            textBoxContent.ScrollBars = ScrollBars.Vertical;
            textBoxContent.Size = new Size(1160, 686);
            textBoxContent.TabIndex = 3;
            // 
            // textBoxPages
            // 
            textBoxPages.Location = new Point(18, 133);
            textBoxPages.Margin = new Padding(4, 6, 4, 6);
            textBoxPages.Name = "textBoxPages";
            textBoxPages.Size = new Size(148, 35);
            textBoxPages.TabIndex = 1;
            textBoxPages.Text = "4";
            // 
            // labelBaseUrl
            // 
            labelBaseUrl.AutoSize = true;
            labelBaseUrl.Location = new Point(18, 17);
            labelBaseUrl.Margin = new Padding(4, 0, 4, 0);
            labelBaseUrl.Name = "labelBaseUrl";
            labelBaseUrl.Size = new Size(99, 30);
            labelBaseUrl.TabIndex = 4;
            labelBaseUrl.Text = "Base URL";
            // 
            // labelPages
            // 
            labelPages.AutoSize = true;
            labelPages.Location = new Point(18, 96);
            labelPages.Margin = new Padding(4, 0, 4, 0);
            labelPages.Name = "labelPages";
            labelPages.Size = new Size(160, 30);
            labelPages.TabIndex = 5;
            labelPages.Text = "Pages to Scrape";
            // 
            // ScrapDataForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 988);
            Controls.Add(labelPages);
            Controls.Add(labelBaseUrl);
            Controls.Add(textBoxContent);
            Controls.Add(buttonStart);
            Controls.Add(textBoxPages);
            Controls.Add(textBoxBaseUrl);
            Margin = new Padding(4, 6, 4, 6);
            Name = "ScrapDataForm";
            Text = "Web Crawler";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox textBoxBaseUrl;
        private System.Windows.Forms.TextBox textBoxPages;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxContent;
        private System.Windows.Forms.Label labelBaseUrl;
        private System.Windows.Forms.Label labelPages;
    }
}