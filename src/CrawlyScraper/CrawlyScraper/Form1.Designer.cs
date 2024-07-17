namespace CrawlyScraper
{
    partial class Form1
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
            this.textBoxBaseUrl = new System.Windows.Forms.TextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.textBoxContent = new System.Windows.Forms.TextBox();
            this.textBoxPages = new System.Windows.Forms.TextBox();
            this.labelBaseUrl = new System.Windows.Forms.Label();
            this.labelPages = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxBaseUrl
            // 
            this.textBoxBaseUrl.Location = new System.Drawing.Point(12, 29);
            this.textBoxBaseUrl.Name = "textBoxBaseUrl";
            this.textBoxBaseUrl.Size = new System.Drawing.Size(775, 22);
            this.textBoxBaseUrl.TabIndex = 0;
            // 
            // textBoxPages
            // 
            this.textBoxPages.Location = new System.Drawing.Point(12, 71);
            this.textBoxPages.Name = "textBoxPages";
            this.textBoxPages.Size = new System.Drawing.Size(100, 22);
            this.textBoxPages.TabIndex = 1;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 109);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(775, 23);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Start Crawling";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.btnScrapData_Click);
            // 
            // textBoxContent
            // 
            this.textBoxContent.Location = new System.Drawing.Point(12, 147);
            this.textBoxContent.Multiline = true;
            this.textBoxContent.Name = "textBoxContent";
            this.textBoxContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxContent.Size = new System.Drawing.Size(775, 368);
            this.textBoxContent.TabIndex = 3;
            // 
            // labelBaseUrl
            // 
            this.labelBaseUrl.AutoSize = true;
            this.labelBaseUrl.Location = new System.Drawing.Point(12, 9);
            this.labelBaseUrl.Name = "labelBaseUrl";
            this.labelBaseUrl.Size = new System.Drawing.Size(71, 17);
            this.labelBaseUrl.TabIndex = 4;
            this.labelBaseUrl.Text = "Base URL";
            // 
            // labelPages
            // 
            this.labelPages.AutoSize = true;
            this.labelPages.Location = new System.Drawing.Point(12, 51);
            this.labelPages.Name = "labelPages";
            this.labelPages.Size = new System.Drawing.Size(111, 17);
            this.labelPages.TabIndex = 5;
            this.labelPages.Text = "Pages to Scrape";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 527);
            this.Controls.Add(this.labelPages);
            this.Controls.Add(this.labelBaseUrl);
            this.Controls.Add(this.textBoxContent);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.textBoxPages);
            this.Controls.Add(this.textBoxBaseUrl);
            this.Name = "MainForm";
            this.Text = "Web Crawler";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox textBoxBaseUrl;
        private System.Windows.Forms.TextBox textBoxPages;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxContent;
        private System.Windows.Forms.Label labelBaseUrl;
        private System.Windows.Forms.Label labelPages;
    }
}