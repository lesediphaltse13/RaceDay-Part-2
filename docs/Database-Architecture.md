# RaceDay Part 2 — Database Architecture

## 1. Purpose

This document describes the relational database structure used by the RaceDay Part 2 ASP.NET Core Web API. The application uses **SQL Server**, **Entity Framework Core**, and **Code First migrations** to store organisers, participants, race events, categories, enrolments, and race results.

The database supports the main RaceDay workflow: an organiser creates a race event and its categories, a participant enrols in a category, and an organiser records the participant's race result.

## 2. Technology and source files

| Component | Implementation |
|---|---|
| Application framework | ASP.NET Core Web API (.NET 8) |
| Database | Microsoft SQL Server |
| Data access | Entity Framework Core |
| Schema management | EF Core Code First migrations |
| Database context | `src/RaceDay.Api/Data/RaceDayDbContext.cs` |
| Entity definitions | `src/RaceDay.Api/Models/Entities.cs` |
| Initial migration | `src/RaceDay.Api/Migrations/20261013084431_InitialCreate.cs` |
| Model snapshot | `src/RaceDay.Api/Migrations/RaceDayDbContextModelSnapshot.cs` |
| Supporting SQL script | `docs/RaceDay-Database.sql` |
| Entity relationship diagram | `docs/RaceDay-ERD.png` |

## 3. Main entities

| Entity | Responsibility |
|---|---|
| `UserRole` | Defines application roles, including Organiser and Participant. |
| `Organizer` | Stores organiser account and profile information. |
| `Participant` | Stores participant account and profile information. |
| `RaceEvent` | Represents an organised race event, including event details and ownership. |
| `EventCategory` | Represents a category within a race event, such as a Fun Run. |
| `Enrolment` | Records a participant's registration for a specific event category. |
| `RaceResult` | Records a race outcome linked to an enrolment, including finish time, position, and status. |

**Naming note:** The API's authorization role is spelled `Organiser`, while the C# entity is named `Organizer`. These refer to the organiser concept but serve different purposes.

## 4. Entity relationships

The following describes the functional relationships used by the API. Consult the EF Core context, generated migration, and ERD for the authoritative foreign keys, delete behaviours, required fields, and database constraints.

1. **Organiser → Race events:** An organiser manages race events. The API checks event ownership before permitting organiser-specific changes.
2. **Race event → Event categories:** A race event can offer categories. Categories belong to their parent event.
3. **Participant → Enrolments:** A participant can enrol in race categories. An enrolment identifies the participant and selected category.
4. **Event category → Enrolments:** A category can have multiple participant enrolments.
5. **Enrolment → Race result:** A result references an enrolment so the finish information can be associated with the correct participant and category.
6. **User roles → Access control:** The application uses Organiser and Participant roles to control access to API operations. Role enforcement is implemented in the API and must not be assumed to arise from database relationships alone.

### Conceptual relationship flow

```text
Organizer
    |
    +-- RaceEvent
           |
           +-- EventCategory
                    |
                    +-- Enrolment ---- Participant
                              |
                              +-- RaceResult

UserRole: supports application role classification and authorization.
```

This is a **conceptual overview**, not a replacement for the exact ERD.

## 5. Data lifecycle

### 5.1 Create a race event

An authenticated organiser creates a race event through the Events API. The system associates the event with its organiser.

### 5.2 Add event categories

The organiser adds categories to the event. A participant selects one of these categories when registering for the race.

### 5.3 Enrol a participant

An authenticated participant submits an enrolment. The API links the participant to the selected event category and applies business rules, including registration availability.

### 5.4 Record race results

An organiser records a result against an existing enrolment. Race result data includes finish time, position, and status. Participants can retrieve their own results through the Results API.

## 6. Entity Framework Core and migrations

The `RaceDayDbContext` class is the application's EF Core database context. It provides the data access layer used by API controllers. Entity definitions and context configuration are used to generate and maintain the SQL Server schema.

The project includes an `InitialCreate` migration and a model snapshot. These should remain under source control so another developer can recreate the schema consistently.

### Apply existing migrations

From the repository root, after configuring a valid local SQL Server connection string:

```powershell
dotnet ef database update --project src/RaceDay.Api --startup-project src/RaceDay.Api
```

If the EF Core command-line tool is unavailable, install the compatible .NET 8 EF Core tool version before running the command.

### Add a migration after a legitimate model change

```powershell
dotnet ef migrations add DescriptiveMigrationName --project src/RaceDay.Api --startup-project src/RaceDay.Api
```

Review generated migration code before applying it, especially when changing an existing database with data.

## 7. Data integrity and security

- **Foreign-key relationships:** Preserve associations among organisers, events, categories, participants, enrolments, and results.
- **Authorization:** Use JWT authentication and API role/ownership checks to restrict organiser and participant operations.
- **Validation:** Validate submitted requests before storing data, including event details, enrolments, and race results.
- **Credentials:** Do not commit real SQL Server passwords, JWT signing keys, or other secrets. Configure them through secure local settings or deployment secrets.
- **Migration safety:** Back up important databases and review migrations before applying schema changes outside development.
- **Testing isolation:** The automated integration tests use an in-memory SQLite database; they do not require a running SQL Server instance.

## 8. Database design artefacts

The repository includes the following supporting resources:

- [`RaceDay-ERD.png`](RaceDay-ERD.png) — visual database relationship diagram.
- [`RaceDay-Database.sql`](RaceDay-Database.sql) — supporting SQL database script.
- [`School-Guides-Implementation.md`](School-Guides-Implementation.md) — implementation notes related to the supplied learning guides.

The EF Core migration and `RaceDayDbContext` should be checked against the SQL script and ERD whenever the schema changes.

## 9. Verification checklist

- [ ] The SQL Server connection string is configured for the local environment.
- [ ] The application builds successfully using .NET 8.
- [ ] EF Core migrations apply successfully to the intended database.
- [ ] The ERD and SQL script reflect the implemented schema.
- [ ] Organiser and participant permissions behave as intended.
- [ ] Automated tests pass with `dotnet test RaceDay.sln`.

## 10. Conclusion

RaceDay Part 2 uses a relational database design to connect event management, participant enrolment, and race results. Entity Framework Core provides the application data-access layer, while Code First migrations track database schema changes. The combination of database relationships and API-level authorization supports a maintainable RESTful application.
