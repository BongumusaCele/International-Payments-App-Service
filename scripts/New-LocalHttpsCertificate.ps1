param(
    [string]$DnsName = "localhost",
    [string]$CertificateDirectory = "$env:APPDATA\InternationalPaymentsAPI\certs",
    [string]$ProjectPath = "$PSScriptRoot\..\InternationalPaymentsAPI\InternationalPaymentsAPI.csproj"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $CertificateDirectory)) {
    New-Item -ItemType Directory -Path $CertificateDirectory | Out-Null
}

$certificateName = "international-payments-api-dev"
$pfxPath = Join-Path $CertificateDirectory "$certificateName.pfx"
$cerPath = Join-Path $CertificateDirectory "$certificateName.cer"
$passwordBytes = New-Object byte[] 24
$randomNumberGenerator = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$randomNumberGenerator.GetBytes($passwordBytes)
$randomNumberGenerator.Dispose()
$plainPassword = [Convert]::ToBase64String($passwordBytes)
$securePassword = ConvertTo-SecureString $plainPassword -AsPlainText -Force

$existingCertificate = Get-ChildItem Cert:\CurrentUser\My |
    Where-Object { $_.Subject -eq "CN=$DnsName" -and $_.FriendlyName -eq $certificateName } |
    Select-Object -First 1

if ($existingCertificate) {
    Remove-Item -LiteralPath "Cert:\CurrentUser\My\$($existingCertificate.Thumbprint)" -Force
}

$certificate = New-SelfSignedCertificate `
    -DnsName $DnsName `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -FriendlyName $certificateName `
    -KeyAlgorithm RSA `
    -KeyLength 2048 `
    -KeyExportPolicy Exportable `
    -NotAfter (Get-Date).AddYears(1)

Export-PfxCertificate `
    -Cert $certificate `
    -FilePath $pfxPath `
    -Password $securePassword | Out-Null

Export-Certificate `
    -Cert $certificate `
    -FilePath $cerPath | Out-Null

dotnet user-secrets init --project $ProjectPath | Out-Null
dotnet user-secrets set "Kestrel:Certificates:Default:Path" $pfxPath --project $ProjectPath | Out-Null
dotnet user-secrets set "Kestrel:Certificates:Default:Password" $plainPassword --project $ProjectPath | Out-Null

Write-Host "Generated HTTPS certificate and private key bundle:"
Write-Host "  PFX: $pfxPath"
Write-Host "  CER: $cerPath"
Write-Host ""
Write-Host "Kestrel certificate settings were stored in .NET user-secrets for:"
Write-Host "  $ProjectPath"
Write-Host ""
Write-Host "Run the API with the HTTPS profile:"
Write-Host "  dotnet run --project `"$ProjectPath`" --launch-profile https"
