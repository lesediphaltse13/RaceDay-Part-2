RaceDay Part 2 — Local Development Setup Guide

1. Project Overview

RaceDay Part 2 is an ASP.NET Core Web API developed using .NET 8. The application supports race event management, participant registration, enrolments, race results and role-based access control.

2. Prerequisites

Install the following software:

* .NET 8 SDK
* Visual Studio or Visual Studio Code
* SQL Server or SQL Server Express for the application database
* Git for version control

3. Clone the Repository

git clone https://github.com/lesediphaltse13/RaceDay-Part-2.git
cd RaceDay-Part-2

4. Verify .NET Installation

dotnet --version
dotnet --list-sdks

The repository includes global.json to select the project’s .NET 8 SDK.

5. Restore and Build

Run these commands from the repository root:

dotnet restore RaceDay.sln
dotnet build RaceDay.sln

A successful build should report zero errors.

6. Configure the Database

The application uses Entity Framework Core with SQL Server.

Configure the DefaultConnection connection string for your local SQL Server instance. Keep real database credentials out of Git.

Apply the existing migrations:

dotnet ef database update --project src/RaceDay.Api --startup-project src/RaceDay.Api

Install the dotnet-ef tool if it is not already available.

7. Configure JWT Authentication

The application requires a JWT signing key containing at least 32 bytes.

For local development, use .NET user-secrets or environment variables. Never commit a production signing key to the repository.

For a temporary local PowerShell session:

$env:Jwt__Key = "RaceDayLocalTestSigningKey2026AtLeast32Bytes!"
$env:Jwt__Issuer = "RaceDay.Api"
$env:Jwt__Audience = "RaceDay.Client"

These example values are intended only for development and testing.

8. Run the API

From the repository root:

dotnet run --project src/RaceDay.Api

Use the address displayed in the terminal to access the application. Swagger UI is available at /swagger when running in the Development environment.

9. Run Automated Tests

The project includes xUnit tests for API functionality and controller behaviour.

dotnet test RaceDay.sln --no-restore

The verified local test run completed with:

* 12 tests passed
* 0 tests failed
* 0 tests skipped

The integration tests use an isolated SQLite database rather than requiring a running SQL Server instance.

10. GitHub Actions

The workflow is located at:

.github/workflows/part2-ci.yml

It restores dependencies, builds the solution and runs automated tests using .NET 8.

11. Troubleshooting

MSB1003 — Project or solution file not found

Run commands from the repository root containing RaceDay.sln, or specify the solution path explicitly.

JWT signing key error

Configure Jwt:Key using environment variables or user-secrets. The key must contain at least 32 bytes.

SQL Server connection failure

Verify that SQL Server is running and that the connection string matches your local instance.

Git command not recognised

Install Git for Windows and restart your terminal.
