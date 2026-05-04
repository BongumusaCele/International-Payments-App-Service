# International-Payments-App-Service

ASP.NET Core backend API for the International Payments customer portal.

## Security evidence

- Passwords are stored with PBKDF2 hashing, per-password random salts, and fixed-time verification.
- Input whitelisting is enforced on request DTOs with regular-expression validation.
- Auth, registration, and MFA endpoints are rate-limited.
- Protected payment and beneficiary endpoints require bearer session authentication.
- Session tokens and MFA OTPs are stored as hashes.
- HTTPS redirection, HSTS, and security headers are configured in `InternationalPaymentsAPI/Program.cs`.

## TLS/SSL setup

See [docs/tls-setup.md](docs/tls-setup.md) for the local certificate/key generation script, Kestrel HTTPS launch instructions, and Azure App Service TLS settings.
