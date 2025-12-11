# Certificate Management Helper
# Manage code signing certificates for Simple Image Converter

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("List", "Remove", "RemoveAll")]
    [string]$Action = "List",
    
    [string]$PublisherName = ""
)

# Check for administrator privileges
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "This script requires Administrator privileges." -ForegroundColor Red
    Write-Host "Please run as Administrator." -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Certificate Management Tool" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

function Get-SigningCertificates {
    # Get from both Personal and Trusted Root
    $personalCerts = Get-ChildItem -Path "Cert:\CurrentUser\My" -ErrorAction SilentlyContinue | 
        Where-Object { 
            $_.FriendlyName -like "Code Signing Certificate*" -or 
            $_.EnhancedKeyUsageList -match "Code Signing"
        }
    
    $rootCerts = Get-ChildItem -Path "Cert:\LocalMachine\Root" -ErrorAction SilentlyContinue | 
        Where-Object { 
            $_.FriendlyName -like "Code Signing Certificate*" -or 
            $_.EnhancedKeyUsageList -match "Code Signing"
        }
    
    # Combine and remove duplicates
    $allCerts = @($personalCerts) + @($rootCerts) | 
        Sort-Object Thumbprint -Unique
    
    return $allCerts
}

function List-Certificates {
    Write-Host "Listing code signing certificates in Trusted Root..." -ForegroundColor Cyan
    Write-Host ""
    
    $certs = Get-SigningCertificates
    
    if ($certs.Count -eq 0) {
        Write-Host "No code signing certificates found in Trusted Root." -ForegroundColor Yellow
        return
    }
    
    Write-Host "Found $($certs.Count) certificate(s):" -ForegroundColor Green
    Write-Host ""
    
    $index = 1
    foreach ($cert in $certs) {
        Write-Host "[$index] Certificate Details:" -ForegroundColor White
        Write-Host "    Subject:      $($cert.Subject)" -ForegroundColor Gray
        Write-Host "    Issuer:       $($cert.Issuer)" -ForegroundColor Gray
        Write-Host "    Thumbprint:   $($cert.Thumbprint)" -ForegroundColor Gray
        Write-Host "    FriendlyName: $($cert.FriendlyName)" -ForegroundColor Gray
        Write-Host "    Valid From:   $($cert.NotBefore)" -ForegroundColor Gray
        Write-Host "    Valid To:     $($cert.NotAfter)" -ForegroundColor Gray
        Write-Host ""
        $index++
    }
}

function Remove-Certificate {
    param([string]$Publisher)
    
    if ([string]::IsNullOrWhiteSpace($Publisher)) {
        $Publisher = Read-Host "Enter Publisher Name to remove"
    }
    
    Write-Host "Searching for certificates matching: $Publisher" -ForegroundColor Cyan
    
    # Search in both stores
    $personalCerts = Get-ChildItem -Path "Cert:\CurrentUser\My" -ErrorAction SilentlyContinue | 
        Where-Object { 
            $_.Subject -like "*$Publisher*" -and
            $_.FriendlyName -like "Code Signing Certificate*"
        }
    
    $rootCerts = Get-ChildItem -Path "Cert:\LocalMachine\Root" -ErrorAction SilentlyContinue | 
        Where-Object { 
            $_.Subject -like "*$Publisher*" -and
            $_.FriendlyName -like "Code Signing Certificate*"
        }
    
    $totalCount = $personalCerts.Count + $rootCerts.Count
    
    if ($totalCount -eq 0) {
        Write-Host "No certificates found matching: $Publisher" -ForegroundColor Yellow
        return
    }
    
    Write-Host "Found $totalCount certificate(s) to remove:" -ForegroundColor Yellow
    foreach ($cert in $personalCerts) {
        Write-Host "  - Personal Store: $($cert.Subject)" -ForegroundColor Gray
    }
    foreach ($cert in $rootCerts) {
        Write-Host "  - Trusted Root: $($cert.Subject)" -ForegroundColor Gray
    }
    Write-Host ""
    
    $confirm = Read-Host "Are you sure you want to remove these certificates? (yes/no)"
    
    if ($confirm -eq "yes") {
        foreach ($cert in $personalCerts) {
            try {
                Remove-Item "Cert:\CurrentUser\My\$($cert.Thumbprint)" -Force
                Write-Host "Removed from Personal: $($cert.Subject)" -ForegroundColor Green
            }
            catch {
                Write-Host "Failed to remove from Personal: $($cert.Subject)" -ForegroundColor Red
                Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
        foreach ($cert in $rootCerts) {
            try {
                Remove-Item "Cert:\LocalMachine\Root\$($cert.Thumbprint)" -Force
                Write-Host "Removed from Trusted Root: $($cert.Subject)" -ForegroundColor Green
            }
            catch {
                Write-Host "Failed to remove from Trusted Root: $($cert.Subject)" -ForegroundColor Red
                Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
        Write-Host ""
        Write-Host "Certificate removal completed." -ForegroundColor Green
    }
    else {
        Write-Host "Removal cancelled." -ForegroundColor Yellow
    }
}

function Remove-AllCertificates {
    Write-Host "WARNING: This will remove ALL code signing certificates!" -ForegroundColor Red
    Write-Host ""
    
    $certs = Get-SigningCertificates
    
    if ($certs.Count -eq 0) {
        Write-Host "No code signing certificates found." -ForegroundColor Yellow
        return
    }
    
    Write-Host "Found $($certs.Count) certificate(s):" -ForegroundColor Yellow
    foreach ($cert in $certs) {
        Write-Host "  - $($cert.Subject)" -ForegroundColor Gray
    }
    Write-Host ""
    
    $confirm = Read-Host "Type 'REMOVE ALL' to confirm removal of all certificates"
    
    if ($confirm -eq "REMOVE ALL") {
        foreach ($cert in $certs) {
            try {
                Remove-Item "Cert:\LocalMachine\Root\$($cert.Thumbprint)" -Force
                Write-Host "Removed: $($cert.Subject)" -ForegroundColor Green
            }
            catch {
                Write-Host "Failed to remove: $($cert.Subject)" -ForegroundColor Red
            }
        }
        Write-Host ""
        Write-Host "All certificates removed." -ForegroundColor Green
    }
    else {
        Write-Host "Removal cancelled." -ForegroundColor Yellow
    }
}

# Execute action
switch ($Action) {
    "List" {
        List-Certificates
    }
    "Remove" {
        Remove-Certificate -Publisher $PublisherName
    }
    "RemoveAll" {
        Remove-AllCertificates
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Usage Examples:" -ForegroundColor Cyan
Write-Host "  .\manage-certificates.ps1 -Action List" -ForegroundColor Gray
Write-Host "  .\manage-certificates.ps1 -Action Remove -PublisherName 'My Company'" -ForegroundColor Gray
Write-Host "  .\manage-certificates.ps1 -Action RemoveAll" -ForegroundColor Gray
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if (-not $Quiet) {
    Read-Host "Press Enter to exit"
}
