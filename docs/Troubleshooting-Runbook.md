# RaceDay Troubleshooting Runbook

## Start with the environment
From the solution root:
```powershell
dotnet --list-sdks
dotnet restore RaceDay.sln
dotnet build RaceDay.sln
```
The repository targets .NET 8. Check `global.json` and the API/test project files when SDK or NuGet compatibility errors occur.

## Symptoms and checks
| Symptom | Check | Action |
| --- | --- | --- |
| JWT signing key error at startup | `Jwt:Key` absent or under 32 UTF-8 bytes | Set a valid secret via user secrets or environment |
| HTTP 401 | Missing, invalid or expired JWT | Log in again and send `Authorization: Bearer <token>` |
| HTTP 403 | Wrong role or record ownership | Use an authorised account; verify ownership |
| SQL Server connection error | Server name, instance, service, authentication | Confirm SSMS connection and configuration |
| Migration conflict | Existing schema or repeated initial migration | Inspect migration history; avoid recreating initial migration |
| Swagger unavailable | Hosting environment or URL | Use Development and the actual listening address |
| Test startup failure | Missing JWT settings | Supply test-only `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience` |
| CI red | Restore/build/test step logs | Inspect the failing step in GitHub Actions |
| Duplicate enrolment 409 | Unique participant/category pairing | Review existing enrolment, including cancelled records |
| Category full 409 | Confirmed count at capacity | Select another category or review event settings |

## Safe debugging order
1. Reproduce the failure with the exact endpoint and HTTP method.
2. Capture status code and sanitized response.
3. Identify whether the failure is configuration, authentication, validation, persistence or authorization.
4. Inspect the corresponding controller and relevant test.
5. Apply one targeted fix.
6. Re-run the build/tests and check GitHub Actions after pushing.

Never paste real JWTs, passwords, database connection secrets or personal participant records into public issues.
