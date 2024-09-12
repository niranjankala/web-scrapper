namespace CrawlyScraper.App
{
    partial class CategoriesCrawlerForm
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
            btnBrowseImageExportFolder = new Button();
            txtImageExportPath = new TextBox();
            label1 = new Label();
            UpdateCategoriesButton = new Button();
            statusBar.SuspendLayout();
            SuspendLayout();
            // 
            // ProcessCategoriesButton
            // 
            ProcessCategoriesButton.Location = new Point(48, 418);
            ProcessCategoriesButton.Margin = new Padding(5, 6, 5, 6);
            ProcessCategoriesButton.Name = "ProcessCategoriesButton";
            ProcessCategoriesButton.Size = new Size(266, 64);
            ProcessCategoriesButton.TabIndex = 7;
            ProcessCategoriesButton.Text = "Process Categories";
            ProcessCategoriesButton.UseVisualStyleBackColor = true;
            ProcessCategoriesButton.Click += ProcessCategoriesButton_Click;
            // 
            // statusBar
            // 
            statusBar.ImageScalingSize = new Size(28, 28);
            statusBar.Items.AddRange(new ToolStripItem[] { ProgressStatusLabel });
            statusBar.Location = new Point(0, 503);
            statusBar.Name = "statusBar";
            statusBar.Padding = new Padding(2, 0, 27, 0);
            statusBar.Size = new Size(794, 39);
            statusBar.TabIndex = 15;
            // 
            // ProgressStatusLabel
            // 
            ProgressStatusLabel.Name = "ProgressStatusLabel";
            ProgressStatusLabel.Size = new Size(28, 30);
            ProgressStatusLabel.Text = "...";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(27, 313);
            progressBar.Margin = new Padding(7, 8, 7, 8);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(761, 52);
            progressBar.TabIndex = 14;
            // 
            // btnBrowseExport
            // 
            btnBrowseExport.Location = new Point(634, 138);
            btnBrowseExport.Margin = new Padding(7, 8, 7, 8);
            btnBrowseExport.Name = "btnBrowseExport";
            btnBrowseExport.Size = new Size(151, 50);
            btnBrowseExport.TabIndex = 13;
            btnBrowseExport.Text = "Browse...";
            btnBrowseExport.UseVisualStyleBackColor = true;
            btnBrowseExport.Click += btnBrowseExport_Click;
            // 
            // txtExportPath
            // 
            txtExportPath.Location = new Point(22, 142);
            txtExportPath.Margin = new Padding(7, 8, 7, 8);
            txtExportPath.Name = "txtExportPath";
            txtExportPath.Size = new Size(595, 35);
            txtExportPath.TabIndex = 12;
            txtExportPath.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\industrybuying\\categories_images.xlsx";
            // 
            // lblExportPath
            // 
            lblExportPath.AutoSize = true;
            lblExportPath.Location = new Point(22, 106);
            lblExportPath.Margin = new Padding(7, 0, 7, 0);
            lblExportPath.Name = "lblExportPath";
            lblExportPath.Size = new Size(200, 30);
            lblExportPath.TabIndex = 11;
            lblExportPath.Text = "Exported Excel Path:";
            // 
            // btnBrowseCategories
            // 
            btnBrowseCategories.Location = new Point(634, 50);
            btnBrowseCategories.Margin = new Padding(7, 8, 7, 8);
            btnBrowseCategories.Name = "btnBrowseCategories";
            btnBrowseCategories.Size = new Size(151, 52);
            btnBrowseCategories.TabIndex = 18;
            btnBrowseCategories.Text = "Browse...";
            btnBrowseCategories.UseVisualStyleBackColor = true;
            btnBrowseCategories.Click += btnBrowseCategories_Click;
            // 
            // txtCategoriesPath
            // 
            txtCategoriesPath.Location = new Point(22, 56);
            txtCategoriesPath.Margin = new Padding(7, 8, 7, 8);
            txtCategoriesPath.Name = "txtCategoriesPath";
            txtCategoriesPath.Size = new Size(595, 35);
            txtCategoriesPath.TabIndex = 17;
            txtCategoriesPath.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\categories-2024-07-22.xlsx";
            // 
            // lblCategoriesPath
            // 
            lblCategoriesPath.AutoSize = true;
            lblCategoriesPath.Location = new Point(22, 18);
            lblCategoriesPath.Margin = new Padding(7, 0, 7, 0);
            lblCategoriesPath.Name = "lblCategoriesPath";
            lblCategoriesPath.Size = new Size(216, 30);
            lblCategoriesPath.TabIndex = 16;
            lblCategoriesPath.Text = "Categories Excel Path:";
            // 
            // btnBrowseImageExportFolder
            // 
            btnBrowseImageExportFolder.Location = new Point(634, 224);
            btnBrowseImageExportFolder.Margin = new Padding(7, 8, 7, 8);
            btnBrowseImageExportFolder.Name = "btnBrowseImageExportFolder";
            btnBrowseImageExportFolder.Size = new Size(151, 50);
            btnBrowseImageExportFolder.TabIndex = 21;
            btnBrowseImageExportFolder.Text = "Browse...";
            btnBrowseImageExportFolder.UseVisualStyleBackColor = true;
            btnBrowseImageExportFolder.Click += btnBrowseImageExportFolder_Click;
            // 
            // txtImageExportPath
            // 
            txtImageExportPath.Location = new Point(22, 228);
            txtImageExportPath.Margin = new Padding(7, 8, 7, 8);
            txtImageExportPath.Name = "txtImageExportPath";
            txtImageExportPath.Size = new Size(595, 35);
            txtImageExportPath.TabIndex = 20;
            txtImageExportPath.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\industrybuying\\images\\category";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 192);
            label1.Margin = new Padding(7, 0, 7, 0);
            label1.Name = "label1";
            label1.Size = new Size(197, 30);
            label1.TabIndex = 19;
            label1.Text = "Images Export Path:";
            // 
            // UpdateCategoriesButton
            // 
            UpdateCategoriesButton.Location = new Point(467, 418);
            UpdateCategoriesButton.Margin = new Padding(5, 6, 5, 6);
            UpdateCategoriesButton.Name = "UpdateCategoriesButton";
            UpdateCategoriesButton.Size = new Size(266, 64);
            UpdateCategoriesButton.TabIndex = 7;
            UpdateCategoriesButton.Text = "Update Categories";
            UpdateCategoriesButton.UseVisualStyleBackColor = true;
            UpdateCategoriesButton.Click += UpdateCategoriesButton_Click;
            // 
            // CategoriesCrawlerForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(794, 542);
            Controls.Add(btnBrowseImageExportFolder);
            Controls.Add(txtImageExportPath);
            Controls.Add(label1);
            Controls.Add(btnBrowseCategories);
            Controls.Add(txtCategoriesPath);
            Controls.Add(lblCategoriesPath);
            Controls.Add(statusBar);
            Controls.Add(progressBar);
            Controls.Add(btnBrowseExport);
            Controls.Add(txtExportPath);
            Controls.Add(lblExportPath);
            Controls.Add(UpdateCategoriesButton);
            Controls.Add(ProcessCategoriesButton);
            Margin = new Padding(5, 6, 5, 6);
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
        private Button btnBrowseImageExportFolder;
        private TextBox txtImageExportPath;
        private Label label1;
        private Button UpdateCategoriesButton;
    }
}