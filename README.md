# International Payments App Service

ASP.NET Core backend API for the International Payments application.

## Run Locally

```bash
dotnet restore InternationalPaymentsAPI/InternationalPaymentsAPI.csproj
dotnet run --project InternationalPaymentsAPI/InternationalPaymentsAPI.csproj
```

## DevSecOps Pipeline Setup

The backend uses the same DevSecOps pattern as the frontend.

| Area | Backend setup |
| --- | --- |
| Main toolchain | .NET 10 SDK, ASP.NET Core, NuGet |
| GitHub Actions | `.github/workflows/devsecops.yml` |
| CircleCI | `.circleci/config.yml` |
| SonarCloud config | `sonar-project.properties` |
| Security checks | NuGet vulnerability scan, security middleware check, CodeQL, dependency review |
| Deployment target | Azure App Service backend API |

Required GitHub repository settings:

| Type | Name |
| --- | --- |
| Repository variable | `AZURE_WEBAPP_NAME` |
| Repository secret | `AZURE_WEBAPP_PUBLISH_PROFILE` |

Required CircleCI environment variable:

| Name | Purpose |
| --- | --- |
| `SONAR_TOKEN` | Publishes CircleCI analysis to SonarCloud/SonarQube |

Tool declarations and exact versions are kept in the pipeline files so CI remains the source of truth.
