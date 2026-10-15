# RaceDay Part 2 — JWT Authentication and Role-Based Authorization

## Purpose
RaceDay protects operations that change race events, categories, enrolments, profiles, and results using JSON Web Tokens (JWT) and ASP.NET Core role-based authorization. This document describes the implementation in the repository, not a separate proposed security system.

## Registration and login
The authentication controller is routed at `/api/auth`.

| Method | Route | Access | Behavior |
| --- | --- | --- | --- |
| POST | `/api/auth/register/organizer` | Anonymous | Creates an organiser account and returns HTTP 201 |
| POST | `/api/auth/register/participant` | Anonymous | Creates a participant account; requires a valid date of birth; returns HTTP 201 |
| POST | `/api/auth/login` | Anonymous | Checks credentials and returns a signed token, role, and expiry |

Registration normalizes email addresses by trimming and lowercasing them. The API checks for an existing email across both account types and returns HTTP 409 Conflict for duplicates. Passwords are stored using ASP.NET Core's `PasswordHasher<object>`, not as plaintext.

Incorrect login credentials return HTTP 401 Unauthorized. A successful login returns `token`, `role`, and `expiresInSeconds: 7200`.

## JWT creation
`src/RaceDay.Api/Services/TokenService.cs` issues a token containing:
- `ClaimTypes.NameIdentifier`: organiser or participant ID
- `ClaimTypes.Role`: `Organiser` or `Participant`
- `ClaimTypes.Email`: normalized email address

Tokens use HMAC-SHA256 signing and expire two hours after issuance.

## Token validation
`src/RaceDay.Api/Program.cs` configures JWT bearer authentication. It validates:
- The issuer (`Jwt:Issuer`)
- The audience (`Jwt:Audience`)
- The token lifetime
- The signature against the configured signing key

Clock skew is set to 30 seconds. The application checks that the signing key contains at least 32 UTF-8 bytes and refuses to start with an invalid key.

Authentication middleware runs before authorization middleware.

## Roles and permissions
The API distinguishes the **Organiser** and **Participant** roles. Controllers apply `[Authorize]` and role restrictions to protected actions. Public endpoints use anonymous access where appropriate.

Examples verified by the existing integration tests:
- An unauthenticated request to create an event receives **401 Unauthorized**.
- A signed-in Participant attempting to create an event receives **403 Forbidden**.
- An authenticated Organiser can create an event.
- A Participant can enrol in a race through the supported enrolment workflow.
- Public event listing is accessible without a token.

For the complete current route list, see `docs/API-Endpoint-Guide.md` and the controller source files. Consult the attributes on each action as the authoritative permission definition.

## Using a token
1. Register an organiser or participant, or use an existing test account.
2. Send a POST request to `/api/auth/login` with email and password.
3. Copy the returned token.
4. Send it with protected requests in the header: `Authorization: Bearer <token>`.
5. In Development, Swagger UI offers a Bearer authorization control at `/swagger`.

Do not share real tokens in screenshots, documentation, or issue reports.

## Local configuration
The application reads `Jwt:Key`, `Jwt:Issuer`, and `Jwt:Audience` from ASP.NET Core configuration. On Windows PowerShell, temporary environment variables can be set as follows:

```powershell
$env:Jwt__Key = "RaceDayLocalTestSigningKey2026AtLeast32Bytes!"
$env:Jwt__Issuer = "RaceDay.Api"
$env:Jwt__Audience = "RaceDay.Client"
dotnet test RaceDay.sln
```

The key shown here is a public, disposable **test-only** example. Never reuse it for a deployed application. Use user-secrets or a secret manager for real credentials, and do not commit production secrets.

## Automated security checks
The xUnit test project `tests/RaceDay.Tests` includes checks for public event access, missing authentication, incorrect passwords, duplicate registration, role restrictions, and the organiser-to-participant event/enrolment workflow. Integration tests use an isolated SQLite database.

The GitHub Actions workflow at `.github/workflows/part2-ci.yml` supplies disposable JWT settings and runs the .NET 8 solution tests. Check the Actions tab for the latest results rather than assuming a successful local run guarantees a successful CI run.

## HTTP status code reference
| Code | Meaning |
| --- | --- |
| 200 | Request succeeded |
| 201 | Resource created |
| 400 | Invalid request or validation failure |
| 401 | Missing or invalid authentication |
| 403 | Authenticated user lacks permission |
| 409 | Conflicting registration, such as a duplicate email |

## Security considerations
JWT authentication establishes identity and access permissions; it does not by itself provide refresh tokens, account lockout, revocation, or production secret management. Those features should not be claimed unless implemented and tested.
