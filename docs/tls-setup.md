# TLS setup

The API is configured to serve traffic over HTTPS and to redirect HTTP requests to HTTPS.

## Local development certificate

Generate a local self-signed certificate and private key bundle for Kestrel:

```powershell
.\scripts\New-LocalHttpsCertificate.ps1
```

The script:

- creates a one-year RSA 2048-bit certificate for `localhost`;
- exports a `.pfx` file containing the certificate and private key;
- exports a `.cer` public certificate file;
- stores `Kestrel:Certificates:Default:Path` and `Kestrel:Certificates:Default:Password` in .NET user-secrets.

Run the API with the HTTPS launch profile:

```powershell
dotnet run --project .\InternationalPaymentsAPI\InternationalPaymentsAPI.csproj --launch-profile https
```

The HTTPS endpoint is defined in `InternationalPaymentsAPI/Properties/launchSettings.json` as:

```text
https://localhost:7077
```

## Runtime HTTPS protections

`Program.cs` enables HTTPS redirection and HSTS:

```csharp
app.UseHttpsRedirection();
app.UseHsts();
```

The API also sends defensive browser/security headers:

```csharp
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
Referrer-Policy: no-referrer
Content-Security-Policy: frame-ancestors 'none'
```

## Azure App Service deployment

The production API is deployed behind Azure App Service HTTPS. Keep these platform settings enabled for assessment and production:

- HTTPS Only: `On`
- Minimum TLS version: `1.2` or later
- FTPS state: `Disabled` or `FTPS Only`
- Custom domain certificates: use Azure-managed certificates or an uploaded certificate from Key Vault

The database connection string also enforces encryption:

```text
Encrypt=True;TrustServerCertificate=False
```
