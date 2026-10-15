# RaceDay Database Data Dictionary

## Purpose
Identify the seven EF Core entities and key columns in `RaceDayDbContext`. Database names and relationships follow `src/RaceDay.Api/Data/RaceDayDbContext.cs` and `src/RaceDay.Api/Models/Entities.cs`.

| Table | Primary key | Important fields | Relationship |
| --- | --- | --- | --- |
| `UserRole` | `RoleId` | `RoleName` (unique, max 30) | Referenced by organisers and participants |
| `Organizers` | `OrganiserId` | `RoleId`, `FirstName`, `LastName`, `Email`, `PasswordHash`, `PhoneNumber`, `CreatedAt` | One organiser has many events |
| `Participants` | `ParticipantId` | `RoleId`, names, `Email`, `PasswordHash`, `DateOfBirth`, `CreatedAt` | One participant has many enrolments |
| `Events` | `EventId` | `OrganiserId`, `EventName`, `EventType`, dates, `Venue`, `City`, `Province` | One event has many categories |
| `EventCategories` | `CategoryId` | `EventId`, `CategoryName`, `DistanceKm`, `EntryFee`, `MaximumParticipants` | One category has many enrolments |
| `Enrolments` | `EnrolmentId` | `ParticipantId`, `CategoryId`, `Status`, `RaceNumber`, `EnrolmentDate` | One enrolment may have one result |
| `Results` | `ResultId` | `EnrolmentId`, `FinishTime`, `Position`, `Status`, `RecordedAt` | One-to-one with enrolment |

## Important constraints
- `Organizers.Email` and `Participants.Email` each have a unique index. The registration controller additionally checks email across both tables.
- `UserRole.RoleName` is unique.
- `Enrolments(ParticipantId, CategoryId)` has a composite unique index.
- `Enrolments.RaceNumber` has a filtered unique index for non-null values.
- `EventCategories.DistanceKm` uses decimal precision (6,2); `EntryFee` uses (10,2).
- `Results.FinishTime` is stored as `time(0)`.
- The relationships use restrictive delete behavior to avoid unintended cascades.
- Seed roles: ID 1 `Organiser`, ID 2 `Participant`.

## Verification
Open SQL Server Management Studio after applying the existing migration and inspect tables, keys and indexes. Compare actual schema with the migration snapshot before changing it. Do not run the Part 1 SQL planning script over a Code First database.
