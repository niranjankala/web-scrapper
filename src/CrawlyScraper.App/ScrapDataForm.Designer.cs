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
            progressBar = new ProgressBar();
            btnMergeData = new Button();
            SplitDataButton = new Button();
            CategoryDataButton = new Button();
            btnProcessProducts = new Button();
            SuspendLayout();
            // 
            // textBoxBaseUrl
            // 
            textBoxBaseUrl.Location = new Point(17, 54);
            textBoxBaseUrl.Margin = new Padding(3, 6, 3, 6);
            textBoxBaseUrl.Name = "textBoxBaseUrl";
            textBoxBaseUrl.Size = new Size(1159, 35);
            textBoxBaseUrl.TabIndex = 0;
            textBoxBaseUrl.Text = "https://www.industrybuying.com/agriculture-garden-landscaping-2384/harvester-2752/";
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(17, 204);
            buttonStart.Margin = new Padding(3, 6, 3, 6);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(1162, 44);
            buttonStart.TabIndex = 2;
            buttonStart.Text = "Start Crawling";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += btnScrapData_Click;
            // 
            // textBoxContent
            // 
            textBoxContent.Location = new Point(19, 320);
            textBoxContent.Margin = new Padding(3, 6, 3, 6);
            textBoxContent.Multiline = true;
            textBoxContent.Name = "textBoxContent";
            textBoxContent.ScrollBars = ScrollBars.Vertical;
            textBoxContent.Size = new Size(1159, 356);
            textBoxContent.TabIndex = 3;
            // 
            // textBoxPages
            // 
            textBoxPages.Location = new Point(17, 132);
            textBoxPages.Margin = new Padding(3, 6, 3, 6);
            textBoxPages.Name = "textBoxPages";
            textBoxPages.Size = new Size(148, 35);
            textBoxPages.TabIndex = 1;
            textBoxPages.Text = "4";
            // 
            // labelBaseUrl
            // 
            labelBaseUrl.AutoSize = true;
            labelBaseUrl.Location = new Point(17, 16);
            labelBaseUrl.Name = "labelBaseUrl";
            labelBaseUrl.Size = new Size(99, 30);
            labelBaseUrl.TabIndex = 4;
            labelBaseUrl.Text = "Base URL";
            // 
            // labelPages
            // 
            labelPages.AutoSize = true;
            labelPages.Location = new Point(17, 96);
            labelPages.Name = "labelPages";
            labelPages.Size = new Size(160, 30);
            labelPages.TabIndex = 5;
            labelPages.Text = "Pages to Scrape";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(17, 260);
            progressBar.Margin = new Padding(3, 6, 3, 6);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1161, 48);
            progressBar.TabIndex = 7;
            // 
            // btnMergeData
            // 
            btnMergeData.Location = new Point(21, 692);
            btnMergeData.Margin = new Padding(5, 6, 5, 6);
            btnMergeData.Name = "btnMergeData";
            btnMergeData.Size = new Size(129, 46);
            btnMergeData.TabIndex = 8;
            btnMergeData.Text = "Merge";
            btnMergeData.UseVisualStyleBackColor = true;
            btnMergeData.Click += btnMergeData_Click;
            // 
            // SplitDataButton
            // 
            SplitDataButton.Location = new Point(159, 692);
            SplitDataButton.Margin = new Padding(5, 6, 5, 6);
            SplitDataButton.Name = "SplitDataButton";
            SplitDataButton.Size = new Size(125, 46);
            SplitDataButton.TabIndex = 8;
            SplitDataButton.Text = "Split Data";
            SplitDataButton.UseVisualStyleBackColor = true;
            SplitDataButton.Click += SplitDataButton_Click;
            // 
            // CategoryDataButton
            // 
            CategoryDataButton.Location = new Point(692, 692);
            CategoryDataButton.Margin = new Padding(5, 6, 5, 6);
            CategoryDataButton.Name = "CategoryDataButton";
            CategoryDataButton.Size = new Size(237, 46);
            CategoryDataButton.TabIndex = 9;
            CategoryDataButton.Text = "Create Categories Data";
            CategoryDataButton.UseVisualStyleBackColor = true;
            CategoryDataButton.Click += CategoryDataButton_Click;
            // 
            // btnProcessProducts
            // 
            btnProcessProducts.Location = new Point(939, 692);
            btnProcessProducts.Margin = new Padding(5, 6, 5, 6);
            btnProcessProducts.Name = "btnProcessProducts";
            btnProcessProducts.Size = new Size(237, 46);
            btnProcessProducts.TabIndex = 9;
            btnProcessProducts.Text = "Process Products";
            btnProcessProducts.UseVisualStyleBackColor = true;
            btnProcessProducts.Click += btnProcessProducts_Click;
            // 
            // ScrapDataForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 748);
            Controls.Add(CategoryDataButton);
            Controls.Add(btnProcessProducts);
            Controls.Add(SplitDataButton);
            Controls.Add(btnMergeData);
            Controls.Add(progressBar);
            Controls.Add(labelPages);
            Controls.Add(labelBaseUrl);
            Controls.Add(textBoxContent);
            Controls.Add(buttonStart);
            Controls.Add(textBoxPages);
            Controls.Add(textBoxBaseUrl);
            Margin = new Padding(3, 6, 3, 6);
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
        private ProgressBar progressBar;
        private Button btnMergeData;
        private Button SplitDataButton;
        private Button CategoryDataButton;
        private Button btnProcessProducts;
    }
}