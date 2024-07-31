namespace CrawlyScraper.App
{
    partial class CategoriesCrowlerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ProcessCategoriesButton = new Button();
            statusBar = new StatusStrip();
            ProgressStatusLabel = new ToolStripStatusLabel();
            progressBar = new ProgressBar();
            btnBrowseExport = new Button();
            txtExportPath = new TextBox();
            lblExportPath = new Label();
            btnBrowseCategories = new Button();
            txtCategoriesPath = new TextBox();
            lblCategoriesPath = new Label();
            statusBar.SuspendLayout();
            SuspendLayout();
            // 
            // ProcessCategoriesButton
            // 
            ProcessCategoriesButton.Location = new Point(143, 163);
            ProcessCategoriesButton.Name = "ProcessCategoriesButton";
            ProcessCategoriesButton.Size = new Size(155, 32);
            ProcessCategoriesButton.TabIndex = 7;
            ProcessCategoriesButton.Text = "Process Categories";
            ProcessCategoriesButton.UseVisualStyleBackColor = true;
            ProcessCategoriesButton.Click += ProcessCategoriesButton_Click;
            // 
            // statusBar
            // 
            statusBar.ImageScalingSize = new Size(28, 28);
            statusBar.Items.AddRange(new ToolStripItem[] { ProgressStatusLabel });
            statusBar.Location = new Point(0, 219);
            statusBar.Name = "statusBar";
            statusBar.Padding = new Padding(1, 0, 16, 0);
            statusBar.Size = new Size(463, 22);
            statusBar.TabIndex = 15;
            // 
            // ProgressStatusLabel
            // 
            ProgressStatusLabel.Name = "ProgressStatusLabel";
            ProgressStatusLabel.Size = new Size(16, 17);
            ProgressStatusLabel.Text = "...";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(13, 113);
            progressBar.Margin = new Padding(4);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(444, 26);
            progressBar.TabIndex = 14;
            // 
            // btnBrowseExport
            // 
            btnBrowseExport.Location = new Point(370, 69);
            btnBrowseExport.Margin = new Padding(4);
            btnBrowseExport.Name = "btnBrowseExport";
            btnBrowseExport.Size = new Size(88, 25);
            btnBrowseExport.TabIndex = 13;
            btnBrowseExport.Text = "Browse...";
            btnBrowseExport.UseVisualStyleBackColor = true;
            btnBrowseExport.Click += btnBrowseExport_Click;
            // 
            // txtExportPath
            // 
            txtExportPath.Location = new Point(13, 71);
            txtExportPath.Margin = new Padding(4);
            txtExportPath.Name = "txtExportPath";
            txtExportPath.Size = new Size(349, 23);
            txtExportPath.TabIndex = 12;
            txtExportPath.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\industrybuying\\categories_images.xlsx";
            // 
            // lblExportPath
            // 
            lblExportPath.AutoSize = true;
            lblExportPath.Location = new Point(13, 53);
            lblExportPath.Margin = new Padding(4, 0, 4, 0);
            lblExportPath.Name = "lblExportPath";
            lblExportPath.Size = new Size(114, 15);
            lblExportPath.TabIndex = 11;
            lblExportPath.Text = "Exported Excel Path:";
            // 
            // btnBrowseCategories
            // 
            btnBrowseCategories.Location = new Point(370, 25);
            btnBrowseCategories.Margin = new Padding(4);
            btnBrowseCategories.Name = "btnBrowseCategories";
            btnBrowseCategories.Size = new Size(88, 26);
            btnBrowseCategories.TabIndex = 18;
            btnBrowseCategories.Text = "Browse...";
            btnBrowseCategories.UseVisualStyleBackColor = true;
            btnBrowseCategories.Click += btnBrowseCategories_Click;
            // 
            // txtCategoriesPath
            // 
            txtCategoriesPath.Location = new Point(13, 28);
            txtCategoriesPath.Margin = new Padding(4);
            txtCategoriesPath.Name = "txtCategoriesPath";
            txtCategoriesPath.Size = new Size(349, 23);
            txtCategoriesPath.TabIndex = 17;
            txtCategoriesPath.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\categories-2024-07-22.xlsx";
            // 
            // lblCategoriesPath
            // 
            lblCategoriesPath.AutoSize = true;
            lblCategoriesPath.Location = new Point(13, 9);
            lblCategoriesPath.Margin = new Padding(4, 0, 4, 0);
            lblCategoriesPath.Name = "lblCategoriesPath";
            lblCategoriesPath.Size = new Size(123, 15);
            lblCategoriesPath.TabIndex = 16;
            lblCategoriesPath.Text = "Categories Excel Path:";
            // 
            // CategoriesCrowlerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(463, 241);
            Controls.Add(btnBrowseCategories);
            Controls.Add(txtCategoriesPath);
            Controls.Add(lblCategoriesPath);
            Controls.Add(statusBar);
            Controls.Add(progressBar);
            Controls.Add(btnBrowseExport);
            Controls.Add(txtExportPath);
            Controls.Add(lblExportPath);
            Controls.Add(ProcessCategoriesButton);
            Name = "CategoriesCrowlerForm";
            Text = "CategoriesCrowlerForm";
            Load += CategoriesCrowlerForm_Load;
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ProcessCategoriesButton;
        private StatusStrip statusBar;
        private ToolStripStatusLabel ProgressStatusLabel;
        private ProgressBar progressBar;
        private Button btnBrowseExport;
        private TextBox txtExportPath;
        private Label lblExportPath;
        private Button btnBrowseCategories;
        private TextBox txtCategoriesPath;
        private Label lblCategoriesPath;
    }
}