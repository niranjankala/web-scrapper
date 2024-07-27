namespace CrawlyScraper.App
{
    partial class SplitDataForm
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
            labelFilePath = new Label();
            textBoxFilePath = new TextBox();
            buttonBrowseFile = new Button();
            labelNumFiles = new Label();
            textBoxNumFiles = new TextBox();
            labelTargetFolder = new Label();
            textBoxTargetFolder = new TextBox();
            buttonBrowseFolder = new Button();
            buttonSplitData = new Button();
            progressBar = new ProgressBar();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // labelFilePath
            // 
            labelFilePath.AutoSize = true;
            labelFilePath.Location = new Point(12, 15);
            labelFilePath.Name = "labelFilePath";
            labelFilePath.Size = new Size(149, 30);
            labelFilePath.TabIndex = 0;
            labelFilePath.Text = "Excel File Path:";
            // 
            // textBoxFilePath
            // 
            textBoxFilePath.Location = new Point(21, 54);
            textBoxFilePath.Name = "textBoxFilePath";
            textBoxFilePath.Size = new Size(569, 35);
            textBoxFilePath.TabIndex = 1;
            textBoxFilePath.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\industrybuying\\products_import.xlsx";
            // 
            // buttonBrowseFile
            // 
            buttonBrowseFile.Location = new Point(596, 52);
            buttonBrowseFile.Name = "buttonBrowseFile";
            buttonBrowseFile.Size = new Size(117, 37);
            buttonBrowseFile.TabIndex = 2;
            buttonBrowseFile.Text = "Browse";
            buttonBrowseFile.UseVisualStyleBackColor = true;
            buttonBrowseFile.Click += buttonBrowseFile_Click;
            // 
            // labelNumFiles
            // 
            labelNumFiles.AutoSize = true;
            labelNumFiles.Location = new Point(12, 92);
            labelNumFiles.Name = "labelNumFiles";
            labelNumFiles.Size = new Size(165, 30);
            labelNumFiles.TabIndex = 3;
            labelNumFiles.Text = "Number of Files:";
            // 
            // textBoxNumFiles
            // 
            textBoxNumFiles.Location = new Point(21, 125);
            textBoxNumFiles.Name = "textBoxNumFiles";
            textBoxNumFiles.Size = new Size(100, 35);
            textBoxNumFiles.TabIndex = 4;
            textBoxNumFiles.Text = "150";
            // 
            // labelTargetFolder
            // 
            labelTargetFolder.AutoSize = true;
            labelTargetFolder.Location = new Point(12, 170);
            labelTargetFolder.Name = "labelTargetFolder";
            labelTargetFolder.Size = new Size(138, 30);
            labelTargetFolder.TabIndex = 5;
            labelTargetFolder.Text = "Target Folder:";
            // 
            // textBoxTargetFolder
            // 
            textBoxTargetFolder.Location = new Point(21, 203);
            textBoxTargetFolder.Name = "textBoxTargetFolder";
            textBoxTargetFolder.Size = new Size(569, 35);
            textBoxTargetFolder.TabIndex = 6;
            textBoxTargetFolder.Text = "G:\\My Drive\\Work\\DGIS\\Industry Cart\\industrybuying\\Splitted_Files";
            // 
            // buttonBrowseFolder
            // 
            buttonBrowseFolder.Location = new Point(596, 201);
            buttonBrowseFolder.Name = "buttonBrowseFolder";
            buttonBrowseFolder.Size = new Size(117, 37);
            buttonBrowseFolder.TabIndex = 7;
            buttonBrowseFolder.Text = "Browse";
            buttonBrowseFolder.UseVisualStyleBackColor = true;
            buttonBrowseFolder.Click += buttonBrowseFolder_Click;
            // 
            // buttonSplitData
            // 
            buttonSplitData.Location = new Point(230, 328);
            buttonSplitData.Name = "buttonSplitData";
            buttonSplitData.Size = new Size(241, 71);
            buttonSplitData.TabIndex = 8;
            buttonSplitData.Text = "Split Data";
            buttonSplitData.UseVisualStyleBackColor = true;
            buttonSplitData.Click += buttonSplitData_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(21, 266);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(692, 23);
            progressBar.TabIndex = 9;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(28, 28);
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 444);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(730, 39);
            statusStrip.TabIndex = 10;
            statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(69, 30);
            statusLabel.Text = "Ready";
            // 
            // SplitDataForm
            // 
            ClientSize = new Size(730, 483);
            Controls.Add(statusStrip);
            Controls.Add(progressBar);
            Controls.Add(buttonSplitData);
            Controls.Add(buttonBrowseFolder);
            Controls.Add(textBoxTargetFolder);
            Controls.Add(labelTargetFolder);
            Controls.Add(textBoxNumFiles);
            Controls.Add(labelNumFiles);
            Controls.Add(buttonBrowseFile);
            Controls.Add(textBoxFilePath);
            Controls.Add(labelFilePath);
            Name = "SplitDataForm";
            Text = "Excel Splitter";
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label labelFilePath;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button buttonBrowseFile;
        private System.Windows.Forms.Label labelNumFiles;
        private System.Windows.Forms.TextBox textBoxNumFiles;
        private System.Windows.Forms.Label labelTargetFolder;
        private System.Windows.Forms.TextBox textBoxTargetFolder;
        private System.Windows.Forms.Button buttonBrowseFolder;
        private System.Windows.Forms.Button buttonSplitData;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;

        #endregion
    }
}