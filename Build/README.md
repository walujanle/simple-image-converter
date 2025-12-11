# Simple Image Converter - Build Scripts

---

## ?? Quick Navigation

- [Quick Start](#-quick-start)
- [Build Scripts](#-build-scripts)
- [Certificate Scripts](#-certificate-scripts)
- [Utility Scripts](#-utility-scripts)
- [Build Options](#-build-options)
- [Requirements](#-requirements)

---

## ?? Quick Start

### Interactive Menu (Easiest)
```cmd
BUILD-MENU.bat
```

### Command Line
```cmd
# Development build (no signature)
build-unsigned.bat

# Production build (with digital signature)
build-signed.bat
```

---

## ?? Build Scripts

### BUILD-MENU.bat
**Purpose:** Interactive menu for all build operations

**Features:**
- User-friendly interface
- Guides through all build options
- No command-line knowledge required

**Menu Options:**
1. **Build Unsigned** - Fast development builds
2. **Build Signed** - Production builds with digital signature
3. **Clean All** - Remove all build outputs
4. **Open Output** - View built executables
5. **Documentation** - View this README
0. **Exit**

**Usage:**
```cmd
cd Build
BUILD-MENU.bat
```

**When to use:** Best for beginners or when you want a guided experience

---

### build-unsigned.bat
**Purpose:** Build executable without digital signature

**Features:**
- No administrator privileges required
- Fastest build method (~1-2 minutes)
- Single-file self-contained executable
- Optimized for development and testing

**Build Process:**
1. Locate MSBuild (Visual Studio or .NET SDK)
2. Clean previous builds
3. Restore NuGet packages with win-x64 runtime
4. Publish single-file executable
   - Configuration: Release
   - Runtime: win-x64 (64-bit Windows)
   - Self-contained: All dependencies included
   - Single file: One .exe file
5. Copy LICENSE and README files

**Output:** `Build\Output\Unsigned\simple-image-converter.exe`

**Usage:**
```cmd
cd Build
build-unsigned.bat
```

**When to use:**
- ? Quick development testing
- ? Internal use only
- ? No digital signature needed
- ? Fastest iteration

---

### build-signed.bat
**Purpose:** Wrapper script for PowerShell signed build

**Features:**
- Auto-elevation to administrator (required for signing)
- Backward compatible with older workflows
- Simple command-line interface

**How it works:**
1. Check if running as Administrator
2. If not admin: Request elevation via UAC prompt
3. Call `build-signed.ps1` PowerShell script
4. Handle errors gracefully

**Output:** `Build\Output\Signed\simple-image-converter.exe` (digitally signed)

**Usage:**
```cmd
cd Build
build-signed.bat
```

**When to use:**
- ? Prefer batch file interface
- ? Production builds
- ? Need digital signature
- ? Distribution to users

---

### build-signed.ps1
**Purpose:** Main PowerShell script for creating digitally signed builds

**Features:**
- PowerShell native code signing (no signtool.exe required)
- Certificate creation and management
- Auto-elevation support
- Command-line parameters for automation
- Comprehensive error handling
- Color-coded console output

**Parameters:**
```powershell
-PublisherName    # Your company/publisher name (default: interactive prompt)
-UseExistingCert  # Use existing certificate instead of creating new
-CertPath         # Path to existing .pfx certificate file
-Quiet            # Suppress non-essential output (for automation)
```

**Build Process:**
1. **Auto-elevation** - Request admin if not already running as admin
2. **Find MSBuild** - Locate Visual Studio or .NET SDK MSBuild
3. **Clean builds** - Remove previous output and temporary files
4. **Certificate handling:**
   - Check if certificate exists in Personal store
   - Reuse existing certificate if found (faster)
   - Create new self-signed certificate if needed
   - Install to Trusted Root for signature visibility
5. **Restore packages** - NuGet restore with win-x64 runtime
6. **Build executable** - Publish with PublisherName embedded
7. **Digital signing:**
   - Get certificate from Personal store
   - Sign using PowerShell `Set-AuthenticodeSignature`
   - Add timestamp from DigiCert server
   - Verify signature was applied
8. **Copy files** - Include LICENSE and README
9. **Verification** - Confirm signature is embedded and valid

**Certificate Details:**
- **Subject:** CN=YourCompany, O=YourCompany, C=US
- **Type:** Code Signing Certificate
- **Key Length:** 2048-bit RSA
- **Hash Algorithm:** SHA256
- **Validity:** 1 year
- **Storage:** Personal (Cert:\CurrentUser\My) + Trusted Root (Cert:\LocalMachine\Root)

**Output:** `Build\Output\Signed\simple-image-converter.exe` (digitally signed)

**Usage Examples:**
```powershell
# Interactive mode (prompts for publisher name)
.\build-signed.ps1

# Non-interactive mode
.\build-signed.ps1 -PublisherName "My Company"

# Use existing certificate
.\build-signed.ps1 -UseExistingCert -CertPath "C:\certs\mycert.pfx"

# Automation/CI mode
.\build-signed.ps1 -PublisherName "Company" -Quiet
```

**When to use:**
- ? Production builds for distribution
- ? Need command-line parameters
- ? Automation/CI/CD pipelines
- ? Professional digital signatures

**Why PowerShell native signing?**
- No Windows SDK required (signtool.exe not needed)
- Built into Windows (PowerShell 5.0+)
- Same Authenticode signature as signtool
- Simpler, more reliable code
- Works on any Windows system

---

## ?? Certificate Scripts

### create-cert.ps1
**Purpose:** Create self-signed code signing certificates

**Features:**
- Admin privilege validation
- Certificate provider availability check
- PKI module auto-import
- Trusted Root installation option
- Enhanced certificate properties

**Parameters:**
```powershell
-PublisherName         # Required: Certificate subject name
-CertPath             # Required: Output path for .pfx file
-CertPassword         # Required: Certificate password
-InstallToTrustedRoot # Optional: Install to Trusted Root (makes signature visible)
```

**Certificate Creation Process:**
1. Verify administrator privileges
2. Check Certificate provider availability
3. Import PKI module if needed
4. Create self-signed certificate with properties:
   - Subject: CN=Publisher, O=Publisher, C=US
   - Type: Code Signing Certificate
   - Key: 2048-bit RSA
   - Hash: SHA256
   - Enhanced Key Usage: Code Signing (1.3.6.1.5.5.7.3.3)
   - Friendly Name: "Code Signing Certificate - [Publisher]"
5. Export to .pfx file with password protection
6. Optionally install to Trusted Root
7. Clean up certificate from Personal store (security)

**Usage:**
```powershell
# Basic usage (called by build scripts)
.\create-cert.ps1 -PublisherName "MyCompany" -CertPath "cert.pfx" -CertPassword "pass123"

# With Trusted Root installation
.\create-cert.ps1 -PublisherName "MyCompany" -CertPath "cert.pfx" -CertPassword "pass123" -InstallToTrustedRoot
```

**When to use:**
- Usually called automatically by build scripts
- Manually create certificate for specific purposes
- Replace expired certificates

**Note:** This script is typically called by `build-signed.ps1` automatically. Manual use is rare.

---

### manage-certificates.ps1
**Purpose:** Manage installed code signing certificates

**Features:**
- List all code signing certificates
- Remove specific certificates by publisher name
- Remove all signing certificates
- Search in both Personal and Trusted Root stores
- Interactive confirmation for deletions

**Actions:**

#### List Certificates
```powershell
.\manage-certificates.ps1 -Action List
```

**Shows:**
- Subject (CN, O, C)
- Thumbprint
- Friendly Name
- Valid From/To dates
- Store location (Personal and/or Trusted Root)

**Example Output:**
```
[1] Certificate Details:
    Subject:      CN=My Company, O=My Company, C=US
    Thumbprint:   E81F39C4176004B65E501917F71D9104D06D2A56
    FriendlyName: Code Signing Certificate - My Company
    Valid From:   2025-01-15
    Valid To:     2026-01-15
```

#### Remove Specific Certificate
```powershell
.\manage-certificates.ps1 -Action Remove -PublisherName "My Company"
```

**Process:**
1. Search for certificates matching publisher name
2. Display certificates found in both stores
3. Ask for confirmation
4. Remove from Personal store
5. Remove from Trusted Root store
6. Confirm deletion

#### Remove All Certificates
```powershell
.\manage-certificates.ps1 -Action RemoveAll
```

**Process:**
1. Find all code signing certificates
2. Display complete list
3. Require "REMOVE ALL" confirmation (safety)
4. Remove all certificates from both stores

**When to use:**
- View installed certificates
- Clean up old/expired certificates
- Prepare for commercial certificate installation
- Troubleshoot certificate issues

**Important:** Removing certificates will make digital signatures unverifiable (they'll still be in the .exe but won't show as trusted)

---

## ??? Utility Scripts

### clean-all.bat
**Purpose:** Remove all build outputs and temporary files

**What it cleans:**
- `Build\Output\Unsigned\*` - Unsigned build files
- `Build\Output\Signed\*` - Signed build files  
- `simple-image-converter\bin\*` - Binary outputs
- `simple-image-converter\obj\*` - Object files and intermediates
- `Build\Temp\*` - Temporary certificates and files

**Process:**
1. Navigate to project root
2. Delete output directories
3. Delete bin and obj folders
4. Delete temporary files
5. Confirm deletion

**Usage:**
```cmd
cd Build
clean-all.bat
```

**When to use:**
- Before important builds (ensure clean state)
- Free up disk space
- Troubleshoot build issues
- Start fresh after errors

**Safe to run:** Yes, it only deletes generated files, never source code

---

### verify-environment.bat
**Purpose:** Check if build environment is properly configured

**What it checks:**
1. **.NET SDK** - Required for building
   - Runs `dotnet --version`
   - Shows installed SDK version
2. **MSBuild** - Required for compilation
   - Searches Visual Studio installations
   - Searches .NET SDK locations
3. **PowerShell** - Required for signed builds
   - Checks PowerShell version
   - Requires 5.0 or higher
4. **Administrator** - Required for signed builds
   - Checks current privileges
5. **Windows SDK** - Optional (for signtool.exe)
   - Not required (we use PowerShell signing)
   - Shows if available

**Output:**
```
Checking build environment...

.NET SDK: ? Installed (10.0.101)
MSBuild: ? Found (Visual Studio 2022)
PowerShell: ? Version 5.1
Admin: ? Running as administrator
Windows SDK: ? Not required (using PowerShell signing)

Environment: Ready for builds!
```

**Usage:**
```cmd
cd Build
verify-environment.bat
```

**When to use:**
- First time setup
- Build failures
- After installing/updating tools
- Troubleshooting environment issues

---

## ?? Build Options

### Comparison Table

| Feature | Unsigned Build | Signed Build |
|---------|----------------|--------------|
| **Admin Required** | ? No | ? Yes |
| **Build Time** | ~1-2 minutes | ~2-3 minutes |
| **Output Size** | ~25 MB | ~25 MB |
| **Digital Signature** | ? None | ? Embedded |
| **Publisher Name** | ? Generic | ? Your name |
| **Windows SmartScreen** | ?? Warning | ?? Warning* |
| **Certificate Required** | ? No | ? Self-signed |
| **Best For** | Development | Distribution |

### Build Outputs

Both build types produce:
- **Single executable file** - No DLL files needed
- **Self-contained** - Runs on any Windows 10/11 (no .NET required)
- **64-bit native** - Optimized for modern Windows
- **Compressed** - Smaller download size

**Unsigned Build Includes:**
- ? Executable
- ? LICENSE file
- ? README file

**Signed Build Includes:**
- ? Everything from unsigned
- ? Digital signature (Authenticode)
- ? Publisher name in file properties
- ? Timestamp (signature valid even after cert expires)

### File Properties Comparison

**Unsigned:**
```
Right-click .exe ? Properties:
  Details:
    File description: Simple Image Converter
    Product name: Simple Image Converter
    Company: Image Converter Developer (generic)
    File version: 1.0.0.0
  
  Digital Signatures:
    [Empty - no signature]
```

**Signed:**
```
Right-click .exe ? Properties:
  Details:
    File description: Simple Image Converter
    Product name: Simple Image Converter  
    Company: [Your Company Name] ?
    File version: 1.0.0.0
    Copyright: Copyright   2025
  
  Digital Signatures:
    Name of signer: CN=[Your Company], O=[Your Company], C=US ?
    Digest algorithm: sha256
    Timestamp: [DigiCert timestamp]
    Status: This digital signature is OK ?
```

---

## ?? Requirements

### For Unsigned Builds
- **Windows 10/11** or Windows Server 2016+
- **.NET 10 SDK** or Visual Studio 2022
  - Download: https://dotnet.microsoft.com/download

### For Signed Builds
All requirements from unsigned builds, plus:
- **Administrator privileges** (for certificate installation)
- **PowerShell 5.0+** (included in Windows 10/11)

### Optional (Not Required)
- **Windows SDK** - Not needed (we use PowerShell signing instead)
- **Visual Studio** - MSBuild can use .NET SDK instead

---

## ?? Common Workflows

### Development Workflow
```cmd
# Fast iteration during development
cd Build
build-unsigned.bat
# Test the executable
# Make changes
build-unsigned.bat
# Repeat
```

### Release Workflow
```cmd
# Clean build for release
cd Build
clean-all.bat
build-signed.bat
# Enter your company name when prompted
# Executable ready for distribution
```

### First-Time Setup
```cmd
cd Build
verify-environment.bat
# Fix any missing requirements
build-unsigned.bat
# Test basic build works
build-signed.bat
# Test signed build works
```

### Automation/CI Workflow
```powershell
# CI/CD pipeline
cd Build
.\build-signed.ps1 -PublisherName "$env:COMPANY_NAME" -Quiet
# No prompts, clean output for logs
```

---

## ?? Directory Structure

```
Build/
??? BUILD-MENU.bat              # Interactive menu
??? build-unsigned.bat          # Unsigned build script
??? build-signed.bat            # Signed build wrapper
??? build-signed.ps1            # Main signed build (PowerShell)
??? create-cert.ps1             # Certificate creation
??? manage-certificates.ps1     # Certificate management
??? clean-all.bat               # Cleanup utility
??? verify-environment.bat      # Environment checker
??? README.md                   # This file
??? BUILD-SYSTEM-GUIDE.md       # Complete technical documentation
??? Output/
    ??? Unsigned/               # Unsigned build output
    ?   ??? simple-image-converter.exe
    ??? Signed/                 # Signed build output
        ??? simple-image-converter.exe
```

---

## ?? Script Selection Guide

**Choose your script based on your goal:**

| Goal | Use This Script | Why |
|------|----------------|-----|
| Quick development test | `build-unsigned.bat` | Fastest, no admin needed |
| Production release | `build-signed.bat` | Professional signature |
| Automated builds | `build-signed.ps1` | Supports parameters |
| First time | `BUILD-MENU.bat` | Guided experience |
| Check setup | `verify-environment.bat` | Verify prerequisites |
| Clean slate | `clean-all.bat` | Remove old builds |
| Manage certs | `manage-certificates.ps1` | Certificate operations |

---

## ?? Additional Documentation

- **BUILD-SYSTEM-GUIDE.md** - Complete technical documentation including:
  - Detailed troubleshooting guide
  - Fix history and solutions
  - Technical implementation details
  - Best practices
  - Advanced topics

---

## ?? Tips

### For Developers
- Use **unsigned builds** during development (faster iteration)
- Use **signed builds** for final testing
- Run **clean-all.bat** before important builds

### For Distribution
- Always use **signed builds** for users
- Keep the same certificate for all versions (consistency)
- Consider commercial certificate for wide distribution

### For Automation
- Use **build-signed.ps1** with `-Quiet` parameter
- Pass publisher name via environment variable
- Check exit codes for success/failure

---

## ?? Troubleshooting

### "Cannot find drive Cert"

**Cause:** Certificate provider not available

**Solutions:**
1. Run PowerShell as Administrator
2. Check PowerShell version: `$PSVersionTable.PSVersion` (requires 5.0+)
3. Import PKI module: `Import-Module PKI`

**Verify:**
```powershell
Get-PSProvider -PSProvider Certificate
Test-Path "Cert:\CurrentUser\My"
```

---

### "MSB3191: Illegal characters in path"

**Cause:** Trailing backslash in path

**Solution:** Already fixed in current scripts. If error persists, update scripts from repository.

---

### "NETSDK1047: Assets file doesn't have target"

**Cause:** NuGet restore and publish runtime mismatch

**Solution:** Already fixed (RuntimeIdentifiers in .csproj + --runtime parameter)

**Manual fix if needed:**
```cmd
dotnet clean
dotnet restore --runtime win-x64
dotnet publish /p:RuntimeIdentifier=win-x64
```

---

### "Digital Signatures tab empty"

**Possible causes:**
1. Certificate not in Personal store
2. Certificate not in Trusted Root  
3. Executable not signed

**Check 1 - Certificate in Personal:**
```powershell
Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.FriendlyName -like "Code Signing*" }
```

**Check 2 - Certificate in Trusted Root:**
```powershell
Get-ChildItem Cert:\LocalMachine\Root | Where-Object { $_.FriendlyName -like "Code Signing*" }
```

**Check 3 - Signature exists:**
```powershell
Get-AuthenticodeSignature "Build\Output\Signed\simple-image-converter.exe"
```

**Solution:** If any check fails, rebuild with `build-signed.bat`

---

### "Publisher name not showing"

**Cause:** PublisherName not passed to build

**Solution:** Build scripts automatically include publisher name. If using manual build:
```cmd
dotnet publish /p:PublisherName="Your Company Name"
```

---

### "Signature untrusted on other computers"

**This is EXPECTED for self-signed certificates!**

**Why:**
- Your PC: Certificate in Trusted Root ? Shows as trusted
- Other PCs: Certificate NOT in their Trusted Root ? Shows as "Unknown Publisher"
- Signature IS still visible and valid

---

### "MSBuild not found"

**Solutions:**
- **Option 1 - Install .NET SDK:**
  1. Download: https://dotnet.microsoft.com/download
  2. Install .NET 10 SDK or later
  3. Verify: `dotnet --version`

- **Option 2 - Install Visual Studio:**
  1. Download: https://visualstudio.microsoft.com/
  2. Install Visual Studio 2022 Community (free)
  3. Select workload: ".NET desktop development"

---

## ?? Best Practices

### Development Workflow
```cmd
# Fast iteration
build-unsigned.bat
# Make changes
build-unsigned.bat
# Repeat...
```

**Tips:**
- ? Use unsigned builds during development (faster)
- ? Press F5 in Visual Studio for quick testing
- ? Run `clean-all.bat` before important builds
- ? Use signed builds only for final testing

### Production Distribution
```cmd
# Clean build for release
clean-all.bat
build-signed.bat
```

**Tips:**
- ? Always use signed builds for distribution
- ? Keep the same certificate for all versions (consistency)
- ? Include README and LICENSE files
- ? Test on clean machine before release
- ? Consider commercial certificate for wide distribution

### Automation/CI/CD
```powershell
# Non-interactive build
.\build-signed.ps1 -PublisherName "$env:COMPANY_NAME" -Quiet
```

**Tips:**
- ? Use `-Quiet` parameter to suppress prompts
- ? Pass publisher name via environment variable
- ? Check exit codes for success/failure
- ? Archive build outputs as artifacts

### Certificate Management
```powershell
# List certificates periodically
.\manage-certificates.ps1 -Action List

# Remove old/expired certificates
.\manage-certificates.ps1 -Action Remove -PublisherName "Old Company"
```

**Tips:**
- ? Keep certificate in Personal store (required for signing)
- ? Keep certificate in Trusted Root (required for visibility)
- ? Check certificate expiry dates regularly (1 year validity)
- ? Only remove certificates when replacing with new ones
- ? Use same certificate for consistency across builds

### Code Signing
**Tips:**
- ? Always add timestamp (signature valid even after cert expires)
- ? Use SHA256 hash algorithm (industry standard)
- ? Verify signature immediately after signing
- ? Test signature visibility on clean Windows installation
- ? Document certificate thumbprint for reference

---

## ?? Version History

| Version | Date | Major Changes |
|---------|------|---------------|
| **1.8** | Jan 2025 | PowerShell native signing (no signtool.exe required) |
| **1.7** | Jan 2025 | Certificate persistence for signature visibility |
| **1.6** | Jan 2025 | Digital signature & publisher name fixes |
| **1.5.1** | Jan 2025 | Start-Process elevation error fix |
| **1.5** | Jan 2025 | Build script consolidation (3?1) |
| **1.4** | Jan 2025 | Runtime identifier restore fix |
| **1.3** | Jan 2025 | Certificate creation error fix |
| **1.2** | Jan 2025 | Console output cleanup |
| **1.1** | Jan 2025 | Build path error fix |
| **1.0** | Dec 2024 | Initial build system |

### Key Improvements Summary
- ? 8 critical issues resolved
- ? 37% code reduction through consolidation
- ? PowerShell native signing (no external dependencies)
- ? Complete certificate management
- ? Professional digital signatures
- ? Comprehensive documentation

---

## ?? Support

### For build issues:
1. Run `verify-environment.bat` to check prerequisites
2. Check troubleshooting section above
3. Review build script console output
4. Check GitHub Issues: https://github.com/walujanle/simple-image-converter/issues

### Resources:
- **MSBuild:** https://docs.microsoft.com/visualstudio/msbuild/
- **.NET SDK:** https://dotnet.microsoft.com/download
- **PowerShell:** https://github.com/PowerShell/PowerShell
- **Code Signing:** https://docs.microsoft.com/windows/win32/seccrypto/cryptography-tools

---

**End of Documentation**

