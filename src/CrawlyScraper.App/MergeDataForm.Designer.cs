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
            btnMergeData = new Button();
            SuspendLayout();
            // 
            // lblCategoriesPath
            // 
            lblCategoriesPath.AutoSize = true;
            lblCategoriesPath.Location = new Point(24, 21);
            lblCategoriesPath.Margin = new Padding(6, 0, 6, 0);
            lblCategoriesPath.Name = "lblCategoriesPath";
            lblCategoriesPath.Size = new Size(216, 30);
            lblCategoriesPath.TabIndex = 0;
            lblCategoriesPath.Text = "Categories Excel Path:";
            // 
            // txtCategoriesPath
            // 
            txtCategoriesPath.Location = new Point(24, 58);
            txtCategoriesPath.Margin = new Padding(6, 7, 6, 7);
            txtCategoriesPath.Name = "txtCategoriesPath";
            txtCategoriesPath.Size = new Size(596, 35);
            txtCategoriesPath.TabIndex = 1;
            // 
            // btnBrowseCategories
            // 
            btnBrowseCategories.Location = new Point(636, 53);
            btnBrowseCategories.Margin = new Padding(6, 7, 6, 7);
            btnBrowseCategories.Name = "btnBrowseCategories";
            btnBrowseCategories.Size = new Size(150, 40);
            btnBrowseCategories.TabIndex = 2;
            btnBrowseCategories.Text = "Browse...";
            btnBrowseCategories.UseVisualStyleBackColor = true;
            btnBrowseCategories.Click += btnBrowseCategories_Click;
            // 
            // lblCategoriesFolderPath
            // 
            lblCategoriesFolderPath.AutoSize = true;
            lblCategoriesFolderPath.Location = new Point(24, 134);
            lblCategoriesFolderPath.Margin = new Padding(6, 0, 6, 0);
            lblCategoriesFolderPath.Name = "lblCategoriesFolderPath";
            lblCategoriesFolderPath.Size = new Size(226, 30);
            lblCategoriesFolderPath.TabIndex = 3;
            lblCategoriesFolderPath.Text = "Categories Folder Path:";
            // 
            // txtCategoriesFolderPath
            // 
            txtCategoriesFolderPath.Location = new Point(24, 171);
            txtCategoriesFolderPath.Margin = new Padding(6, 7, 6, 7);
            txtCategoriesFolderPath.Name = "txtCategoriesFolderPath";
            txtCategoriesFolderPath.Size = new Size(596, 35);
            txtCategoriesFolderPath.TabIndex = 4;            
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Location = new Point(636, 166);
            btnBrowseFolder.Margin = new Padding(6, 7, 6, 7);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(150, 40);
            btnBrowseFolder.TabIndex = 5;
            btnBrowseFolder.Text = "Browse...";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // lblExportPath
            // 
            lblExportPath.AutoSize = true;
            lblExportPath.Location = new Point(24, 247);
            lblExportPath.Margin = new Padding(6, 0, 6, 0);
            lblExportPath.Name = "lblExportPath";
            lblExportPath.Size = new Size(200, 30);
            lblExportPath.TabIndex = 6;
            lblExportPath.Text = "Exported Excel Path:";
            // 
            // txtExportPath
            // 
            txtExportPath.Location = new Point(24, 284);
            txtExportPath.Margin = new Padding(6, 7, 6, 7);
            txtExportPath.Name = "txtExportPath";
            txtExportPath.Size = new Size(596, 35);
            txtExportPath.TabIndex = 7;
            // 
            // btnBrowseExport
            // 
            btnBrowseExport.Location = new Point(636, 279);
            btnBrowseExport.Margin = new Padding(6, 7, 6, 7);
            btnBrowseExport.Name = "btnBrowseExport";
            btnBrowseExport.Size = new Size(150, 40);
            btnBrowseExport.TabIndex = 8;
            btnBrowseExport.Text = "Browse...";
            btnBrowseExport.UseVisualStyleBackColor = true;
            btnBrowseExport.Click += btnBrowseExport_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(24, 369);
            progressBar.Margin = new Padding(6, 7, 6, 7);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(762, 53);
            progressBar.TabIndex = 9;
            // 
            // statusBar
            // 
            statusBar.ImageScalingSize = new Size(28, 28);
            statusBar.Location = new Point(0, 580);
            statusBar.Name = "statusBar";
            statusBar.Padding = new Padding(2, 0, 28, 0);
            statusBar.Size = new Size(810, 22);
            statusBar.TabIndex = 10;
            // 
            // btnMergeData
            // 
            btnMergeData.Location = new Point(318, 531);
            btnMergeData.Margin = new Padding(6, 7, 6, 7);
            btnMergeData.Name = "btnMergeData";
            btnMergeData.Size = new Size(150, 53);
            btnMergeData.TabIndex = 11;
            btnMergeData.Text = "Merge Data";
            btnMergeData.UseVisualStyleBackColor = true;
            btnMergeData.Click += btnMergeData_Click;
            // 
            // MergeDataForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(810, 602);
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
            Margin = new Padding(6, 7, 6, 7);
            Name = "MergeDataForm";
            Text = "Category Data Merger";
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
    }
}