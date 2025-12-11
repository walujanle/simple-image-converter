using ImageMagick;

namespace simple_image_converter
{
    public partial class Form1 : Form
    {
        private List<string> selectedFiles = new List<string>();
        private int dragIndex = -1;
        private string customOutputPath = string.Empty;
        private List<string> convertedFileNames = new List<string>();
        
        private static readonly byte[][] SupportedMagicNumbers = new byte[][]
        {
            new byte[] { 0xFF, 0xD8, 0xFF },
            new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
            new byte[] { 0x52, 0x49, 0x46, 0x46 },
            new byte[] { 0x47, 0x49, 0x46 },
            new byte[] { 0x42, 0x4D },
            new byte[] { 0x49, 0x49, 0x2A, 0x00 },
            new byte[] { 0x4D, 0x4D, 0x00, 0x2A },
            new byte[] { 0x00, 0x00, 0x00 }
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (cmbFormat != null && cmbFormat.Items.Count > 0)
                {
                    cmbFormat.SelectedIndex = 0;
                }

                if (cmbOrientation != null && cmbOrientation.Items.Count > 0)
                {
                    cmbOrientation.SelectedIndex = 0;
                }

                if (txtQuality != null && trackQuality != null)
                {
                    txtQuality.Text = trackQuality.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing form: {ex.Message}\n\nThe application may not work correctly.",
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void TrackQuality_Scroll(object sender, EventArgs e)
        {
            txtQuality.Text = trackQuality.Value.ToString();
        }

        private void TxtQuality_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuality.Text))
                return;

            if (int.TryParse(txtQuality.Text, out int quality))
            {
                if (quality >= 1 && quality <= 100)
                {
                    if (trackQuality.Value != quality)
                    {
                        trackQuality.Value = quality;
                    }
                }
                else if (quality > 100)
                {
                    txtQuality.Text = "100";
                    txtQuality.SelectionStart = txtQuality.Text.Length;
                }
                else if (quality < 1)
                {
                    txtQuality.Text = "1";
                    txtQuality.SelectionStart = txtQuality.Text.Length;
                }
            }
            else
            {
                int cursorPosition = txtQuality.SelectionStart;
                txtQuality.Text = trackQuality.Value.ToString();
                txtQuality.SelectionStart = Math.Max(0, cursorPosition - 1);
            }
        }

        private void ChkResize_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkResize.Checked;
            txtWidth.Enabled = enabled;
            txtHeight.Enabled = enabled;
            cmbOrientation.Enabled = enabled;

            if (!enabled)
            {
                txtWidth.Clear();
                txtHeight.Clear();
                lblAspectRatio.Text = "-";
            }
        }

        private void TxtResolution_TextChanged(object sender, EventArgs e)
        {
            UpdateAspectRatioLabel();
        }

        private void CmbOrientation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbOrientation.SelectedIndex == 0)
            {
                UpdateAspectRatioLabel();
            }
        }

        private void UpdateAspectRatioLabel()
        {
            if (int.TryParse(txtWidth.Text, out int width) &&
                int.TryParse(txtHeight.Text, out int height) &&
                width > 0 && height > 0)
            {
                int gcd = GCD(width, height);
                int ratioW = width / gcd;
                int ratioH = height / gcd;

                string orientation = width >= height ? "Horizontal" : "Vertical";
                if (cmbOrientation.SelectedIndex == 0)
                {
                    Invoke(new Action(() =>
                    {
                        cmbOrientation.SelectedIndex = width >= height ? 1 : 2;
                    }));
                }

                lblAspectRatio.Text = $"{ratioW}:{ratioH} ({orientation})";
            }
            else
            {
                lblAspectRatio.Text = "-";
            }
        }

        private int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        #region Drag & Drop and List Management

        private void LstSelectedFiles_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstSelectedFiles.Items.Count == 0)
                return;

            dragIndex = lstSelectedFiles.IndexFromPoint(e.X, e.Y);
            if (dragIndex != ListBox.NoMatches)
            {
                lstSelectedFiles.DoDragDrop(lstSelectedFiles.Items[dragIndex], DragDropEffects.Move);
            }
        }

        private void LstSelectedFiles_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void LstSelectedFiles_DragDrop(object sender, DragEventArgs e)
        {
            Point point = lstSelectedFiles.PointToClient(new Point(e.X, e.Y));
            int targetIndex = lstSelectedFiles.IndexFromPoint(point);

            if (targetIndex == ListBox.NoMatches)
                targetIndex = lstSelectedFiles.Items.Count - 1;

            if (dragIndex != -1 && dragIndex != targetIndex)
            {
                string draggedItem = selectedFiles[dragIndex];
                selectedFiles.RemoveAt(dragIndex);
                selectedFiles.Insert(targetIndex, draggedItem);

                RefreshFileList();
            }
        }

        private void LstSelectedFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                BtnDeleteSelected_Click(sender, e);
            }
        }

        private void BtnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (lstSelectedFiles.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Please select files to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var indicesToRemove = lstSelectedFiles.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();

            foreach (int index in indicesToRemove)
            {
                if (index >= 0 && index < selectedFiles.Count)
                {
                    selectedFiles.RemoveAt(index);
                }
            }

            RefreshFileList();
            UpdateFileCount();
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            if (selectedFiles.Count == 0)
                return;

            var result = MessageBox.Show(
                "Are you sure you want to clear all files?",
                "Confirm Clear",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                selectedFiles.Clear();
                RefreshFileList();
                UpdateFileCount();
            }
        }

        private void RefreshFileList()
        {
            lstSelectedFiles.Items.Clear();
            foreach (string file in selectedFiles)
            {
                lstSelectedFiles.Items.Add(Path.GetFileName(file));
            }
        }

        private void UpdateFileCount()
        {
            lblSelectedFiles.Text = $"Selected Files ({selectedFiles.Count}) - Drag to Reorder:";
            btnConvert.Enabled = selectedFiles.Count > 0;
            lblStatus.Text = selectedFiles.Count > 0
                ? $"Ready to convert {selectedFiles.Count} file(s)"
                : "Ready to process";
        }

        #endregion

        private void BtnSelectFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.tiff;*.tif|All Files|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "Select Image Files";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var validFiles = new List<string>();
                    var invalidFiles = new List<string>();

                    foreach (string file in openFileDialog.FileNames)
                    {
                        if (ValidateImageFile(file))
                        {
                            if (!selectedFiles.Contains(file))
                            {
                                validFiles.Add(file);
                            }
                        }
                        else
                        {
                            invalidFiles.Add(Path.GetFileName(file));
                        }
                    }

                    selectedFiles.AddRange(validFiles);
                    RefreshFileList();
                    UpdateFileCount();

                    if (invalidFiles.Count > 0)
                    {
                        MessageBox.Show(
                            $"The following files are invalid or not image files:\n\n{string.Join("\n", invalidFiles.Take(10))}" +
                            (invalidFiles.Count > 10 ? $"\n\n...and {invalidFiles.Count - 10} more files" : ""),
                            "Invalid Files",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private bool ValidateImageFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length == 0 || fileInfo.Length > 100 * 1024 * 1024)
                    return false;

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] headerBytes = new byte[8];
                    int bytesRead = fs.Read(headerBytes, 0, headerBytes.Length);

                    if (bytesRead < 3)
                        return false;

                    foreach (var magicNumber in SupportedMagicNumbers)
                    {
                        if (bytesRead >= magicNumber.Length)
                        {
                            bool match = true;
                            for (int i = 0; i < magicNumber.Length; i++)
                            {
                                if (headerBytes[i] != magicNumber[i])
                                {
                                    match = false;
                                    break;
                                }
                            }
                            if (match)
                                return true;
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void ChkCustomOutput_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkCustomOutput.Checked;
            txtOutputPath.Enabled = enabled;
            btnBrowseOutput.Enabled = enabled;

            if (!enabled)
            {
                txtOutputPath.Clear();
                customOutputPath = string.Empty;
            }
        }

        private void BtnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select output folder for converted images";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    customOutputPath = folderDialog.SelectedPath;
                    txtOutputPath.Text = customOutputPath;
                }
            }
        }

        private void ChkGenerateTxt_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkGenerateTxt.Checked;
            chkAddNumbering.Enabled = enabled;
            btnGenerateList.Enabled = enabled && convertedFileNames.Count > 0;
        }

        private void BtnGenerateList_Click(object sender, EventArgs e)
        {
            if (convertedFileNames.Count == 0)
            {
                MessageBox.Show(
                    "No converted files available. Please convert images first!",
                    "No Files",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            GenerateDatasetListFile();
        }

        private void GenerateDatasetListFile()
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Text Files|*.txt";
                    saveDialog.Title = "Save Dataset List File";
                    saveDialog.FileName = "dataset_list.txt";
                    
                    string outputDir = chkCustomOutput.Checked && !string.IsNullOrEmpty(customOutputPath)
                        ? customOutputPath
                        : (selectedFiles.Count > 0 ? Path.GetDirectoryName(selectedFiles[0]) : "");
                    
                    if (!string.IsNullOrEmpty(outputDir))
                    {
                        saveDialog.InitialDirectory = outputDir;
                    }

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        bool addNumbering = chkAddNumbering.Checked;
                        var lines = new List<string>();

                        for (int i = 0; i < convertedFileNames.Count; i++)
                        {
                            string fileName = Path.GetFileName(convertedFileNames[i]);
                            
                            if (addNumbering)
                            {
                                lines.Add($"{i + 1}. {fileName}");
                            }
                            else
                            {
                                lines.Add(fileName);
                            }
                        }

                        File.WriteAllLines(saveDialog.FileName, lines);

                        MessageBox.Show(
                            $"Dataset list file created successfully!\n\nLocation: {saveDialog.FileName}\nTotal files: {lines.Count}",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error creating dataset list file:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void BtnConvert_Click(object sender, EventArgs e)
        {
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("Please select files first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbFormat.SelectedIndex < 0)
            {
                MessageBox.Show("Please select output format!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chkResize.Checked && (!int.TryParse(txtWidth.Text, out int w) || !int.TryParse(txtHeight.Text, out int h) || w <= 0 || h <= 0))
            {
                MessageBox.Show("Please enter valid resolution values!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chkCustomOutput.Checked && string.IsNullOrEmpty(customOutputPath))
            {
                MessageBox.Show("Please select an output folder!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConvert.Enabled = false;
            btnSelectFiles.Enabled = false;
            btnDeleteSelected.Enabled = false;
            btnClearAll.Enabled = false;
            progressBar.Value = 0;
            progressBar.Maximum = selectedFiles.Count;

            string outputFormat = cmbFormat.SelectedItem?.ToString() ?? "JPG";
            int quality = trackQuality.Value;
            bool keepMetadata = chkKeepMetadata.Checked;
            string prefix = SanitizeString(txtPrefix.Text);
            string findPattern = txtFindPattern.Text;
            string replaceWith = txtReplaceWith.Text;
            bool autoSuffix = chkAutoSuffix.Checked;
            bool resize = chkResize.Checked;
            var targetResolution = resize ? GetCustomResolution() : (0, 0);
            bool useCustomOutput = chkCustomOutput.Checked;
            string outputPath = customOutputPath;

            int successCount = 0;
            int failCount = 0;
            var errorMessages = new List<string>();
            convertedFileNames.Clear();

            await Task.Run(() =>
            {
                for (int i = 0; i < selectedFiles.Count; i++)
                {
                    string inputFile = selectedFiles[i];

                    try
                    {
                        string outputFile = GenerateOutputFileName(
                            inputFile,
                            outputFormat,
                            prefix,
                            findPattern,
                            replaceWith,
                            autoSuffix,
                            quality,
                            targetResolution,
                            useCustomOutput,
                            outputPath);

                        ConvertImage(inputFile, outputFile, outputFormat, quality, keepMetadata, resize, targetResolution);

                        convertedFileNames.Add(outputFile);
                        successCount++;

                        Invoke(new Action(() =>
                        {
                            progressBar.Value = i + 1;
                            lblStatus.Text = $"Processing: {Path.GetFileName(inputFile)} ({i + 1}/{selectedFiles.Count})";
                        }));
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        errorMessages.Add($"{Path.GetFileName(inputFile)}: {ex.Message}");

                        Invoke(new Action(() =>
                        {
                            progressBar.Value = i + 1;
                        }));
                    }
                }
            });

            lblStatus.Text = $"Complete! Success: {successCount}, Failed: {failCount}";

            if (errorMessages.Count > 0)
            {
                MessageBox.Show(
                    $"Conversion completed with some errors:\n\n{string.Join("\n", errorMessages.Take(5))}" +
                    (errorMessages.Count > 5 ? $"\n\n...and {errorMessages.Count - 5} more errors" : ""),
                    "Conversion Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show(
                    $"All files converted successfully!\n\nTotal: {successCount} file(s)",
                    "Conversion Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (chkGenerateTxt.Checked && convertedFileNames.Count > 0)
            {
                GenerateDatasetListFile();
            }

            btnGenerateList.Enabled = convertedFileNames.Count > 0 && chkGenerateTxt.Checked;
            btnConvert.Enabled = true;
            btnSelectFiles.Enabled = true;
            btnDeleteSelected.Enabled = true;
            btnClearAll.Enabled = true;
        }

        private void ConvertImage(string inputPath, string outputPath, string format, int quality, bool keepMetadata, bool resize, (int width, int height) targetResolution)
        {
            using (var image = new MagickImage(inputPath))
            {
                // Convert color profile to sRGB for consistent color across all devices
                try
                {
                    var currentProfile = image.GetColorProfile();
                    
                    if (currentProfile != null)
                    {
                        // Transform existing profile to sRGB
                        image.TransformColorSpace(ColorProfiles.SRGB);
                    }
                    else
                    {
                        // No profile exists, assign sRGB directly
                        image.SetProfile(ColorProfiles.SRGB);
                        image.ColorSpace = ColorSpace.sRGB;
                    }
                }
                catch
                {
                    // Fallback to basic sRGB assignment if color profile conversion fails
                    try
                    {
                        image.ColorSpace = ColorSpace.sRGB;
                    }
                    catch
                    {
                        // Silently ignore color profile errors
                    }
                }

                if (resize && targetResolution.width > 0 && targetResolution.height > 0)
                {
                    var geometry = new MagickGeometry((uint)targetResolution.width, (uint)targetResolution.height)
                    {
                        IgnoreAspectRatio = false,
                        Greater = false,
                        Less = false
                    };

                    image.FilterType = FilterType.Lanczos;
                    image.Resize(geometry);
                }

                if (!keepMetadata)
                {
                    image.Strip();
                }
                else
                {
                    // Even when keeping metadata, ensure sRGB profile is embedded
                    try
                    {
                        image.SetProfile(ColorProfiles.SRGB);
                    }
                    catch
                    {
                        // Ignore if profile setting fails
                    }
                }

                image.Quality = (uint)quality;

                switch (format.ToUpperInvariant())
                {
                    case "JPG":
                        image.Format = MagickFormat.Jpeg;
                        // Optimize JPEG: chroma subsampling
                        image.Settings.SetDefine(MagickFormat.Jpeg, "sampling-factor", "4:2:0");
                        image.Settings.SetDefine(MagickFormat.Jpeg, "optimize-coding", "true");
                        image.SetProfile(ColorProfiles.SRGB);                        break;
                    case "PNG":
                        image.Format = MagickFormat.Png;
                        // Maximum PNG compression
                        image.Settings.SetDefine(MagickFormat.Png, "compression-level", "9");
                        image.Settings.SetDefine(MagickFormat.Png, "compression-strategy", "1");
                        image.SetProfile(ColorProfiles.SRGB);                        break;
                    case "WEBP":
                        image.Format = MagickFormat.WebP;
                        // WebP optimization: lossless for quality 95+
                        image.Settings.SetDefine(MagickFormat.WebP, "method", "6");
                        image.Settings.SetDefine(MagickFormat.WebP, "lossless", quality >= 95 ? "true" : "false");
                        image.SetProfile(ColorProfiles.SRGB);                        break;
                    default:
                        image.Format = MagickFormat.Jpeg;
                        image.SetProfile(ColorProfiles.SRGB);                        break;
                }

                image.Write(outputPath);
            }
        }
        private string GenerateOutputFileName(
            string inputPath,
            string format,
            string prefix,
            string findPattern,
            string replaceWith,
            bool autoSuffix,
            int quality,
            (int width, int height) targetResolution,
            bool useCustomOutput = false,
            string customOutput = "")
        {
            string directory = useCustomOutput && !string.IsNullOrEmpty(customOutput)
                ? customOutput
                : Path.GetDirectoryName(inputPath) ?? "";
            
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(inputPath);
            string extension = format.ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(findPattern))
            {
                fileNameWithoutExt = fileNameWithoutExt.Replace(findPattern, replaceWith ?? "");
            }

            if (autoSuffix)
            {
                string resolutionLabel = GetResolutionLabel(targetResolution);
                string suffix = $"{resolutionLabel}{quality}q";
                fileNameWithoutExt = $"{fileNameWithoutExt}-{suffix}";
            }

            string baseOutputName = $"{prefix}{fileNameWithoutExt}.{extension}";
            string outputPath = Path.Combine(directory, baseOutputName);

            int counter = 1;
            while (File.Exists(outputPath))
            {
                baseOutputName = $"{prefix}{fileNameWithoutExt}_{counter}.{extension}";
                outputPath = Path.Combine(directory, baseOutputName);
                counter++;
            }

            return outputPath;
        }

        private (int width, int height) GetCustomResolution()
        {
            if (int.TryParse(txtWidth.Text, out int width) &&
                int.TryParse(txtHeight.Text, out int height))
            {
                return (width, height);
            }
            return (0, 0);
        }

        private string GetResolutionLabel((int width, int height) resolution)
        {
            if (resolution.width == 0 || resolution.height == 0)
                return "";

            bool isHorizontal = resolution.width >= resolution.height;
            int shortSide = isHorizontal ? resolution.height : resolution.width;

            string orientSuffix = isHorizontal ? "" : "-vert";

            string label = shortSide switch
            {
                2160 => $"4k{orientSuffix}-",
                1440 => $"1440p{orientSuffix}-",
                1080 => $"1080p{orientSuffix}-",
                720 => $"720p{orientSuffix}-",
                _ => $"{resolution.width}x{resolution.height}-"
            };

            return label;
        }

        private string SanitizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = string.Join("_", input.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

            return sanitized.Length > 50 ? sanitized.Substring(0, 50) : sanitized;
        }

        private void txtFindPattern_TextChanged(object sender, EventArgs e)
        {

        }
    }
}