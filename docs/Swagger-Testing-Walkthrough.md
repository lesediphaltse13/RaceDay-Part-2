# RaceDay Swagger Testing Walkthrough

## Goal
Demonstrate REST requests, JWT authorization, role restrictions and the organiser-to-participant race workflow using the API's Swagger UI.

## Before testing
1. Install .NET 8 and configure a SQL Server instance as described in the README.
2. Apply the existing EF Core migration with `dotnet ef database update --project src/RaceDay.Api` from the repository root.
3. Configure `Jwt:Key` (minimum 32 UTF-8 bytes), `Jwt:Issuer` and `Jwt:Audience` through user secrets or environment variables.
4. Start the API in Development and open the displayed base URL followed by `/swagger`.

## Demonstration sequence
| Step | Request | Expected result |
| --- | --- | --- |
| 1 | `GET /api/events` without a token | 200 OK |
| 2 | `POST /api/auth/register/organizer` | 201 Created for a new email |
| 3 | `POST /api/auth/register/participant` | 201 Created with a valid date of birth |
| 4 | `POST /api/auth/login` with organiser credentials | 200 OK with JWT |
| 5 | Authorize Swagger using the organiser JWT | Bearer token attached to protected requests |
| 6 | `POST /api/events` | 201 Created; save `eventId` |
| 7 | `POST /api/events/{eventId}/categories` | 201 Created; save `categoryId` |
| 8 | Log in as participant and re-authorize | New participant JWT |
| 9 | `POST /api/enrolments` with `categoryId` | 201 Created; save `enrolmentId` |
| 10 | `GET /api/enrolments/my` | 200 OK, includes enrolment |
| 11 | Log in as organiser and re-authorize | Organiser JWT |
| 12 | `POST /api/enrolments/{enrolmentId}/result` | 201 Created for confirmed enrolment |
| 13 | Log in as participant; `GET /api/results/my` | 200 OK with own result |

## Negative scenarios
- Without authentication, `POST /api/events` should return 401.
- A participant attempting `POST /api/events` should return 403.
- An incorrect password at `POST /api/auth/login` should return 401.
- Registering an email that already exists should return 409.
- A duplicate enrolment should return 409.

## Request body reference
Use the Swagger schemas for `RegisterRequest`, `LoginRequest`, `EventRequest`, `CategoryRequest`, `EnrolRequest` and `ResultRequest`. Choose future event and registration-closing dates when testing enrolment.

## Evidence
Capture screenshots showing endpoint/method, request body with secrets redacted, status code, and response. Never publish a live JWT, password or connection string. This is a manual walkthrough; completion must be recorded by the person running it.
