# Simple Image Converter

**Version:** 1.0.0  
**Platform:** Windows (.NET 10)

A lightweight desktop application for batch image conversion with advanced customization options. Built with C# and ImageMagick.NET

---

## ?? Features

### Core Conversion
- **Supported Input Formats:** JPG, JPEG, PNG, BMP, GIF, WEBP, TIFF
- **Output Formats:** JPG, PNG, WEBP
- **Quality Control:** Adjustable quality (1-100) with real-time preview
- **Batch Processing:** Convert multiple images simultaneously
- **Color Profile Management:** Automatic sRGB conversion for consistent colors

### Image Processing
- **Smart Resizing:** Custom resolution with aspect ratio preservation
- **Orientation Detection:** Auto-detect horizontal/vertical layout
- **Advanced Filtering:** Lanczos resampling for high-quality results
- **Format Optimization:**
  - JPG: Chroma subsampling (4:2:0) + optimize-coding
  - PNG: Maximum compression (level 9)
  - WEBP: Lossless mode for quality ?95

### File Management
- **Drag & Drop Reordering:** Change batch processing order
- **Custom Output Location:** Save converted files to any folder
- **Filename Customization:**
  - Add prefix to all files
  - Find & replace patterns
  - Auto-suffix with resolution + quality (e.g., `-1080p-85q`)
- **Duplicate Handling:** Automatic file counter for naming conflicts

### Dataset Tools
- **TXT List Generator:** Create text files listing all converted images
- **Numbering Options:** Add sequential numbers to each entry
- **Auto-generate Mode:** Create list immediately after conversion
- **Manual Export:** Generate list anytime from converted files

### Security
- **File Validation:** Magic number verification (not just file extensions)
- **Size Limits:** Max 100MB per file
- **ImageMagick Policies:** Disabled dangerous coders (MVG, MSL, HTTP, FTP)
- **Resource Limits:** 512MB memory, 1GB disk usage cap
- **Metadata Control:** Optional EXIF/metadata stripping

---

## ?? System Requirements

- **OS:** Windows 10/11 (64-bit)
- **.NET Runtime:** .NET 10.0 or higher
- **RAM:** 512MB minimum (for ImageMagick processing)
- **Disk Space:** 100MB + space for converted images

---

## ?? Installation

### Option 1: Download Release (Recommended)
1. Go to [Releases](https://github.com/walujanle/simple-image-converter/releases)
2. Download `SimpleImageConverter-v1.0.0.zip`
3. Extract and run `simple-image-converter.exe`

### Option 2: Build from Source
```bash
# Clone repository
git clone https://github.com/walujanle/simple-image-converter.git
cd simple-image-converter

# Build project
dotnet build -c Release

# Run application
dotnet run --project simple-image-converter
```

---

## ?? Usage Guide

### Basic Workflow
1. **Select Files:** Click `?? Select Files` and choose images
2. **Configure Settings:**
   - Choose output format (JPG/PNG/WEBP)
   - Adjust quality slider (default: 85)
   - *(Optional)* Enable resize and set dimensions
3. **Output Options:**
   - *(Optional)* Check "Use Custom Output Folder" to save elsewhere
   - *(Optional)* Enable "Generate TXT List" for dataset tracking
4. **Convert:** Click `?? Start Conversion`

### Advanced Features

#### Custom Resolution
```
? Enable Resize
  Width: 1920
  Height: 1080
  Orientation: Auto
  ? Aspect Ratio: 16:9 (Horizontal)
```

#### Filename Patterns
```
Prefix: converted_
Find Pattern: IMG_
Replace With: photo_
Auto Suffix: ? (adds -1080p-85q)

Result: converted_photo_0123-1080p-85q.jpg
```

#### Dataset List Output
**Without Numbering:**
```
image1-1080p-85q.jpg
image2-1080p-85q.jpg
image3-1080p-85q.jpg
```

**With Numbering:**
```
1. image1-1080p-85q.jpg
2. image2-1080p-85q.jpg
3. image3-1080p-85q.jpg
```

---

## ??? Security Features

This application implements multiple security layers:

1. **File Type Validation:** Verifies magic numbers (file signatures) instead of relying on extensions
2. **ImageMagick Hardening:** 
   - Disabled remote coders (HTTP, HTTPS, FTP)
   - Blocked script coders (MVG, MSL)
   - Memory/disk resource limits enforced
3. **Path Sanitization:** Removes invalid characters from custom filenames
4. **Safe Metadata Handling:** Controlled EXIF data processing

---

## ?? Technology Stack

- **Framework:** .NET 10.0 (Windows Forms)
- **Image Processing:** Magick.NET-Q8-AnyCPU v14.9.1
- **Language:** C# 14.0
- **UI:** Native Windows Forms components
- **Architecture:** Single-threaded UI + async Task-based batch processing

---

## ?? Performance

- **Single Image:** ~50-200ms (depending on size and operations)
- **Batch 100 Images (1080p ? 720p):** ~10-15 seconds
- **Memory Usage:** 50-150MB during conversion
- **CPU Usage:** Moderate (1 core utilized per conversion)

---

## ?? Known Limitations

- Windows-only (uses Windows Forms)
- Single-threaded conversion (processes images sequentially)
- Max file size: 100MB per image
- No undo/redo functionality
- No image preview before conversion

---

## ?? Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## ?? Changelog

### v1.0.0 (2025)
- ? Initial release
- ? Batch image conversion (JPG, PNG, WEBP)
- ? Custom resolution scaling with Lanczos filter
- ? sRGB color profile normalization
- ? Custom output folder support
- ? Dataset TXT list generator
- ? Filename pattern customization (prefix, find/replace, auto-suffix)
- ? Drag & drop file reordering
- ? Security hardening (magic number validation, ImageMagick policies)
- ? Format-specific optimizations (JPEG chroma subsampling, PNG compression)

---

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## ?? Acknowledgments

- [Magick.NET](https://github.com/dlemstra/Magick.NET) - Powerful .NET wrapper for ImageMagick
- [ImageMagick](https://imagemagick.org/) - Core image processing library

---

## ?? Contact

**Author:** Leonard Walujan  
**GitHub:** [@walujanle](https://github.com/walujanle)  
**Project Link:** [https://github.com/walujanle/simple-image-converter](https://github.com/walujanle/simple-image-converter)

---

## ? Support

If you find this project useful, please consider giving it a star on GitHub!
