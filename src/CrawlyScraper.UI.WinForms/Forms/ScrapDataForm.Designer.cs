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
            btnProcessCategories = new Button();
            SuspendLayout();
            // 
            // textBoxBaseUrl
            // 
            textBoxBaseUrl.Location = new Point(105, 19);
            textBoxBaseUrl.Margin = new Padding(2, 3, 2, 3);
            textBoxBaseUrl.Name = "textBoxBaseUrl";
            textBoxBaseUrl.Size = new Size(554, 23);
            textBoxBaseUrl.TabIndex = 0;
            textBoxBaseUrl.Text = "https://www.industrybuying.com/agriculture-garden-landscaping-2384/harvester-2752/";
            // 
            // textBoxPages
            // 
            textBoxPages.Location = new Point(105, 56);
            textBoxPages.Margin = new Padding(2, 3, 2, 3);
            textBoxPages.Name = "textBoxPages";
            textBoxPages.Size = new Size(88, 23);
            textBoxPages.TabIndex = 1;
            textBoxPages.Text = "4";
            // 
            // btnScrapData
            // 
            btnScrapData.Location = new Point(105, 94);
            btnScrapData.Margin = new Padding(2, 3, 2, 3);
            btnScrapData.Name = "btnScrapData";
            btnScrapData.Size = new Size(88, 28);
            btnScrapData.TabIndex = 2;
            btnScrapData.Text = "Scrap Data";
            btnScrapData.UseVisualStyleBackColor = true;
            btnScrapData.Click += btnScrapData_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(18, 140);
            progressBar.Margin = new Padding(2, 3, 2, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(640, 24);
            progressBar.TabIndex = 3;
            // 
            // labelBaseUrl
            // 
            labelBaseUrl.AutoSize = true;
            labelBaseUrl.Location = new Point(18, 19);
            labelBaseUrl.Margin = new Padding(2, 0, 2, 0);
            labelBaseUrl.Name = "labelBaseUrl";
            labelBaseUrl.Size = new Size(55, 15);
            labelBaseUrl.TabIndex = 4;
            labelBaseUrl.Text = "Base URL";
            // 
            // labelPages
            // 
            labelPages.AutoSize = true;
            labelPages.Location = new Point(18, 56);
            labelPages.Margin = new Padding(2, 0, 2, 0);
            labelPages.Name = "labelPages";
            labelPages.Size = new Size(38, 15);
            labelPages.TabIndex = 5;
            labelPages.Text = "Pages";
            // 
            // btnProcessCategories
            // 
            btnProcessCategories.Location = new Point(537, 203);
            btnProcessCategories.Name = "btnProcessCategories";
            btnProcessCategories.Size = new Size(121, 23);
            btnProcessCategories.TabIndex = 6;
            btnProcessCategories.Text = "Process Categories";
            btnProcessCategories.UseVisualStyleBackColor = true;
            btnProcessCategories.Click += btnProcessCategories_Click;
            // 
            // ScrapDataForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(673, 238);
            Controls.Add(btnProcessCategories);
            Controls.Add(labelPages);
            Controls.Add(labelBaseUrl);
            Controls.Add(progressBar);
            Controls.Add(btnScrapData);
            Controls.Add(textBoxPages);
            Controls.Add(textBoxBaseUrl);
            Margin = new Padding(2, 3, 2, 3);
            Name = "ScrapDataForm";
            Text = "Scrap Data";
            ResumeLayout(false);
            PerformLayout();
        }

        private Button btnProcessCategories;
    }
}
