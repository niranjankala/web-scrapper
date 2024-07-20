namespace CrawlyScraper
{
    partial class ScrapDataForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBoxBaseUrl;
        private System.Windows.Forms.TextBox textBoxPages;
        private System.Windows.Forms.Button btnScrapData;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelBaseUrl;
        private System.Windows.Forms.Label labelPages;

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
            textBoxPages = new TextBox();
            btnScrapData = new Button();
            progressBar = new ProgressBar();
            labelBaseUrl = new Label();
            labelPages = new Label();
            SuspendLayout();
            // 
            // textBoxBaseUrl
            // 
            textBoxBaseUrl.Location = new Point(180, 38);
            textBoxBaseUrl.Margin = new Padding(4, 6, 4, 6);
            textBoxBaseUrl.Name = "textBoxBaseUrl";
            textBoxBaseUrl.Size = new Size(947, 35);
            textBoxBaseUrl.TabIndex = 0;
            textBoxBaseUrl.Text = "https://www.industrybuying.com/agriculture-garden-landscaping-2384/harvester-2752/";
            // 
            // textBoxPages
            // 
            textBoxPages.Location = new Point(180, 112);
            textBoxPages.Margin = new Padding(4, 6, 4, 6);
            textBoxPages.Name = "textBoxPages";
            textBoxPages.Size = new Size(148, 35);
            textBoxPages.TabIndex = 1;
            textBoxPages.Text = "4";
            // 
            // btnScrapData
            // 
            btnScrapData.Location = new Point(180, 188);
            btnScrapData.Margin = new Padding(4, 6, 4, 6);
            btnScrapData.Name = "btnScrapData";
            btnScrapData.Size = new Size(150, 56);
            btnScrapData.TabIndex = 2;
            btnScrapData.Text = "Scrap Data";
            btnScrapData.UseVisualStyleBackColor = true;
            btnScrapData.Click += btnScrapData_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(30, 281);
            progressBar.Margin = new Padding(4, 6, 4, 6);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1097, 47);
            progressBar.TabIndex = 3;
            // 
            // labelBaseUrl
            // 
            labelBaseUrl.AutoSize = true;
            labelBaseUrl.Location = new Point(30, 38);
            labelBaseUrl.Margin = new Padding(4, 0, 4, 0);
            labelBaseUrl.Name = "labelBaseUrl";
            labelBaseUrl.Size = new Size(99, 30);
            labelBaseUrl.TabIndex = 4;
            labelBaseUrl.Text = "Base URL";
            // 
            // labelPages
            // 
            labelPages.AutoSize = true;
            labelPages.Location = new Point(30, 112);
            labelPages.Margin = new Padding(4, 0, 4, 0);
            labelPages.Name = "labelPages";
            labelPages.Size = new Size(67, 30);
            labelPages.TabIndex = 5;
            labelPages.Text = "Pages";
            // 
            // ScrapDataForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1153, 475);
            Controls.Add(labelPages);
            Controls.Add(labelBaseUrl);
            Controls.Add(progressBar);
            Controls.Add(btnScrapData);
            Controls.Add(textBoxPages);
            Controls.Add(textBoxBaseUrl);
            Margin = new Padding(4, 6, 4, 6);
            Name = "ScrapDataForm";
            Text = "Scrap Data";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
