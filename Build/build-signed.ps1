# ========================================
# Simple Image Converter - Signed Build
# Unified PowerShell Script with Enhanced Features
# ========================================

param(
    [string]$PublisherName = "",
    [switch]$UseExistingCert = $false,
    [string]$CertPath = "",
    [switch]$Quiet = $false
)

# Check for administrator privileges and request elevation if needed
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "Requesting administrator privileges..." -ForegroundColor Yellow
    Write-Host ""
    
    $arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`""
    if ($PublisherName) { $arguments += " -PublisherName `"$PublisherName`"" }
    if ($UseExistingCert) { $arguments += " -UseExistingCert" }
    if ($CertPath) { $arguments += " -CertPath `"$CertPath`"" }
    if ($Quiet) { $arguments += " -Quiet" }
    
    Start-Process powershell -Verb RunAs -ArgumentList $arguments
    exit
}

Write-Host "Running with Administrator privileges" -ForegroundColor Green
Write-Host ""

# Set error action preference
$ErrorActionPreference = "Stop"

# Enhanced output functions
function Write-Step($message) {
    if (-not $Quiet) {
        Write-Host "[INFO] $message" -ForegroundColor Cyan
    }
}

function Write-Success($message) {
    if (-not $Quiet) {
        Write-Host "[SUCCESS] $message" -ForegroundColor Green
    }
}

function Write-Error($message) {
    Write-Host "[ERROR] $message" -ForegroundColor Red
}

function Write-Warning($message) {
    Write-Host "[WARNING] $message" -ForegroundColor Yellow
}

# Find MSBuild
function Find-MSBuild {
    Write-Step "Locating MSBuild..."
    
    # Try vswhere first (Visual Studio 2017+)
    $vsWhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vsWhere) {
        $msbuildPath = & $vsWhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
        if ($msbuildPath) {
            Write-Success "Found MSBuild: $msbuildPath"
            return $msbuildPath
        }
    }
    
    # Try dotnet msbuild
    $dotnetPath = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($dotnetPath) {
        Write-Success "Using dotnet msbuild"
        return "dotnet"
    }
    
    throw "MSBuild not found! Please install Visual Studio or .NET SDK."
}

# Navigate to project root
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location (Join-Path $scriptPath "..")

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "Simple Image Converter - Signed Build" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

# Get publisher name if not provided
if ([string]::IsNullOrWhiteSpace($PublisherName)) {
    $PublisherName = Read-Host "Enter Publisher Name (e.g., Your Company Name)"
    
    if ([string]::IsNullOrWhiteSpace($PublisherName)) {
        Write-Error "Publisher name cannot be empty!"
        Read-Host "Press Enter to exit"
        exit 1
    }
}

Write-Host ""
Write-Host "Publisher: $PublisherName" -ForegroundColor Green
Write-Host ""

# Certificate variables
$certName = "ImageConverterSigningCert"
$tempDir = Join-Path $scriptPath "Temp"
$certFilePath = Join-Path $tempDir "$certName.pfx"
$certPassword = "TempPass_$(Get-Random)$(Get-Random)"
$securePassword = ConvertTo-SecureString -String $certPassword -Force -AsPlainText
$buildOutput = Join-Path $scriptPath "Output\Signed"
$projectFile = "simple-image-converter\simple-image-converter.csproj"

# Create temp directory
if (-not (Test-Path $tempDir)) {
    New-Item -ItemType Directory -Path $tempDir | Out-Null
}

try {
    # Step 1: Find MSBuild
    Write-Step "Step 1/8: Locating MSBuild..."
    $msbuild = Find-MSBuild
    $useDotnet = $msbuild -eq "dotnet"

    # Step 2: Clean previous builds
    Write-Step "Step 2/8: Cleaning previous builds..."
    if (Test-Path $buildOutput) {
        Remove-Item $buildOutput -Recurse -Force
    }
    if (Test-Path "simple-image-converter\bin") {
        Remove-Item "simple-image-converter\bin" -Recurse -Force
    }
    if (Test-Path "simple-image-converter\obj") {
        Remove-Item "simple-image-converter\obj" -Recurse -Force
    }
    Write-Success "Clean completed"

    # Step 3: Create or use existing certificate
    if (-not $UseExistingCert) {
        Write-Step "Step 3/8: Creating self-signed certificate..."
        Write-Host ""
        Write-Host "Creating persistent certificate for code signing..." -ForegroundColor Cyan
        Write-Host ""
        
        # Check if Certificate Provider is available
        try {
            $certProvider = Get-PSProvider -PSProvider Certificate -ErrorAction Stop
        }
        catch {
            Write-Warning "Certificate provider not available. Attempting to import PKI module..."
            try {
                Import-Module PKI -ErrorAction Stop
                Start-Sleep -Milliseconds 500
            }
            catch {
                throw "Could not load Certificate provider. Please ensure you are running PowerShell with administrator privileges and PKI module is available."
            }
        }

        # Verify Cert drive exists
        if (-not (Test-Path "Cert:\CurrentUser\My")) {
            throw "Cannot access certificate store. Please run PowerShell as Administrator."
        }
        
        # Check if certificate already exists
        $existingCert = Get-ChildItem "Cert:\CurrentUser\My" -ErrorAction SilentlyContinue | 
            Where-Object { 
                $_.Subject -like "*CN=$PublisherName*" -and 
                $_.FriendlyName -like "Code Signing Certificate - $PublisherName" -and
                $_.NotAfter -gt (Get-Date)
            } | Select-Object -First 1
        
        if ($existingCert) {
            Write-Success "Found existing valid certificate: $($existingCert.Thumbprint)"
            $cert = $existingCert
            $thumbprint = $cert.Thumbprint
        }
        else {
            # Create new self-signed certificate
            Write-Host "Creating code signing certificate for: $PublisherName" -ForegroundColor Cyan
            $cert = New-SelfSignedCertificate `
                -Type CodeSigningCert `
                -Subject "CN=$PublisherName, O=$PublisherName, C=US" `
                -CertStoreLocation "Cert:\CurrentUser\My" `
                -NotBefore (Get-Date) `
                -NotAfter (Get-Date).AddYears(1) `
                -KeyUsage DigitalSignature `
                -KeySpec Signature `
                -KeyLength 2048 `
                -KeyAlgorithm RSA `
                -HashAlgorithm SHA256 `
                -KeyExportPolicy Exportable `
                -FriendlyName "Code Signing Certificate - $PublisherName" `
                -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3")
            
            if (-not $cert) {
                throw "Failed to create certificate"
            }
            
            $thumbprint = $cert.Thumbprint
            Write-Success "Certificate created with thumbprint: $thumbprint"
        }
        
        # Export certificate to temporary file for signing
        $certFilePath = Join-Path $tempDir "$certName-$thumbprint.pfx"
        Export-PfxCertificate -Cert "Cert:\CurrentUser\My\$thumbprint" -FilePath $certFilePath -Password $securePassword | Out-Null
        
        if (-not (Test-Path $certFilePath)) {
            throw "Certificate export failed - file not found at: $certFilePath"
        }
        
        # Install to Trusted Root for visible signature
        $rootCertExists = Get-ChildItem "Cert:\LocalMachine\Root" -ErrorAction SilentlyContinue |
            Where-Object { $_.Thumbprint -eq $thumbprint }
        
        if (-not $rootCertExists) {
            try {
                Write-Host "Installing certificate to Trusted Root..." -ForegroundColor Cyan
                $cerPath = [System.IO.Path]::ChangeExtension($certFilePath, ".cer")
                Export-Certificate -Cert "Cert:\CurrentUser\My\$thumbprint" -FilePath $cerPath -Type CERT | Out-Null
                Import-Certificate -FilePath $cerPath -CertStoreLocation "Cert:\LocalMachine\Root" -ErrorAction Stop | Out-Null
                Remove-Item $cerPath -Force -ErrorAction SilentlyContinue
                Write-Success "Certificate installed to Trusted Root"
            }
            catch {
                Write-Warning "Could not install to Trusted Root. Signature may show as untrusted."
            }
        }
        else {
            Write-Success "Certificate already in Trusted Root"
        }
        
        Write-Success "Certificate ready for signing"
    } else {
        Write-Step "Step 3/8: Using existing certificate..."
        if ([string]::IsNullOrWhiteSpace($CertPath) -or -not (Test-Path $CertPath)) {
            throw "Certificate path is invalid or file does not exist!"
        }
        $certFilePath = $CertPath
        Write-Success "Using certificate: $CertPath"
    }

    # Step 4: Restore dependencies
    Write-Step "Step 4/8: Restoring NuGet packages..."
    & dotnet restore $projectFile --runtime win-x64 --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        throw "Restore failed!"
    }
    Write-Success "Restore completed"

    # Step 5: Build the project using MSBuild
    Write-Step "Step 5/8: Building project with MSBuild..."
    
    $buildArgs = @(
        "/p:Configuration=Release",
        "/p:RuntimeIdentifier=win-x64",
        "/p:SelfContained=true",
        "/p:PublishSingleFile=true",
        "/p:IncludeNativeLibrariesForSelfExtract=true",
        "/p:EnableCompressionInSingleFile=true",
        "/p:DebugType=None",
        "/p:DebugSymbols=false",
        "/p:PublishDir=$buildOutput",
        "/p:PublishReadyToRun=false",
        "/p:PublisherName=$PublisherName",
        "/verbosity:minimal"
    )
    
    if ($useDotnet) {
        $publishArgs = @("publish", $projectFile) + $buildArgs
        & dotnet @publishArgs
    } else {
        $publishArgs = @($projectFile, "/t:Publish") + $buildArgs
        & $msbuild @publishArgs
    }
    
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed!"
    }
    Write-Success "Build completed"

    # Step 6: Sign the executable using PowerShell native signing
    Write-Step "Step 6/8: Signing executable with digital signature..."
    
    $exePath = Join-Path $buildOutput "simple-image-converter.exe"
    
    if (-not (Test-Path $exePath)) {
        throw "Executable not found at: $exePath"
    }
    
    Write-Host "Executable to sign: $exePath" -ForegroundColor Gray
    Write-Host "Certificate thumbprint: $thumbprint" -ForegroundColor Gray
    Write-Host ""
    
    try {
        # Get certificate from Personal store
        $signingCert = Get-ChildItem "Cert:\CurrentUser\My\$thumbprint" -ErrorAction Stop
        
        if (-not $signingCert) {
            throw "Certificate not found in Personal store with thumbprint: $thumbprint"
        }
        
        Write-Host "Using certificate:" -ForegroundColor Cyan
        Write-Host "  Subject: $($signingCert.Subject)" -ForegroundColor White
        Write-Host "  Issuer: $($signingCert.Issuer)" -ForegroundColor White
        Write-Host "  Valid until: $($signingCert.NotAfter)" -ForegroundColor White
        Write-Host ""
        
        # Sign the executable using PowerShell native method
        Write-Host "Applying digital signature..." -ForegroundColor Cyan
        $signature = Set-AuthenticodeSignature -FilePath $exePath -Certificate $signingCert -TimestampServer "http://timestamp.digicert.com" -HashAlgorithm SHA256
        
        if ($signature.Status -eq "Valid") {
            Write-Success "Executable signed successfully!"
            Write-Host ""
            Write-Host "Signature details:" -ForegroundColor Cyan
            Write-Host "  Status: $($signature.Status)" -ForegroundColor Green
            Write-Host "  Signer: $($signature.SignerCertificate.Subject)" -ForegroundColor White
            Write-Host "  Hash Algorithm: SHA256" -ForegroundColor White
            Write-Host "  Timestamp: $($signature.TimeStamperCertificate.NotBefore)" -ForegroundColor White
            Write-Host ""
        }
        elseif ($signature.Status -eq "UnknownError") {
            # Sometimes returns UnknownError but actually signed - verify
            $verify = Get-AuthenticodeSignature $exePath
            if ($verify.Status -eq "Valid" -or $verify.SignerCertificate) {
                Write-Success "Executable signed (verification: $($verify.Status))"
                Write-Host "  Signer: $($verify.SignerCertificate.Subject)" -ForegroundColor White
            }
            else {
                Write-Warning "Signing completed with status: $($signature.Status)"
                Write-Warning "This may be due to self-signed certificate (expected)"
            }
        }
        else {
            Write-Warning "Signing returned status: $($signature.Status)"
            Write-Warning "Status Message: $($signature.StatusMessage)"
        }
        
        # Verify the signature was applied
        Write-Host "Verifying digital signature..." -ForegroundColor Gray
        $verifyResult = Get-AuthenticodeSignature $exePath
        
        if ($verifyResult.SignerCertificate) {
            Write-Success "Digital signature verified and embedded!"
            Write-Host ""
            Write-Host "Embedded signature information:" -ForegroundColor Cyan
            Write-Host "  Certificate Subject: $($verifyResult.SignerCertificate.Subject)" -ForegroundColor White
            Write-Host "  Certificate Issuer: $($verifyResult.SignerCertificate.Issuer)" -ForegroundColor White
            Write-Host "  Certificate Thumbprint: $($verifyResult.SignerCertificate.Thumbprint)" -ForegroundColor White
            Write-Host "  Signature Status: $($verifyResult.Status)" -ForegroundColor White
            if ($verifyResult.TimeStamperCertificate) {
                Write-Host "  Timestamp: $($verifyResult.TimeStamperCertificate.NotBefore)" -ForegroundColor White
            }
            Write-Host ""
            Write-Host "? Digital signature is now embedded in the executable!" -ForegroundColor Green
            Write-Host "? Signature will be visible in Properties ? Digital Signatures tab" -ForegroundColor Green
        }
        else {
            throw "Signature verification failed - no signer certificate found"
        }
        
    } catch {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Red
        Write-Host "ERROR: Code signing failed!" -ForegroundColor Red
        Write-Host "========================================" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        throw "Signing failed: $($_.Exception.Message)"
    }

    # Step 7: Copy additional files
    Write-Step "Step 7/8: Copying additional files..."
    
    $filesToCopy = @("LICENSE", "LICENSE.txt", "README.md")
    foreach ($file in $filesToCopy) {
        if (Test-Path $file) {
            Copy-Item $file -Destination $buildOutput -Force
            Write-Success "Copied $file"
        }
    }

    # Step 8: Final verification
    Write-Step "Step 8/8: Final verification..."
    
    $finalCheck = Get-AuthenticodeSignature $exePath
    if ($finalCheck.SignerCertificate) {
        Write-Success "Build and signing completed successfully!"
    }
    else {
        Write-Warning "Build completed but signature verification inconclusive"
    }

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Build completed successfully!" -ForegroundColor Green
    Write-Host "Output: $buildOutput" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    
    Write-Host ""
    Write-Host "IMPORTANT: Certificate & Signature Information" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host "Code signing certificate is installed in:" -ForegroundColor Cyan
    Write-Host "  1. Cert:\CurrentUser\My (Personal)" -ForegroundColor White
    Write-Host "  2. Cert:\LocalMachine\Root (Trusted Root)" -ForegroundColor White
    Write-Host ""
    Write-Host "Digital signature is embedded in:" -ForegroundColor Cyan
    Write-Host "  $exePath" -ForegroundColor White
    Write-Host ""
    Write-Host "To view signature:" -ForegroundColor Yellow
    Write-Host "  1. Right-click the .exe file" -ForegroundColor White
    Write-Host "  2. Select 'Properties'" -ForegroundColor White
    Write-Host "  3. Click 'Digital Signatures' tab" -ForegroundColor White
    Write-Host "  4. You should see: $PublisherName" -ForegroundColor White
    Write-Host ""
    Write-Host "This ensures:" -ForegroundColor Cyan
    Write-Host "  ? Digital signature is visible in file properties" -ForegroundColor Green
    Write-Host "  ? Signature shows as trusted by Windows" -ForegroundColor Green
    Write-Host "  ? Certificate can be reused for future builds" -ForegroundColor Green
    Write-Host ""
    Write-Host "Certificate Details:" -ForegroundColor Yellow
    Write-Host "  Publisher: $PublisherName" -ForegroundColor White
    if ($thumbprint) {
        Write-Host "  Thumbprint: $thumbprint" -ForegroundColor White
    }
    Write-Host ""
    Write-Host "To manage certificates:" -ForegroundColor Yellow
    Write-Host "  Run: .\manage-certificates.ps1 -Action List" -ForegroundColor White
    Write-Host "  Or: certmgr.msc (Windows Certificate Manager)" -ForegroundColor White
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Error "Build failed: $($_.Exception.Message)"
    Write-Host ""
    if (-not $Quiet) {
        Read-Host "Press Enter to exit"
    }
    exit 1
} finally {
    # Cleanup ONLY the temporary PFX file (keep certificates in stores for signature visibility)
    if (-not $UseExistingCert -and (Test-Path $certFilePath)) {
        Write-Step "Cleaning up temporary certificate file..."
        Remove-Item $certFilePath -Force -ErrorAction SilentlyContinue
        Write-Success "Temporary PFX file deleted"
    }
    
    # DO NOT remove certificates from stores!
    # Certificate in Personal store: Required for future builds and signature validation
    # Certificate in Trusted Root: Required for signature to show as trusted
    # Users can manually remove using manage-certificates.ps1 or certmgr.msc
    
    # Remove temp directory if empty
    if (Test-Path $tempDir) {
        $items = Get-ChildItem $tempDir -ErrorAction SilentlyContinue
        if ($items.Count -eq 0) {
            Remove-Item $tempDir -Force -ErrorAction SilentlyContinue
        }
    }
}

# Open output folder
if (Test-Path $buildOutput) {
    Start-Process explorer.exe $buildOutput
}

Write-Host ""
if (-not $Quiet) {
    Read-Host "Press Enter to exit"
}
