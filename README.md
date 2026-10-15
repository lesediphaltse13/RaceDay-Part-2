# RaceDay — Part 2 RESTful API (.NET 8)

**Student:** Lesedi Phalatse  
**Project:** RaceDay, South African road-event management  
**Framework:** .NET 8 / ASP.NET Core Web API  
**Database:** Microsoft SQL Server + Entity Framework Core 8 (Code First)  
**Testing:** xUnit + EF Core InMemory + SQLite integration tests  
**Documentation:** Swagger UI

RaceDay supports organisers who manage running, walking and cycling events and participants who enter categories and view results. This repository follows the Part 1 ERD and API plan and applies the three lecturer guides (REST, EF Core, unit testing).

## 1. Requirements to install

On Windows install:

1. **Visual Studio 2022** (or a newer Visual Studio with .NET 8 support). During installation tick **ASP.NET and web development**.
2. **.NET 8 SDK** from https://dotnet.microsoft.com/download/dotnet/8.0 (the SDK, not just the runtime).
3. **SQL Server Express** or **SQL Server Developer Edition**. SQL Server LocalDB can also be used.
4. **SQL Server Management Studio (SSMS)** to check your SQL instance and database.
5. **Git** if you plan to push to GitHub.

Open **PowerShell** and confirm the SDK:

```powershell
dotnet --list-sdks
```

You should see an installed version beginning with `8.0.`. You can have other SDKs installed too.

## 2. Open the solution

1. Extract the downloaded ZIP to a normal folder (for example `Documents\RaceDay`).
2. Open Visual Studio → **Open a project or solution** → select `RaceDay.slnx`.
3. If your Visual Studio version does not support `.slnx`, open `src/RaceDay.Api/RaceDay.Api.csproj` and then add the test project, or upgrade Visual Studio.
4. In Visual Studio select **Tools → NuGet Package Manager → Package Manager Console**. You can also use **View → Terminal**.
5. Run these commands **from the folder containing `RaceDay.slnx`**:

```powershell
dotnet restore RaceDay.slnx
dotnet build RaceDay.slnx
```

**Do not use .NET 10 or EF Core 10 packages** for this project. The project files specify .NET 8 and matching EF Core 8 packages.

## 3. Install / verify Entity Framework Core packages

NuGet packages are already declared in the `.csproj` files. `dotnet restore` downloads them automatically. To understand the required packages, these are the equivalent installation commands.

**From the repository root** (do not add the packages twice if they are already listed):

```powershell
# Database provider for the actual API
dotnet add src/RaceDay.Api/RaceDay.Api.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.8

# EF Core design-time tools for migrations
dotnet add src/RaceDay.Api/RaceDay.Api.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.8

# EF Core test providers (test project only)
dotnet add tests/RaceDay.Tests/RaceDay.Tests.csproj package Microsoft.EntityFrameworkCore.InMemory --version 8.0.8
dotnet add tests/RaceDay.Tests/RaceDay.Tests.csproj package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.8
```

The API also uses `Microsoft.AspNetCore.Authentication.JwtBearer` **8.0.8**, `Swashbuckle.AspNetCore` **6.6.2** and `Microsoft.OpenApi` **1.6.14**. These Swagger versions keep `using Microsoft.OpenApi.Models;` compatible with the existing `Program.cs` and avoid the previous `CS0234` namespace mismatch.

Install the **dotnet-ef 8** command-line tool (one-time installation):

```powershell
dotnet tool install --global dotnet-ef --version 8.0.8
```

If the tool is already installed at a different major version, run:

```powershell
dotnet tool update --global dotnet-ef --version 8.0.8
```

Confirm with `dotnet ef --version`. The output should begin with `8.0.`.

### Visual Studio Package Manager Console alternative

If you prefer Visual Studio's **Package Manager Console**, choose `RaceDay.Api` in **Default project** and run:

```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.8
Install-Package Microsoft.EntityFrameworkCore.Design -Version 8.0.8
```

For `RaceDay.Tests`, choose that project in the **Default project** dropdown and install `Microsoft.EntityFrameworkCore.InMemory` and `Microsoft.EntityFrameworkCore.Sqlite` version `8.0.8`. These steps are **alternatives**, not extra steps after `dotnet restore`.

## 4. Start SQL Server and identify your server name

1. Open **SSMS** and select **Connect → Database Engine**.
2. Try the server name for your installed instance:
   - SQL Server Express: `.\SQLEXPRESS` (often `localhost\SQLEXPRESS`)
   - Default SQL Server instance: `localhost` or `.`
   - LocalDB: `(localdb)\MSSQLLocalDB`
3. Select **Windows Authentication**, then click **Connect**.
4. If connection fails, check **SQL Server Configuration Manager** or **Windows Services** to confirm that the SQL Server service for your instance is running. LocalDB starts on demand.
5. In SSMS, expand **Databases**. The `RaceDayDB` database will be created by EF Core migrations in the next step.

**Important:** You do **not** need to run `docs/RaceDay-Database.sql` to create the Part 2 tables. That file is your Part 1 planning artefact. The assessment requires **Code First**; EF Core migrations must create the database schema.

## 5. Connect the API to SQL Server

Open `src/RaceDay.Api/appsettings.json`. Locate:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Change `Server=` to the instance name you successfully connected to in SSMS. Examples:

**SQL Server Express:**

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

**LocalDB:**

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

**Default SQL Server instance:**

```json
"DefaultConnection": "Server=localhost;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

> In JSON, a backslash must be escaped as `\\` in the source text. Use the examples exactly as written. Do not commit SQL usernames or passwords to GitHub. If you use SQL Authentication, store its connection string in .NET user secrets instead.

## 6. Configure the JWT authentication secret

Passwords are hashed by the API; the JWT signing key is a separate secret used to sign login tokens. **Never commit the key to GitHub.**

From the repository root, run:

```powershell
cd src/RaceDay.Api
dotnet user-secrets init
```

Generate a random signing key using PowerShell (in the same terminal):

```powershell
$bytes = New-Object byte[] 48
[System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
$key = [Convert]::ToBase64String($bytes)
dotnet user-secrets set "Jwt:Key" "$key"
```

The project expects a signing key of at least 32 UTF-8 bytes. User secrets keep the key outside source control.

## 7. Create the database using EF Core Code First

Make sure you are in `src/RaceDay.Api` (where `RaceDay.Api.csproj` is located). Then run:

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

What happens:

- `migrations add` creates a `Migrations` folder with the database creation instructions.
- `database update` connects to the SQL Server instance from `appsettings.json`, creates `RaceDayDB` if necessary, and creates the seven RaceDay tables.
- The model also seeds the organiser and participant role records.

Open SSMS → **Databases** → **RaceDayDB** → **Tables** and verify the tables exist. The main tables are `UserRole`, `Organizers`, `Participants`, `Events`, `EventCategories`, `Enrolments` and `Results`.

If you have already generated a migration in this project, **do not create `InitialCreate` again**. Use `dotnet ef database update` for the existing migrations.

**If you created RaceDayDB manually from the Part 1 script:** Do not apply a fresh initial migration over those existing tables. Use a new empty development database or reconcile the existing schema first; otherwise you may see `There is already an object named ...` errors.

## 8. Run the RaceDay API

Still inside `src/RaceDay.Api`, execute:

```powershell
dotnet run --launch-profile http
```

Open your browser at:

**http://localhost:5088/swagger**

You should see the RESTful endpoints grouped by controller. If the URL differs, use the address printed in the terminal (`Now listening on:`).

## 9. Test registration, login and role-based endpoints in Swagger

**Register an organiser** using `POST /api/auth/register/organizer`, and register a participant with `POST /api/auth/register/participant`. Enter the fields shown in the Swagger request schema; use a different email for each account.

**Log in** with `POST /api/auth/login` and copy the returned JWT token.

Click **Authorize** in Swagger, paste the JWT token (Swagger's HTTP bearer scheme adds the `Bearer` prefix), and confirm. Now test protected endpoints:

| Test | Expected behaviour |
| --- | --- |
| `GET /api/events` | Public list of events, 200 OK |
| Organiser `POST /api/events` | Creates event, 201 Created |
| Participant `POST /api/events` | 403 Forbidden |
| Participant `POST /api/enrolments` | Creates enrolment, 201 Created |
| Organiser `POST /api/enrolments` | 403 Forbidden |
| Protected endpoint without a token | 401 Unauthorized |
| Request for missing record | 404 Not Found |

An organiser can create, update and delete their own events and categories, inspect enrolments and manage results. A participant can browse events, enrol, cancel their own enrolment and view their own results.

## 10. Run the automated tests

Return to the solution root:

```powershell
cd ../..
dotnet test RaceDay.slnx
```

Or open Visual Studio → **Test → Test Explorer → Run All**. The xUnit tests use EF Core **InMemory** databases for isolated controller tests, following the lecturer's Arrange–Act–Assert guide. HTTP integration tests use in-memory SQLite to verify routing, authentication and authorisation without requiring your local SQL Server.

**Do not report tests as passed until you have run them and seen the result.**

## 11. GitHub Actions CI/CD

The workflow `.github/workflows/part2-ci.yml` uses .NET 8 to restore, build and run tests automatically on pushes and pull requests.

```powershell
git status
git add .
git commit -m "Update RaceDay Part 2 to .NET 8 with SQL Server setup guide"
git push
```

After pushing, open the GitHub repository → **Actions** → **RaceDay Part 2 CI**. Wait for a green successful run, take a screenshot and save it as `docs/part2-ci-success.png`. Add that screenshot to the README and commit it separately. Make at least **20 meaningful Part 2 commits** representing real changes.

## 12. Common errors and solutions

| Error | Fix |
| --- | --- |
| `CS0234 Microsoft.OpenApi.Models` | Restore the provided .NET 8 package versions; the project uses Swashbuckle 6.6.2 + Microsoft.OpenApi 1.6.14. |
| `NU1202` incompatible packages | Check `.csproj` targets `net8.0` and EF Core/JWT packages are version 8.x, then restore. |
| `dotnet ef` not recognised | Install `dotnet-ef` version 8.0.8 globally. Restart the terminal if necessary. |
| SQL Server error 40 / cannot connect | Check SSMS instance name, SQL Server service and the connection string. |
| `Login failed for user` | Check Windows Authentication access and database permissions. |
| `Jwt:Key` missing | Follow step 6; run the API from `src/RaceDay.Api` in Development. |
| `There is already an object named ...` | A pre-existing SQL schema conflicts with a Code-First migration. Use a clean development DB or reconcile migrations. |
| Swagger does not open | Check the terminal for the correct port and confirm `ASPNETCORE_ENVIRONMENT=Development`. |
| 401 Unauthorized | Log in and Authorize in Swagger with a valid token. |
| 403 Forbidden | The logged-in role is not allowed to use that endpoint. |

## 13. Repository structure

```text
RaceDay/
├── .github/workflows/part2-ci.yml
├── docs/
│   ├── RaceDay-Database.sql
│   ├── RaceDay-ERD.png
│   ├── ST10447256_LesediPhalatse_POE_Part1.pdf
│   └── School-Guides-Implementation.md
├── src/RaceDay.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   ├── appsettings.json
│   └── RaceDay.Api.csproj
├── tests/RaceDay.Tests/
├── RaceDay.slnx
└── README.md
```

## 14. School-guideline alignment

- **RESTful Web Services:** HTTP GET/POST/PUT/DELETE, resource-based routes, JSON, correct response codes.
- **EF Core API guide:** model classes, DbContext, dependency injection, asynchronous EF Core CRUD, Swagger.
- **Unit-testing guide:** xUnit, separate in-memory test databases, Arrange–Act–Assert.
- **RaceDay Part 2 requirements:** SQL Server Code First, JWT-based role authentication, organiser/participant operations, CI.

**Authentication note:** JWT is stateless token-based authentication, not an ASP.NET server-side session. Confirm with your lecturer whether JWT meets the assignment's term *session management*.

## 15. Submission evidence

- [ ] SQL Server migration applied and tables verified in SSMS
- [ ] Swagger endpoints demonstrated
- [ ] Organiser and participant registration/login demonstrated
- [ ] Role restrictions demonstrated (401 and 403)
- [ ] xUnit tests passing locally
- [ ] GitHub Actions green build screenshot saved and linked below
- [ ] 20+ meaningful Part 2 Git commits
- [ ] Unlisted YouTube walkthrough recorded and linked below

**CI screenshot:** Add `![Successful CI run](docs/part2-ci-success.png)` once the file exists.  
**YouTube demonstration:** Add your video URL after recording.  
**GitHub repository:** https://github.com/lesediphaltse13/RaceDay

---

**Developer:** Lesedi Phalatse · Diploma in IT (Software Development) · Portfolio of Evidence.
