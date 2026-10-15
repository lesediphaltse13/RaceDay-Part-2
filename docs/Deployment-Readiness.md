# Deployment Readiness Checklist

## Status
RaceDay is a .NET 8 educational API with SQL Server, JWT authentication, Swagger in Development and GitHub Actions build/test. This checklist describes requirements before a real deployment; it is **not** a claim that production deployment is complete.

## Configuration
- [ ] Select an ASP.NET Core hosting environment supporting .NET 8.
- [ ] Provision a reachable SQL Server instance and a dedicated least-privilege database login.
- [ ] Supply `ConnectionStrings:DefaultConnection` securely.
- [ ] Generate a unique high-entropy `Jwt:Key` of at least 32 UTF-8 bytes.
- [ ] Set `Jwt:Issuer` and `Jwt:Audience` to deployed values.
- [ ] Keep production secrets out of `appsettings.json`, Git and screenshots.
- [ ] Configure HTTPS, certificate renewal and appropriate firewall rules.

## Database
- [ ] Back up the database before schema changes.
- [ ] Review the EF Core migration SQL before applying it.
- [ ] Apply existing migrations once per target database.
- [ ] Verify role seed data and foreign-key constraints.
- [ ] Test database connectivity from the hosting environment.

## API and operations
- [ ] Verify authentication and authorization in the deployed environment.
- [ ] Keep Swagger disabled or access-restricted in Production (current `Program.cs` enables it only in Development).
- [ ] Set up request logging, exception monitoring and backups.
- [ ] Review account lockout, rate limiting and JWT rotation needs.
- [ ] Validate response behavior without leaking internal details.
- [ ] Perform manual end-to-end testing with disposable accounts.

## Release verification
```powershell
dotnet restore RaceDay.sln
dotnet build RaceDay.sln --configuration Release
dotnet test RaceDay.sln --configuration Release
```

Use environment-appropriate JWT test settings locally; never use the repository's sample key for deployment. Record the actual command outputs and GitHub Actions status before claiming a successful release.
