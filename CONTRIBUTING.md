# Contributing to Simple Image Converter

First off, thank you for considering contributing to Simple Image Converter! ??

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates. When you create a bug report, include:

- **Clear title and description**
- **Steps to reproduce** the problem
- **Expected behavior** vs actual behavior
- **Screenshots** (if applicable)
- **Environment details:**
  - OS version (Windows 10/11)
  - .NET version
  - Application version

**Bug Report Template:**
```markdown
**Describe the bug**
A clear description of what the bug is.

**To Reproduce**
1. Go to '...'
2. Click on '...'
3. See error

**Expected behavior**
What you expected to happen.

**Screenshots**
If applicable, add screenshots.

**Environment:**
- OS: [e.g., Windows 11 22H2]
- .NET Version: [e.g., .NET 10.0]
- App Version: [e.g., 1.0.0]
```

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, include:

- **Clear title and description**
- **Use case** - Why is this enhancement useful?
- **Proposed solution** - How should it work?
- **Alternatives** - Other solutions you've considered

### Pull Requests

1. **Fork** the repository
2. **Clone** your fork:
   ```bash
   git clone https://github.com/YOUR-USERNAME/simple-image-converter.git
   cd simple-image-converter
   ```

3. **Create a branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

4. **Make your changes**
   - Follow the code style (see below)
   - Add comments for complex logic
   - Update documentation if needed

5. **Test your changes**
   - Build the project: `dotnet build`
   - Test manually with various image formats
   - Verify security features still work

6. **Commit your changes**:
   ```bash
   git add .
   git commit -m "Add feature: your feature description"
   ```

7. **Push to your fork**:
   ```bash
   git push origin feature/your-feature-name
   ```

8. **Open a Pull Request**
   - Go to the original repository
   - Click "New Pull Request"
   - Select your branch
   - Fill in the PR template

---

## Code Style Guidelines

### C# Conventions
- **Naming:**
  - PascalCase for classes, methods, properties
  - camelCase for local variables, parameters
  - _camelCase for private fields
  
- **Formatting:**
  - 4 spaces for indentation (no tabs)
  - Opening braces on new line
  - One statement per line
  
- **Comments:**
  - Use `//` for single-line comments
  - Use `///` for XML documentation on public methods
  - Explain "why", not "what"

**Example:**
```csharp
public class ImageProcessor
{
    private string _outputPath;
    
    /// <summary>
    /// Converts image to specified format with quality control
    /// </summary>
    public void ConvertImage(string inputPath, int quality)
    {
        // Validate quality range to prevent errors
        if (quality < 1 || quality > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(quality));
        }
        
        // Process image...
    }
}
```

### Windows Forms Design
- Keep UI logic separate from business logic
- Use async/await for long-running operations
- Update UI on main thread using `Invoke()`
- Disable controls during processing to prevent race conditions

---

## Development Setup

### Prerequisites
- Visual Studio 2022 (or later) with .NET 10 SDK
- Git for version control

### Building
```bash
# Clone repository
git clone https://github.com/walujanle/simple-image-converter.git
cd simple-image-converter

# Restore NuGet packages
dotnet restore

# Build
dotnet build -c Debug

# Run
dotnet run --project simple-image-converter
```

### Project Structure
```
simple-image-converter/
??? simple-image-converter/
?   ??? Form1.cs              # Main form logic
?   ??? Form1.Designer.cs     # UI components (auto-generated)
?   ??? Program.cs            # Entry point + security init
?   ??? simple-image-converter.csproj
??? README.md
??? LICENSE
??? CHANGELOG.md
??? CONTRIBUTING.md
```

---

## Testing Guidelines

### Manual Testing Checklist
- [ ] Load various image formats (JPG, PNG, WEBP, BMP, GIF, TIFF)
- [ ] Test with large files (close to 100MB limit)
- [ ] Verify invalid file rejection
- [ ] Test batch conversion with 10+ files
- [ ] Check custom output folder functionality
- [ ] Verify TXT list generation (with/without numbering)
- [ ] Test filename patterns (prefix, find/replace, auto-suffix)
- [ ] Verify drag & drop reordering
- [ ] Test resize with various resolutions
- [ ] Check quality slider synchronization with textbox
- [ ] Test "Keep Metadata" option
- [ ] Verify duplicate file handling
- [ ] Test progress bar updates
- [ ] Check error handling for corrupted files

### Security Testing
- [ ] Attempt to load non-image files
- [ ] Test with files >100MB
- [ ] Verify ImageMagick policy enforcement
- [ ] Test path traversal with custom filenames

---

## Feature Request Template

```markdown
**Is your feature request related to a problem?**
A clear description of the problem. Ex. I'm frustrated when [...]

**Describe the solution you'd like**
A clear description of what you want to happen.

**Describe alternatives you've considered**
Other solutions or features you've considered.

**Additional context**
Add any other context or screenshots.
```

---

## Code Review Process

All pull requests will be reviewed for:

1. **Functionality** - Does it work as intended?
2. **Code Quality** - Is it readable and maintainable?
3. **Security** - Does it introduce vulnerabilities?
4. **Performance** - Does it impact performance negatively?
5. **Documentation** - Is it properly documented?

---

## Questions?

Feel free to open an issue with the `question` label if you need help or clarification.

---

## Code of Conduct

Be respectful and constructive in all interactions. We're all here to learn and improve the project together.

Thank you for contributing! ??
