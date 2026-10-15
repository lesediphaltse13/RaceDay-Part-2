# RaceDay Part 2 — School Guide Implementation Notes

## Guide 1 — RESTful Web Services in ASP.NET Core

Adapt the student resource `/api/students` to RaceDay resources: `/api/events`, `/api/categories/{id}`, `/api/enrolments`, and `/api/results/{id}`. Controllers use `[ApiController]`, route attributes, `ControllerBase`, JSON responses, and appropriate HTTP methods. For example, `GET /api/events/{id}` returns `200 OK` or `404 Not Found`, while `POST /api/events` returns `201 Created` for an authorised organiser. These follow the guide's REST and HTTP-status-code examples.

## Guide 2 — Student Management API + EF Core InMemory

The guide teaches the `Models`, `Data`, `Controllers`, and `Program.cs` separation, constructor dependency injection, async EF Core queries, and Swagger testing. RaceDay uses exactly this structure, extended with DTOs and services. The lecturer's example uses InMemory for its running application; the **RaceDay Part 2 brief instead requires SQL Server and Code First**, so `Program.cs` registers `UseSqlServer`. The EF Core InMemory provider is used only in unit tests.

## Guide 3 — Student Web API Unit Testing + EF Core InMemory

`tests/RaceDay.Tests/EventsControllerUnitTests.cs` follows Arrange–Act–Assert and gives each test a unique InMemory database using `Guid.NewGuid().ToString()`. Tests cover success (`200`) and failure (`404`) paths. `ApiTests.cs` additionally tests the complete HTTP pipeline with SQLite in-memory: unauthenticated requests (`401`), incorrect role (`403`), login, duplicate registration, event creation, and participant enrolment. Unit tests alone do not execute the ASP.NET Core authorisation middleware, hence the integration tests.

## API testing checklist in Swagger

1. Start the API after configuring SQL Server, migrations and `Jwt:Key`.
2. `POST /api/auth/register/organizer` with a unique organiser email.
3. `POST /api/auth/register/participant` with a different email and valid `dateOfBirth`.
4. `POST /api/auth/login` and copy the returned `token`.
5. Click Swagger **Authorize**, enter the JWT and create an event as organiser.
6. Add a category, then log in as participant and enrol.
7. Verify participant cannot create events (`403`) and anonymous users cannot create events (`401`).
8. Log in as organiser, record results, and verify participant can view their own results.
9. Run `dotnet test RaceDay.slnx` and confirm a green GitHub Actions workflow before capturing submission evidence.

## Academic scope and caveats

- The lecturer's sample projects use .NET 8; this RaceDay project targets .NET 8. Package major versions match the target framework.
- The guides teach basic CRUD and unit tests; JWT role-based security, SQL Server Code First and CI/CD come from the RaceDay Part 2 assessment specification.
- The school documents do not provide RaceDay's ERD or endpoint plan. Those come from the existing Part 1 documents.
- Do not claim test success until the tests have actually run. The environment used to prepare this package does not contain the .NET SDK.
