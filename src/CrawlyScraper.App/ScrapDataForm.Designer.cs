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
            button1 = new Button();
            progressBar = new ProgressBar();
            SuspendLayout();
            // 
            // textBoxBaseUrl
            // 
            textBoxBaseUrl.Location = new Point(10, 27);
            textBoxBaseUrl.Margin = new Padding(2, 3, 2, 3);
            textBoxBaseUrl.Name = "textBoxBaseUrl";
            textBoxBaseUrl.Size = new Size(678, 23);
            textBoxBaseUrl.TabIndex = 0;
            textBoxBaseUrl.Text = "https://www.industrybuying.com/agriculture-garden-landscaping-2384/harvester-2752/";
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(10, 102);
            buttonStart.Margin = new Padding(2, 3, 2, 3);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(678, 22);
            buttonStart.TabIndex = 2;
            buttonStart.Text = "Start Crawling";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += btnScrapData_Click;
            // 
            // textBoxContent
            // 
            textBoxContent.Location = new Point(11, 160);
            textBoxContent.Margin = new Padding(2, 3, 2, 3);
            textBoxContent.Multiline = true;
            textBoxContent.Name = "textBoxContent";
            textBoxContent.ScrollBars = ScrollBars.Vertical;
            textBoxContent.Size = new Size(678, 180);
            textBoxContent.TabIndex = 3;
            // 
            // textBoxPages
            // 
            textBoxPages.Location = new Point(10, 66);
            textBoxPages.Margin = new Padding(2, 3, 2, 3);
            textBoxPages.Name = "textBoxPages";
            textBoxPages.Size = new Size(88, 23);
            textBoxPages.TabIndex = 1;
            textBoxPages.Text = "4";
            // 
            // labelBaseUrl
            // 
            labelBaseUrl.AutoSize = true;
            labelBaseUrl.Location = new Point(10, 8);
            labelBaseUrl.Margin = new Padding(2, 0, 2, 0);
            labelBaseUrl.Name = "labelBaseUrl";
            labelBaseUrl.Size = new Size(55, 15);
            labelBaseUrl.TabIndex = 4;
            labelBaseUrl.Text = "Base URL";
            // 
            // labelPages
            // 
            labelPages.AutoSize = true;
            labelPages.Location = new Point(10, 48);
            labelPages.Margin = new Padding(2, 0, 2, 0);
            labelPages.Name = "labelPages";
            labelPages.Size = new Size(90, 15);
            labelPages.TabIndex = 5;
            labelPages.Text = "Pages to Scrape";
            // 
            // button1
            // 
            button1.Location = new Point(549, 346);
            button1.Name = "button1";
            button1.Size = new Size(138, 23);
            button1.TabIndex = 6;
            button1.Text = "Process Categories";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnProcessCategories_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(10, 130);
            progressBar.Margin = new Padding(2, 3, 2, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(677, 24);
            progressBar.TabIndex = 7;
            // 
            // ScrapDataForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 374);
            Controls.Add(progressBar);
            Controls.Add(button1);
            Controls.Add(labelPages);
            Controls.Add(labelBaseUrl);
            Controls.Add(textBoxContent);
            Controls.Add(buttonStart);
            Controls.Add(textBoxPages);
            Controls.Add(textBoxBaseUrl);
            Margin = new Padding(2, 3, 2, 3);
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
        private Button button1;
        private ProgressBar progressBar;
    }
}