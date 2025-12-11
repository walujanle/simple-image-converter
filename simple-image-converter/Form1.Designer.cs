namespace simple_image_converter
{
    partial class Form1
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
            btnSelectFiles = new Button();
            cmbFormat = new ComboBox();
            trackQuality = new TrackBar();
            txtQuality = new TextBox();
            chkKeepMetadata = new CheckBox();
            txtPrefix = new TextBox();
            btnConvert = new Button();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            lstSelectedFiles = new ListBox();
            lblFormat = new Label();
            lblQuality = new Label();
            lblPrefix = new Label();
            lblSelectedFiles = new Label();
            grpNaming = new GroupBox();
            txtReplaceWith = new TextBox();
            txtFindPattern = new TextBox();
            lblReplaceWith = new Label();
            lblFindPattern = new Label();
            chkAutoSuffix = new CheckBox();
            grpResolution = new GroupBox();
            cmbOrientation = new ComboBox();
            lblAspectRatio = new Label();
            txtHeight = new TextBox();
            lblX = new Label();
            txtWidth = new TextBox();
            lblCustomResolution = new Label();
            chkResize = new CheckBox();
            btnDeleteSelected = new Button();
            btnClearAll = new Button();
            grpOutput = new GroupBox();
            btnBrowseOutput = new Button();
            txtOutputPath = new TextBox();
            chkCustomOutput = new CheckBox();
            grpDatasetList = new GroupBox();
            chkAddNumbering = new CheckBox();
            btnGenerateList = new Button();
            chkGenerateTxt = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)trackQuality).BeginInit();
            grpNaming.SuspendLayout();
            grpResolution.SuspendLayout();
            grpOutput.SuspendLayout();
            grpDatasetList.SuspendLayout();
            SuspendLayout();
            // 
            // btnSelectFiles
            // 
            btnSelectFiles.Location = new Point(15, 15);
            btnSelectFiles.Name = "btnSelectFiles";
            btnSelectFiles.Size = new Size(160, 40);
            btnSelectFiles.TabIndex = 0;
            btnSelectFiles.Text = "📁 Select Files";
            btnSelectFiles.UseVisualStyleBackColor = true;
            btnSelectFiles.Click += BtnSelectFiles_Click;
            // 
            // cmbFormat
            // 
            cmbFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFormat.FormattingEnabled = true;
            cmbFormat.Items.AddRange(new object[] { "JPG", "PNG", "WEBP" });
            cmbFormat.Location = new Point(190, 35);
            cmbFormat.Name = "cmbFormat";
            cmbFormat.Size = new Size(130, 23);
            cmbFormat.TabIndex = 2;
            // 
            // trackQuality
            // 
            trackQuality.Location = new Point(340, 35);
            trackQuality.Maximum = 100;
            trackQuality.Minimum = 1;
            trackQuality.Name = "trackQuality";
            trackQuality.Size = new Size(150, 45);
            trackQuality.TabIndex = 4;
            trackQuality.TickFrequency = 10;
            trackQuality.Value = 85;
            trackQuality.Scroll += TrackQuality_Scroll;
            // 
            // txtQuality
            // 
            txtQuality.Location = new Point(500, 35);
            txtQuality.MaxLength = 3;
            txtQuality.Name = "txtQuality";
            txtQuality.Size = new Size(45, 23);
            txtQuality.TabIndex = 5;
            txtQuality.Text = "85";
            txtQuality.TextAlign = HorizontalAlignment.Center;
            txtQuality.TextChanged += TxtQuality_TextChanged;
            // 
            // chkKeepMetadata
            // 
            chkKeepMetadata.AutoSize = true;
            chkKeepMetadata.Location = new Point(575, 38);
            chkKeepMetadata.Name = "chkKeepMetadata";
            chkKeepMetadata.Size = new Size(105, 19);
            chkKeepMetadata.TabIndex = 7;
            chkKeepMetadata.Text = "Keep Metadata";
            chkKeepMetadata.UseVisualStyleBackColor = true;
            // 
            // txtPrefix
            // 
            txtPrefix.Location = new Point(15, 90);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.PlaceholderText = "Example: converted_";
            txtPrefix.Size = new Size(200, 23);
            txtPrefix.TabIndex = 8;
            // 
            // btnConvert
            // 
            btnConvert.Enabled = false;
            btnConvert.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnConvert.Location = new Point(230, 85);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(460, 35);
            btnConvert.TabIndex = 9;
            btnConvert.Text = "🚀 Start Conversion";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Click += BtnConvert_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(15, 585);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(715, 25);
            progressBar.TabIndex = 13;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(15, 565);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(96, 15);
            lblStatus.TabIndex = 12;
            lblStatus.Text = "Ready to process";
            // 
            // lstSelectedFiles
            // 
            lstSelectedFiles.AllowDrop = true;
            lstSelectedFiles.FormattingEnabled = true;
            lstSelectedFiles.Location = new Point(15, 345);
            lstSelectedFiles.Name = "lstSelectedFiles";
            lstSelectedFiles.SelectionMode = SelectionMode.MultiExtended;
            lstSelectedFiles.Size = new Size(595, 139);
            lstSelectedFiles.TabIndex = 11;
            lstSelectedFiles.DragDrop += LstSelectedFiles_DragDrop;
            lstSelectedFiles.DragOver += LstSelectedFiles_DragOver;
            lstSelectedFiles.KeyDown += LstSelectedFiles_KeyDown;
            lstSelectedFiles.MouseDown += LstSelectedFiles_MouseDown;
            // 
            // lblFormat
            // 
            lblFormat.AutoSize = true;
            lblFormat.Location = new Point(190, 15);
            lblFormat.Name = "lblFormat";
            lblFormat.Size = new Size(89, 15);
            lblFormat.TabIndex = 1;
            lblFormat.Text = "Output Format:";
            // 
            // lblQuality
            // 
            lblQuality.AutoSize = true;
            lblQuality.Location = new Point(340, 15);
            lblQuality.Name = "lblQuality";
            lblQuality.Size = new Size(88, 15);
            lblQuality.TabIndex = 3;
            lblQuality.Text = "Quality (0-100):";
            // 
            // lblPrefix
            // 
            lblPrefix.AutoSize = true;
            lblPrefix.Location = new Point(15, 70);
            lblPrefix.Name = "lblPrefix";
            lblPrefix.Size = new Size(90, 15);
            lblPrefix.TabIndex = 7;
            lblPrefix.Text = "Filename Prefix:";
            // 
            // lblSelectedFiles
            // 
            lblSelectedFiles.AutoSize = true;
            lblSelectedFiles.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSelectedFiles.Location = new Point(15, 325);
            lblSelectedFiles.Name = "lblSelectedFiles";
            lblSelectedFiles.Size = new Size(206, 15);
            lblSelectedFiles.TabIndex = 10;
            lblSelectedFiles.Text = "Selected Files (0) - Drag to Reorder:";
            // 
            // grpNaming
            // 
            grpNaming.Controls.Add(txtReplaceWith);
            grpNaming.Controls.Add(txtFindPattern);
            grpNaming.Controls.Add(lblReplaceWith);
            grpNaming.Controls.Add(lblFindPattern);
            grpNaming.Controls.Add(chkAutoSuffix);
            grpNaming.Location = new Point(15, 125);
            grpNaming.Name = "grpNaming";
            grpNaming.Size = new Size(460, 105);
            grpNaming.TabIndex = 14;
            grpNaming.TabStop = false;
            grpNaming.Text = "Filename Pattern Options";
            // 
            // txtReplaceWith
            // 
            txtReplaceWith.Location = new Point(150, 67);
            txtReplaceWith.Name = "txtReplaceWith";
            txtReplaceWith.PlaceholderText = "Leave empty to remove";
            txtReplaceWith.Size = new Size(150, 23);
            txtReplaceWith.TabIndex = 3;
            // 
            // txtFindPattern
            // 
            txtFindPattern.Location = new Point(10, 43);
            txtFindPattern.Name = "txtFindPattern";
            txtFindPattern.PlaceholderText = "e.g., your-file-name";
            txtFindPattern.Size = new Size(440, 23);
            txtFindPattern.TabIndex = 1;
            txtFindPattern.TextChanged += txtFindPattern_TextChanged;
            // 
            // lblReplaceWith
            // 
            lblReplaceWith.AutoSize = true;
            lblReplaceWith.Location = new Point(10, 70);
            lblReplaceWith.Name = "lblReplaceWith";
            lblReplaceWith.Size = new Size(136, 15);
            lblReplaceWith.TabIndex = 2;
            lblReplaceWith.Text = "Replace With (Optional):";
            // 
            // lblFindPattern
            // 
            lblFindPattern.AutoSize = true;
            lblFindPattern.Location = new Point(10, 25);
            lblFindPattern.Name = "lblFindPattern";
            lblFindPattern.Size = new Size(111, 15);
            lblFindPattern.TabIndex = 0;
            lblFindPattern.Text = "Find Text (Remove):";
            // 
            // chkAutoSuffix
            // 
            chkAutoSuffix.AutoSize = true;
            chkAutoSuffix.Location = new Point(310, 69);
            chkAutoSuffix.Name = "chkAutoSuffix";
            chkAutoSuffix.Size = new Size(141, 19);
            chkAutoSuffix.TabIndex = 4;
            chkAutoSuffix.Text = "Auto Suffix (res+qual)";
            chkAutoSuffix.UseVisualStyleBackColor = true;
            // 
            // grpResolution
            // 
            grpResolution.Controls.Add(cmbOrientation);
            grpResolution.Controls.Add(lblAspectRatio);
            grpResolution.Controls.Add(txtHeight);
            grpResolution.Controls.Add(lblX);
            grpResolution.Controls.Add(txtWidth);
            grpResolution.Controls.Add(lblCustomResolution);
            grpResolution.Controls.Add(chkResize);
            grpResolution.Location = new Point(490, 125);
            grpResolution.Name = "grpResolution";
            grpResolution.Size = new Size(240, 105);
            grpResolution.TabIndex = 15;
            grpResolution.TabStop = false;
            grpResolution.Text = "Resolution Scaling";
            // 
            // cmbOrientation
            // 
            cmbOrientation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOrientation.Enabled = false;
            cmbOrientation.FormattingEnabled = true;
            cmbOrientation.Items.AddRange(new object[] { "Auto", "Horizontal", "Vertical" });
            cmbOrientation.Location = new Point(120, 20);
            cmbOrientation.Name = "cmbOrientation";
            cmbOrientation.Size = new Size(110, 23);
            cmbOrientation.TabIndex = 1;
            cmbOrientation.SelectedIndexChanged += CmbOrientation_SelectedIndexChanged;
            // 
            // lblAspectRatio
            // 
            lblAspectRatio.AutoSize = true;
            lblAspectRatio.Font = new Font("Segoe UI", 8F);
            lblAspectRatio.ForeColor = SystemColors.GrayText;
            lblAspectRatio.Location = new Point(158, 71);
            lblAspectRatio.Name = "lblAspectRatio";
            lblAspectRatio.Size = new Size(11, 13);
            lblAspectRatio.TabIndex = 6;
            lblAspectRatio.Text = "-";
            // 
            // txtHeight
            // 
            txtHeight.Enabled = false;
            txtHeight.Location = new Point(92, 68);
            txtHeight.Name = "txtHeight";
            txtHeight.PlaceholderText = "Height";
            txtHeight.Size = new Size(60, 23);
            txtHeight.TabIndex = 5;
            txtHeight.TextChanged += TxtResolution_TextChanged;
            // 
            // lblX
            // 
            lblX.AutoSize = true;
            lblX.Location = new Point(75, 71);
            lblX.Name = "lblX";
            lblX.Size = new Size(12, 15);
            lblX.TabIndex = 4;
            lblX.Text = "x";
            // 
            // txtWidth
            // 
            txtWidth.Enabled = false;
            txtWidth.Location = new Point(10, 68);
            txtWidth.Name = "txtWidth";
            txtWidth.PlaceholderText = "Width";
            txtWidth.Size = new Size(60, 23);
            txtWidth.TabIndex = 3;
            txtWidth.TextChanged += TxtResolution_TextChanged;
            // 
            // lblCustomResolution
            // 
            lblCustomResolution.AutoSize = true;
            lblCustomResolution.Location = new Point(10, 50);
            lblCustomResolution.Name = "lblCustomResolution";
            lblCustomResolution.Size = new Size(66, 15);
            lblCustomResolution.TabIndex = 2;
            lblCustomResolution.Text = "Resolution:";
            // 
            // chkResize
            // 
            chkResize.AutoSize = true;
            chkResize.Location = new Point(10, 22);
            chkResize.Name = "chkResize";
            chkResize.Size = new Size(96, 19);
            chkResize.TabIndex = 0;
            chkResize.Text = "Enable Resize";
            chkResize.UseVisualStyleBackColor = true;
            chkResize.CheckedChanged += ChkResize_CheckedChanged;
            // 
            // btnDeleteSelected
            // 
            btnDeleteSelected.Location = new Point(620, 345);
            btnDeleteSelected.Name = "btnDeleteSelected";
            btnDeleteSelected.Size = new Size(110, 35);
            btnDeleteSelected.TabIndex = 16;
            btnDeleteSelected.Text = "🗑️ Delete Selected";
            btnDeleteSelected.UseVisualStyleBackColor = true;
            btnDeleteSelected.Click += BtnDeleteSelected_Click;
            // 
            // btnClearAll
            // 
            btnClearAll.Location = new Point(620, 385);
            btnClearAll.Name = "btnClearAll";
            btnClearAll.Size = new Size(110, 35);
            btnClearAll.TabIndex = 17;
            btnClearAll.Text = "Clear All";
            btnClearAll.UseVisualStyleBackColor = true;
            btnClearAll.Click += BtnClearAll_Click;
            // 
            // grpOutput
            // 
            grpOutput.Controls.Add(btnBrowseOutput);
            grpOutput.Controls.Add(txtOutputPath);
            grpOutput.Controls.Add(chkCustomOutput);
            grpOutput.Location = new Point(15, 235);
            grpOutput.Name = "grpOutput";
            grpOutput.Size = new Size(460, 80);
            grpOutput.TabIndex = 18;
            grpOutput.TabStop = false;
            grpOutput.Text = "Output Location";
            // 
            // btnBrowseOutput
            // 
            btnBrowseOutput.Enabled = false;
            btnBrowseOutput.Location = new Point(370, 45);
            btnBrowseOutput.Name = "btnBrowseOutput";
            btnBrowseOutput.Size = new Size(80, 25);
            btnBrowseOutput.TabIndex = 2;
            btnBrowseOutput.Text = "Browse...";
            btnBrowseOutput.UseVisualStyleBackColor = true;
            btnBrowseOutput.Click += BtnBrowseOutput_Click;
            // 
            // txtOutputPath
            // 
            txtOutputPath.Enabled = false;
            txtOutputPath.Location = new Point(10, 46);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.PlaceholderText = "Select output folder...";
            txtOutputPath.ReadOnly = true;
            txtOutputPath.Size = new Size(355, 23);
            txtOutputPath.TabIndex = 1;
            // 
            // chkCustomOutput
            // 
            chkCustomOutput.AutoSize = true;
            chkCustomOutput.Location = new Point(10, 22);
            chkCustomOutput.Name = "chkCustomOutput";
            chkCustomOutput.Size = new Size(248, 19);
            chkCustomOutput.TabIndex = 0;
            chkCustomOutput.Text = "Use Custom Output Folder (same as input)";
            chkCustomOutput.UseVisualStyleBackColor = true;
            chkCustomOutput.CheckedChanged += ChkCustomOutput_CheckedChanged;
            // 
            // grpDatasetList
            // 
            grpDatasetList.Controls.Add(chkAddNumbering);
            grpDatasetList.Controls.Add(btnGenerateList);
            grpDatasetList.Controls.Add(chkGenerateTxt);
            grpDatasetList.Location = new Point(490, 235);
            grpDatasetList.Name = "grpDatasetList";
            grpDatasetList.Size = new Size(240, 80);
            grpDatasetList.TabIndex = 19;
            grpDatasetList.TabStop = false;
            grpDatasetList.Text = "Dataset List File";
            // 
            // chkAddNumbering
            // 
            chkAddNumbering.AutoSize = true;
            chkAddNumbering.Enabled = false;
            chkAddNumbering.Location = new Point(10, 45);
            chkAddNumbering.Name = "chkAddNumbering";
            chkAddNumbering.Size = new Size(111, 19);
            chkAddNumbering.TabIndex = 2;
            chkAddNumbering.Text = "Add Numbering";
            chkAddNumbering.UseVisualStyleBackColor = true;
            // 
            // btnGenerateList
            // 
            btnGenerateList.Enabled = false;
            btnGenerateList.Location = new Point(130, 42);
            btnGenerateList.Name = "btnGenerateList";
            btnGenerateList.Size = new Size(100, 25);
            btnGenerateList.TabIndex = 1;
            btnGenerateList.Text = "Generate TXT";
            btnGenerateList.UseVisualStyleBackColor = true;
            btnGenerateList.Click += BtnGenerateList_Click;
            // 
            // chkGenerateTxt
            // 
            chkGenerateTxt.AutoSize = true;
            chkGenerateTxt.Location = new Point(10, 22);
            chkGenerateTxt.Name = "chkGenerateTxt";
            chkGenerateTxt.Size = new Size(191, 19);
            chkGenerateTxt.TabIndex = 0;
            chkGenerateTxt.Text = "Generate TXT List After Convert";
            chkGenerateTxt.UseVisualStyleBackColor = true;
            chkGenerateTxt.CheckedChanged += ChkGenerateTxt_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(745, 625);
            Controls.Add(grpDatasetList);
            Controls.Add(grpOutput);
            Controls.Add(btnClearAll);
            Controls.Add(btnDeleteSelected);
            Controls.Add(grpResolution);
            Controls.Add(grpNaming);
            Controls.Add(progressBar);
            Controls.Add(lblStatus);
            Controls.Add(lstSelectedFiles);
            Controls.Add(lblSelectedFiles);
            Controls.Add(btnConvert);
            Controls.Add(txtPrefix);
            Controls.Add(lblPrefix);
            Controls.Add(chkKeepMetadata);
            Controls.Add(txtQuality);
            Controls.Add(trackQuality);
            Controls.Add(lblQuality);
            Controls.Add(cmbFormat);
            Controls.Add(lblFormat);
            Controls.Add(btnSelectFiles);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Image Converter Portable - Secure Edition";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)trackQuality).EndInit();
            grpNaming.ResumeLayout(false);
            grpNaming.PerformLayout();
            grpResolution.ResumeLayout(false);
            grpResolution.PerformLayout();
            grpOutput.ResumeLayout(false);
            grpOutput.PerformLayout();
            grpDatasetList.ResumeLayout(false);
            grpDatasetList.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSelectFiles;
        private ComboBox cmbFormat;
        private TrackBar trackQuality;
        private TextBox txtQuality;
        private CheckBox chkKeepMetadata;
        private TextBox txtPrefix;
        private Button btnConvert;
        private ProgressBar progressBar;
        private Label lblStatus;
        private ListBox lstSelectedFiles;
        private Label lblFormat;
        private Label lblQuality;
        private Label lblPrefix;
        private Label lblSelectedFiles;
        private GroupBox grpNaming;
        private TextBox txtReplaceWith;
        private TextBox txtFindPattern;
        private Label lblReplaceWith;
        private Label lblFindPattern;
        private CheckBox chkAutoSuffix;
        private GroupBox grpResolution;
        private ComboBox cmbOrientation;
        private Label lblAspectRatio;
        private TextBox txtHeight;
        private Label lblX;
        private TextBox txtWidth;
        private Label lblCustomResolution;
        private CheckBox chkResize;
        private Button btnDeleteSelected;
        private Button btnClearAll;
        private GroupBox grpOutput;
        private Button btnBrowseOutput;
        private TextBox txtOutputPath;
        private CheckBox chkCustomOutput;
        private GroupBox grpDatasetList;
        private CheckBox chkAddNumbering;
        private Button btnGenerateList;
        private CheckBox chkGenerateTxt;
    }
}
