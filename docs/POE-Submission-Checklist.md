# RaceDay Part 2 — POE Submission Checklist

## Project identification
- Project: RaceDay Part 2 RESTful Web API
- Framework: .NET 8 / ASP.NET Core
- Data layer: EF Core Code First and SQL Server
- Security: JWT and role-based authorization
- Testing: xUnit and integration tests
- Repository: `lesediphaltse13/RaceDay-Part-2`

## Before submission
- [ ] Review the lecturer's Part 2 rubric and required file naming.
- [ ] Confirm the solution and API/test projects are present.
- [ ] Verify `global.json` matches the installed .NET 8 SDK.
- [ ] Restore, build and run the automated test suite.
- [ ] Apply the existing migration to a clean SQL Server development database.
- [ ] Demonstrate registration and login in Swagger.
- [ ] Demonstrate Organiser event and category creation.
- [ ] Demonstrate Participant enrolment and own-result viewing.
- [ ] Demonstrate 401 and 403 role restrictions.
- [ ] Capture screenshots with secrets and personal information removed.
- [ ] Inspect the latest GitHub Actions run and record its result.
- [ ] Verify **at least 20 meaningful commits** on `main`.
- [ ] Confirm `README.md`, documentation and implementation agree.
- [ ] Ensure no secrets, database passwords, build artifacts or local configuration are accidentally committed.
- [ ] Export or submit the files in the lecturer's requested format.

## Suggested evidence index
| Evidence | Location / link |
| --- | --- |
| Repository and commit history | GitHub `main` commits |
| CI test execution | GitHub Actions run |
| API routes | `docs/API-Endpoint-Guide.md` |
| Database mapping | `docs/Database-Architecture.md` and `docs/Database-Data-Dictionary.md` |
| JWT and roles | `docs/JWT-Authentication-and-Authorization.md` |
| Swagger walkthrough | `docs/Swagger-Testing-Walkthrough.md` |
| Automated tests | `tests/RaceDay.Tests` |
| Manual test outcomes | `docs/Manual-QA-Acceptance.md` (complete result column after execution) |

## Academic integrity and accuracy
The documents describe repository code and proposed verification. Do not claim unperformed manual tests, deployments or screenshots. Ensure the final submission represents work you understand and can explain during assessment.
