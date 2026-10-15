# RaceDay Requirements Traceability Matrix

## Purpose
Link the Part 2 REST API learning objectives to concrete implementation and verification locations. This is a project-oriented matrix; compare it with the official assessment rubric before submission.

| Requirement / capability | Implementation evidence | Verification approach |
| --- | --- | --- |
| ASP.NET Core REST API | `src/RaceDay.Api/Program.cs`; controllers | Build and call routes in Swagger |
| HTTP GET/POST/PUT/DELETE | Event, category, enrolment, profile and result controllers | Inspect methods and response codes |
| Entity Framework Core | `Data/RaceDayDbContext.cs` | Inspect mappings and run integration tests |
| SQL Server provider | `Program.cs` `UseSqlServer` | Connect to a local SQL Server database |
| Code First schema | `src/RaceDay.Api/Migrations` | Inspect migration and apply to empty DB |
| Entity relationships | `Models/Entities.cs`; DbContext | Review foreign keys and constraints |
| Authentication | `AuthController.cs`; `TokenService.cs` | Register, log in and inspect JWT |
| Authorization | Controller `[Authorize]` attributes | Verify 401, 403 and ownership restrictions |
| Request validation | `DTOs/Requests.cs`; controller rules | Submit valid and invalid JSON |
| Swagger API documentation | `Program.cs` | Open `/swagger` in Development |
| Automated testing | `tests/RaceDay.Tests` | Run `dotnet test RaceDay.sln` |
| CI | `.github/workflows/part2-ci.yml` | Review GitHub Actions run |
| Git history | Repository `main` commit log | Verify at least 20 meaningful commits |

## Evidence standard
For each row, collect source file path, test name or Swagger screenshot, expected outcome, observed outcome and commit SHA. An implementation file proves the feature exists in source; only an actual test/run proves it executes correctly.

## Submission note
This document is supplementary. The lecturer's exact rubric and submission instructions take precedence over this matrix.
