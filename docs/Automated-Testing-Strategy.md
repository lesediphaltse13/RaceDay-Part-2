# Automated Testing Strategy

## Scope
The solution includes `tests/RaceDay.Tests` using xUnit and ASP.NET Core integration testing. The API is built on .NET 8.

## Test layers
| Layer | Purpose | Tools |
| --- | --- | --- |
| Model/controller-focused tests | Exercise isolated business behavior | xUnit, EF Core InMemory where configured |
| HTTP integration tests | Exercise routing, JWT authentication, authorization and persistence together | `WebApplicationFactory<Program>`, SQLite in-memory |
| Manual API demonstration | Show requests and status codes in a browser | Swagger UI |

## Existing integration test scenarios
The `ApiTests` suite includes scenarios for:
- Anonymous access to public event listing.
- 401 for unauthenticated event creation.
- Registration and login behavior.
- Duplicate registration conflict.
- Wrong-password rejection.
- 403 when a participant attempts organiser-only event creation.
- Organiser event/category creation and participant enrolment.

Additional test cases may be present in the project; inspect the test source and runner output for the current total.

## Running tests
From the repository root:
```powershell
dotnet restore RaceDay.sln
dotnet test RaceDay.sln --configuration Release
```

The API requires JWT configuration. For local test-only use:
```powershell
$env:Jwt__Key = "RaceDayLocalTestSigningKey2026AtLeast32Bytes!"
$env:Jwt__Issuer = "RaceDay.Api"
$env:Jwt__Audience = "RaceDay.Client"
dotnet test RaceDay.sln --configuration Release
```

The sample key is public and must never be used in production.

## Test quality checklist
- Use Arrange–Act–Assert.
- Keep test data isolated.
- Verify both success and failure paths.
- Assert status codes and relevant response fields.
- Test role ownership, not only role labels.
- Ensure tests do not depend on a running local SQL Server.

## Reporting
Record the date, commit SHA, command, number passed/failed/skipped and a link to the CI run. This document does not assert that tests were executed as part of its creation.
