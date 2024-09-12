namespace CrawlyScraper.App
{
    partial class MergeDataForm
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
            lblCategoriesPath = new Label();
            txtCategoriesPath = new TextBox();
            btnBrowseCategories = new Button();
            lblCategoriesFolderPath = new Label();
            txtCategoriesFolderPath = new TextBox();
            btnBrowseFolder = new Button();
            lblExportPath = new Label();
            txtExportPath = new TextBox();
            btnBrowseExport = new Button();
            progressBar = new ProgressBar();
            statusBar = new StatusStrip();
            ProgressStatusLabel = new ToolStripStatusLabel();
            btnMergeData = new Button();
            statusBar.SuspendLayout();
            SuspendLayout();
            // 
            // lblCategoriesPath
            // 
            lblCategoriesPath.AutoSize = true;
            lblCategoriesPath.Location = new Point(14, 10);
            lblCategoriesPath.Margin = new Padding(4, 0, 4, 0);
            lblCategoriesPath.Name = "lblCategoriesPath";
            lblCategoriesPath.Size = new Size(123, 15);
            lblCategoriesPath.TabIndex = 0;
            lblCategoriesPath.Text = "Categories Excel Path:";
            // 
            // txtCategoriesPath
            // 
            txtCategoriesPath.Location = new Point(14, 29);
            txtCategoriesPath.Margin = new Padding(4, 4, 4, 4);
            txtCategoriesPath.Name = "txtCategoriesPath";
            txtCategoriesPath.Size = new Size(349, 23);
            txtCategoriesPath.TabIndex = 1;
            txtCategoriesPath.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\categories-2024-07-22.xlsx";
            // 
            // btnBrowseCategories
            // 
            btnBrowseCategories.Location = new Point(371, 26);
            btnBrowseCategories.Margin = new Padding(4, 4, 4, 4);
            btnBrowseCategories.Name = "btnBrowseCategories";
            btnBrowseCategories.Size = new Size(88, 26);
            btnBrowseCategories.TabIndex = 2;
            btnBrowseCategories.Text = "Browse...";
            btnBrowseCategories.UseVisualStyleBackColor = true;
            btnBrowseCategories.Click += btnBrowseCategories_Click;
            // 
            // lblCategoriesFolderPath
            // 
            lblCategoriesFolderPath.AutoSize = true;
            lblCategoriesFolderPath.Location = new Point(14, 67);
            lblCategoriesFolderPath.Margin = new Padding(4, 0, 4, 0);
            lblCategoriesFolderPath.Name = "lblCategoriesFolderPath";
            lblCategoriesFolderPath.Size = new Size(129, 15);
            lblCategoriesFolderPath.TabIndex = 3;
            lblCategoriesFolderPath.Text = "Categories Folder Path:";
            // 
            // txtCategoriesFolderPath
            // 
            txtCategoriesFolderPath.Location = new Point(14, 86);
            txtCategoriesFolderPath.Margin = new Padding(4, 4, 4, 4);
            txtCategoriesFolderPath.Name = "txtCategoriesFolderPath";
            txtCategoriesFolderPath.Size = new Size(349, 23);
            txtCategoriesFolderPath.TabIndex = 4;
            txtCategoriesFolderPath.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\industrybuying\\Categories";
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Location = new Point(371, 83);
            btnBrowseFolder.Margin = new Padding(4, 4, 4, 4);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(88, 26);
            btnBrowseFolder.TabIndex = 5;
            btnBrowseFolder.Text = "Browse...";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // lblExportPath
            // 
            lblExportPath.AutoSize = true;
            lblExportPath.Location = new Point(14, 124);
            lblExportPath.Margin = new Padding(4, 0, 4, 0);
            lblExportPath.Name = "lblExportPath";
            lblExportPath.Size = new Size(114, 15);
            lblExportPath.TabIndex = 6;
            lblExportPath.Text = "Exported Excel Path:";
            // 
            // txtExportPath
            // 
            txtExportPath.Location = new Point(14, 142);
            txtExportPath.Margin = new Padding(4, 4, 4, 4);
            txtExportPath.Name = "txtExportPath";
            txtExportPath.Size = new Size(349, 23);
            txtExportPath.TabIndex = 7;
            txtExportPath.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\industrybuying\\products.xlsx";
            // 
            // btnBrowseExport
            // 
            btnBrowseExport.Location = new Point(371, 140);
            btnBrowseExport.Margin = new Padding(4, 4, 4, 4);
            btnBrowseExport.Name = "btnBrowseExport";
            btnBrowseExport.Size = new Size(88, 25);
            btnBrowseExport.TabIndex = 8;
            btnBrowseExport.Text = "Browse...";
            btnBrowseExport.UseVisualStyleBackColor = true;
            btnBrowseExport.Click += btnBrowseExport_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(14, 184);
            progressBar.Margin = new Padding(4, 4, 4, 4);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(444, 26);
            progressBar.TabIndex = 9;
            // 
            // statusBar
            // 
            statusBar.ImageScalingSize = new Size(28, 28);
            statusBar.Items.AddRange(new ToolStripItem[] { ProgressStatusLabel });
            statusBar.Location = new Point(0, 279);
            statusBar.Name = "statusBar";
            statusBar.Padding = new Padding(1, 0, 16, 0);
            statusBar.Size = new Size(472, 22);
            statusBar.TabIndex = 10;
            // 
            // ProgressStatusLabel
            // 
            ProgressStatusLabel.Name = "ProgressStatusLabel";
            ProgressStatusLabel.Size = new Size(16, 17);
            ProgressStatusLabel.Text = "...";
            // 
            // btnMergeData
            // 
            btnMergeData.Location = new Point(184, 247);
            btnMergeData.Margin = new Padding(4, 4, 4, 4);
            btnMergeData.Name = "btnMergeData";
            btnMergeData.Size = new Size(88, 26);
            btnMergeData.TabIndex = 11;
            btnMergeData.Text = "Merge Data";
            btnMergeData.UseVisualStyleBackColor = true;
            btnMergeData.Click += btnMergeData_Click;
            // 
            // MergeDataForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(472, 301);
            Controls.Add(btnMergeData);
            Controls.Add(statusBar);
            Controls.Add(progressBar);
            Controls.Add(btnBrowseExport);
            Controls.Add(txtExportPath);
            Controls.Add(lblExportPath);
            Controls.Add(btnBrowseFolder);
            Controls.Add(txtCategoriesFolderPath);
            Controls.Add(lblCategoriesFolderPath);
            Controls.Add(btnBrowseCategories);
            Controls.Add(txtCategoriesPath);
            Controls.Add(lblCategoriesPath);
            Margin = new Padding(4, 4, 4, 4);
            Name = "MergeDataForm";
            Text = "Category Data Merger";
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblCategoriesPath;
        private System.Windows.Forms.Label lblCategoriesFolderPath;
        private System.Windows.Forms.Label lblExportPath;
        private System.Windows.Forms.TextBox txtCategoriesPath;
        private System.Windows.Forms.Button btnBrowseCategories;
        private System.Windows.Forms.TextBox txtCategoriesFolderPath;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.TextBox txtExportPath;
        private System.Windows.Forms.Button btnBrowseExport;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.Button btnMergeData;


        #endregion

        private ToolStripStatusLabel ProgressStatusLabel;
    }
}