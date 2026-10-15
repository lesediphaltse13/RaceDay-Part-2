# API Error Handling and HTTP Status Codes

## Intent
RaceDay returns HTTP status codes to distinguish success, invalid input, missing data, authentication failures and conflicts.

| Code | Meaning | Example from implementation |
| --- | --- | --- |
| 200 OK | Successful read/login | `GET /api/events`, `POST /api/auth/login` |
| 201 Created | Successful creation | New event, category, registration or result |
| 204 No Content | Successful update or deletion | Updating an event, cancelling an enrolment |
| 400 Bad Request | Invalid request/business validation | Future date of birth; invalid category distance |
| 401 Unauthorized | Missing/invalid credentials | Login with incorrect password; protected request without JWT |
| 403 Forbidden | Authenticated but not permitted | Participant attempting event creation; editing another organiser's event |
| 404 Not Found | Requested entity missing | Unknown event or category ID |
| 409 Conflict | Duplicate or dependent record | Duplicate email, duplicate enrolment, category with enrolments |

## Ownership checks
Controller actions first load the relevant record, then compare the authenticated user ID with the organiser/participant ownership fields. Missing records generally return 404; existing records owned by someone else return 403.

## Consistency considerations
Some controller methods return `new { message = ... }` for validation failures, while others return an empty `BadRequest()` or `NotFound()`. Client applications should use the HTTP status as the primary signal and not assume every error has the same JSON schema.

## Reproduction checklist
1. Make an unauthenticated protected request.
2. Repeat with the wrong role.
3. Request a non-existent ID.
4. Submit invalid category values.
5. Attempt a duplicate enrolment.
6. Compare response codes with controller logic and capture evidence.

## Future improvements (not yet claimed as implemented)
Standardized Problem Details responses, structured logging, centralized exception handling and correlation IDs could improve production diagnostics. These should be developed and tested separately.
