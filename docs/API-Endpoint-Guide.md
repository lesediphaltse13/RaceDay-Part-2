# RaceDay Part 2 — REST API Endpoint Guide

## Overview

RaceDay Part 2 is an ASP.NET Core 8 Web API for managing race events, event categories, participant enrolments, profiles, and race results. It uses JWT bearer authentication and role-based authorisation.

**Base path:** `/api`  
**Roles:** `Organiser` and `Participant` (use these exact spellings in role checks).  
**Content type:** `application/json` for JSON request and response bodies.

## Authentication

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| POST | `/api/auth/register/organizer` | Anonymous | Register an organiser |
| POST | `/api/auth/register/participant` | Anonymous | Register a participant |
| POST | `/api/auth/login` | Anonymous | Authenticate and obtain a JWT |

The registration and login endpoints accept JSON request bodies. Consult Swagger for the current request schema (`RegisterRequest` and `LoginRequest`).

For protected requests, supply the issued JWT:

```http
Authorization: Bearer <your-jwt-token>
```

## Events

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| GET | `/api/events` | Anonymous | List events; optional `type` and `city` query filters |
| GET | `/api/events/{id}` | Anonymous | Retrieve one event |
| GET | `/api/events/organizers/events` | Organiser | List events owned by the signed-in organiser |
| POST | `/api/events` | Organiser | Create an event |
| PUT | `/api/events/{id}` | Organiser; event owner | Update an event |
| DELETE | `/api/events/{id}` | Organiser; event owner | Delete an event, subject to category restrictions |

**Example:** `GET /api/events?type=Road&city=Pretoria`

Create and update operations accept an `EventRequest` JSON body. The event owner is determined from the authenticated account, not a client-supplied organiser ID.

## Event categories

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| GET | `/api/events/{eventId}/categories` | Anonymous | List categories for an event |
| POST | `/api/events/{eventId}/categories` | Organiser; event owner | Create a category |
| PUT | `/api/categories/{id}` | Organiser; event owner | Update a category |
| DELETE | `/api/categories/{id}` | Organiser; event owner | Delete a category, subject to enrolment restrictions |

Create and update operations accept a `CategoryRequest` JSON body.

## Enrolments

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| POST | `/api/enrolments` | Participant | Enrol in an event category |
| GET | `/api/enrolments/my` | Participant | List the signed-in participant's enrolments |
| GET | `/api/enrolments/{id}` | Authenticated; owner or relevant organiser | Retrieve an enrolment |
| DELETE | `/api/enrolments/{id}` | Participant; enrolment owner | Cancel an enrolment |
| GET | `/api/events/{eventId}/enrolments` | Organiser; event owner | List enrolments for an event |

Creating an enrolment accepts an `EnrolRequest` JSON body. The API checks the category and its registration-closing date, and rejects duplicate enrolments for the same participant and category.

## Profiles

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| GET | `/api/organizers/profile` | Organiser | View own organiser profile |
| PUT | `/api/organizers/profile` | Organiser | Update own organiser profile |
| GET | `/api/participants/profile` | Participant | View own participant profile |
| PUT | `/api/participants/profile` | Participant | Update own participant profile |

Profile updates accept a `ProfileUpdateRequest` JSON body. Profile endpoints operate on the authenticated user rather than accepting a user ID in the route.

## Results

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| POST | `/api/enrolments/{enrolmentId}/result` | Organiser; event owner | Record a result for an enrolment |
| GET | `/api/results/my` | Participant | List own race results |
| GET | `/api/results/{id}` | Authenticated; result owner or relevant organiser | Retrieve a result |
| GET | `/api/events/{eventId}/results` | Organiser; event owner | List results for an event |
| PUT | `/api/results/{id}` | Organiser; event owner | Update a result |
| DELETE | `/api/results/{id}` | Organiser; event owner | Delete a result |

Result creation and updates accept a `ResultRequest` JSON body. A result can only be recorded against an eligible enrolment; the API also checks for an existing result for that enrolment.

## Common HTTP responses

| Status | Meaning |
|---|---|
| `200 OK` | Successful read or operation returning content |
| `201 Created` | Resource created successfully |
| `204 No Content` | Successful update or deletion without a response body |
| `400 Bad Request` | Invalid request or failed validation/business rule |
| `401 Unauthorized` | Missing or invalid authentication token |
| `403 Forbidden` | Signed-in user lacks the required role or ownership |
| `404 Not Found` | Requested resource does not exist |
| `409 Conflict` | Operation conflicts with existing records or dependencies |

Actual status codes depend on the endpoint and condition. Use Swagger and the controller implementations as the authoritative source for individual response schemas.

## Testing the API

1. Start the API using `dotnet run --project src/RaceDay.Api` from the repository root, with a valid database connection and JWT configuration.
2. In Development mode, open `/swagger` at the base address shown in the terminal.
3. Register an organiser or participant, then log in to obtain a JWT.
4. Select **Authorize** in Swagger and provide the bearer token as prompted.
5. Test endpoints with the appropriate role. A participant should not be able to create an event; an organiser should not be able to enrol as a participant.
6. Run the automated suite using `dotnet test RaceDay.sln --no-restore`.

## Implementation references

- `src/RaceDay.Api/Controllers/AuthController.cs`
- `src/RaceDay.Api/Controllers/EventsController.cs`
- `src/RaceDay.Api/Controllers/CategoriesController.cs`
- `src/RaceDay.Api/Controllers/EnrolmentsController.cs`
- `src/RaceDay.Api/Controllers/ProfilesController.cs`
- `src/RaceDay.Api/Controllers/ResultsController.cs`
- `src/RaceDay.Api/DTOs/Requests.cs`

This document describes the endpoint attributes and controller behaviour inspected for RaceDay Part 2. If the API changes, update this guide in the same commit as the corresponding endpoint changes.
