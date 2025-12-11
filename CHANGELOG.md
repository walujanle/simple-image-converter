# Changelog

All notable changes to Simple Image Converter will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2025-12-11

### Added
- **Batch Image Conversion**
  - Support for multiple input formats (JPG, PNG, BMP, GIF, WEBP, TIFF)
  - Output to JPG, PNG, or WEBP formats
  - Drag & drop file reordering in batch list
  
- **Quality & Format Control**
  - Adjustable quality slider (1-100)
  - Format-specific optimizations:
    - JPEG: 4:2:0 chroma subsampling + optimize-coding
    - PNG: Compression level 9 with strategy 1
    - WEBP: Lossless mode for quality ?95
  
- **Image Processing**
  - Custom resolution scaling with aspect ratio preservation
  - Lanczos resampling filter for high-quality resizing
  - Automatic orientation detection (Horizontal/Vertical)
  - sRGB color profile conversion for consistent colors
  
- **Output Management**
  - Custom output folder selection
  - Save to different location than source files
  - Automatic duplicate file handling with counter
  
- **Filename Customization**
  - Add custom prefix to all files
  - Find and replace text patterns
  - Auto-suffix with resolution + quality (e.g., `-1080p-85q`)
  - Resolution labels: 4k, 1440p, 1080p, 720p (with -vert for vertical)
  
- **Dataset Tools**
  - Generate TXT file listing all converted images
  - Optional numbering (1., 2., 3., etc.)
  - Auto-generate mode (create list after conversion)
  - Manual export button for existing conversions
  
- **Security Features**
  - Magic number validation (not just file extensions)
  - ImageMagick security policies (disabled dangerous coders)
  - Resource limits: 512MB memory, 1GB disk
  - Optional metadata stripping (EXIF removal)
  - File size limit: 100MB per image
  
- **User Interface**
  - Progress bar with real-time status updates
  - File counter display
  - Multi-select and batch delete
  - Clear all with confirmation dialog
  - Keyboard shortcuts (Delete key to remove files)
  
- **Error Handling**
  - Detailed error messages for failed conversions
  - Partial success reporting
  - Invalid file warnings with filename list

### Security
- Implemented ImageMagick policy restrictions
- Blocked remote coders (HTTP, HTTPS, FTP)
- Disabled script coders (MVG, MSL)
- Added file signature verification
- Path sanitization for custom filenames

---

## Version History

- **1.0.0** - Initial public release (2025)
