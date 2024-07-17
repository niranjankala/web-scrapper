namespace CrawlyScraper
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnScrapData = new Button();
            txtUrls = new TextBox();
            pnlBottomContainer = new Panel();
            lblUrls = new Label();
            panel2 = new Panel();
            dgvCrawledDataPreview = new DataGridView();
            lblPreview = new Label();
            pnlBottomContainer.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCrawledDataPreview).BeginInit();
            SuspendLayout();
            // 
            // btnScrapData
            // 
            btnScrapData.Location = new Point(757, 13);
            btnScrapData.Name = "btnScrapData";
            btnScrapData.Size = new Size(124, 30);
            btnScrapData.TabIndex = 0;
            btnScrapData.Text = "Start Crawling";
            btnScrapData.UseVisualStyleBackColor = true;
            btnScrapData.Click += btnScrapData_Click;
            // 
            // txtUrls
            // 
            txtUrls.Location = new Point(12, 27);
            txtUrls.Multiline = true;
            txtUrls.Name = "txtUrls";
            txtUrls.Size = new Size(869, 131);
            txtUrls.TabIndex = 1;
            // 
            // pnlBottomContainer
            // 
            pnlBottomContainer.Controls.Add(btnScrapData);
            pnlBottomContainer.Dock = DockStyle.Bottom;
            pnlBottomContainer.Location = new Point(0, 456);
            pnlBottomContainer.Name = "pnlBottomContainer";
            pnlBottomContainer.Size = new Size(893, 55);
            pnlBottomContainer.TabIndex = 2;
            // 
            // lblUrls
            // 
            lblUrls.AutoSize = true;
            lblUrls.Location = new Point(12, 9);
            lblUrls.Name = "lblUrls";
            lblUrls.Size = new Size(66, 15);
            lblUrls.TabIndex = 3;
            lblUrls.Text = "Enter URL's";
            // 
            // panel2
            // 
            panel2.Controls.Add(dgvCrawledDataPreview);
            panel2.Controls.Add(lblPreview);
            panel2.Controls.Add(lblUrls);
            panel2.Controls.Add(txtUrls);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(893, 456);
            panel2.TabIndex = 4;
            // 
            // dgvCrawledDataPreview
            // 
            dgvCrawledDataPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCrawledDataPreview.Location = new Point(15, 182);
            dgvCrawledDataPreview.Name = "dgvCrawledDataPreview";
            dgvCrawledDataPreview.Size = new Size(866, 268);
            dgvCrawledDataPreview.TabIndex = 6;
            // 
            // lblPreview
            // 
            lblPreview.AutoSize = true;
            lblPreview.Location = new Point(12, 161);
            lblPreview.Name = "lblPreview";
            lblPreview.Size = new Size(51, 15);
            lblPreview.TabIndex = 5;
            lblPreview.Text = "Preview:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(893, 511);
            Controls.Add(panel2);
            Controls.Add(pnlBottomContainer);
            Name = "MainForm";
            Text = "Crawly Scraper";
            pnlBottomContainer.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCrawledDataPreview).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnScrapData;
        private TextBox txtUrls;
        private Panel pnlBottomContainer;
        private Label lblUrls;
        private Panel panel2;
        private DataGridView dgvCrawledDataPreview;
        private Label lblPreview;
    }
}
