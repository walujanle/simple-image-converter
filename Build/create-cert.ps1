# Certificate Creation Helper Script
param(
    [string]$PublisherName,
    [string]$CertPath,
    [string]$CertPassword,
    [switch]$InstallToTrustedRoot = $false
)

# Set error action preference
$ErrorActionPreference = "Stop"

try {
    # Verify we're running with admin rights
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    if (-not $isAdmin) {
        Write-Host "Error: This script requires Administrator privileges"
        exit 1
    }

    # Check if Certificate Provider is available
    $certProvider = Get-PSProvider -PSProvider Certificate -ErrorAction SilentlyContinue
    if (-not $certProvider) {
        Write-Host "Certificate provider not available. Attempting to import module..."
        try {
            Import-Module PKI -ErrorAction Stop
            Start-Sleep -Milliseconds 500
        }
        catch {
            Write-Host "Error: Could not load Certificate provider. Please ensure you are running PowerShell with administrator privileges."
            exit 1
        }
    }

    # Verify Cert drive exists
    if (-not (Test-Path "Cert:\CurrentUser\My")) {
        Write-Host "Error: Cannot access certificate store. Please run PowerShell as Administrator."
        exit 1
    }

    Write-Host "Creating self-signed code signing certificate..."
    Write-Host "Publisher: $PublisherName"
    
    # Create self-signed certificate with extended properties
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
        Write-Host "Error: Failed to create certificate"
        exit 1
    }

    $thumbprint = $cert.Thumbprint
    Write-Host "Certificate created with thumbprint: $thumbprint"

    # Export certificate to file
    Write-Host "Exporting certificate to: $CertPath"
    $pwd = ConvertTo-SecureString -String $CertPassword -Force -AsPlainText
    $exportResult = Export-PfxCertificate -Cert "Cert:\CurrentUser\My\$thumbprint" -FilePath $CertPath -Password $pwd -ErrorAction Stop

    if (-not (Test-Path $CertPath)) {
        Write-Host "Error: Certificate export failed - file not found"
        exit 1
    }

    # Optionally install to Trusted Root (makes signature visible)
    if ($InstallToTrustedRoot) {
        Write-Host "Installing certificate to Trusted Root Certification Authorities..."
        try {
            # Export certificate as CER (public key only)
            $cerPath = [System.IO.Path]::ChangeExtension($CertPath, ".cer")
            Export-Certificate -Cert "Cert:\CurrentUser\My\$thumbprint" -FilePath $cerPath -Type CERT | Out-Null
            
            # Import to Trusted Root
            Import-Certificate -FilePath $cerPath -CertStoreLocation "Cert:\LocalMachine\Root" -ErrorAction Stop | Out-Null
            
            # Clean up CER file
            Remove-Item $cerPath -Force -ErrorAction SilentlyContinue
            
            Write-Host "Certificate installed to Trusted Root (signature will be visible)"
        }
        catch {
            Write-Host "Warning: Could not install to Trusted Root. Signature may show as untrusted."
            Write-Host "Error: $($_.Exception.Message)"
        }
    }

    # Remove from Personal store
    Write-Host "Cleaning up certificate store..."
    Remove-Item "Cert:\CurrentUser\My\$thumbprint" -Force -ErrorAction SilentlyContinue

    Write-Host "Certificate exported successfully to: $CertPath"
    Write-Host ""
    Write-Host "[OK] Certificate creation completed successfully" -ForegroundColor Green
    
    if (-not $InstallToTrustedRoot) {
        Write-Host ""
        Write-Host "Note: To make digital signature visible in file properties," -ForegroundColor Yellow
        Write-Host "      the certificate needs to be trusted by Windows." -ForegroundColor Yellow
        Write-Host "      This build uses a temporary self-signed certificate." -ForegroundColor Yellow
    }
    
    exit 0
}
catch {
    Write-Host ""
    Write-Host "[FAILED] Certificate creation error" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)"
    
    # Additional diagnostic info
    Write-Host ""
    Write-Host "Diagnostic Information:"
    Write-Host "  PowerShell Version: $($PSVersionTable.PSVersion)"
    Write-Host "  Execution Policy: $(Get-ExecutionPolicy)"
    Write-Host "  Is Admin: $(([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator))"
    Write-Host ""
    
    exit 1
}
