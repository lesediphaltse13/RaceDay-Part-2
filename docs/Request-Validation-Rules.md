# Request Validation and Business Rules

This guide maps DTO validation and controller-level checks to user-visible API outcomes.

## DTO annotations
| DTO | Validated fields |
| --- | --- |
| `RegisterRequest` | Required first/last name (max 50), valid required email (max 100), password minimum 8 |
| `LoginRequest` | Required valid email and password |
| `ProfileUpdateRequest` | Required first/last name (max 50) |
| `EventRequest` | Required event name (max 120), event type, venue, city and province |
| `CategoryRequest` | Category name required |
| `EnrolRequest` | Category ID provided as integer |
| `ResultRequest` | Enrolment ID, optional finish time and position, status |

`[ApiController]` performs model-state validation for data annotation failures. The controllers also enforce rules beyond annotations.

## Controller-level rules
- Registration: email is normalized and checked across organiser and participant tables; duplicates return 409.
- Participant registration: date of birth is mandatory and cannot be in the future; invalid values return 400.
- Event create/update: the event controller validates event dates and the request before persistence; check its `Error` helper for exact constraints.
- Category create/update: distance must be positive, entry fee nonnegative, and maximum participant count positive when provided; invalid values return 400.
- Enrolment: category must exist; registration must still be open; duplicate participant/category entries are rejected; confirmed enrolments are limited by category capacity.
- Enrolment cancellation: an enrolment with a recorded result cannot be cancelled.
- Result creation: path and body enrolment IDs must match; enrolment must be confirmed; only one result per enrolment; status must be nonempty and position positive if provided.
- Ownership: organisers can change only their own events and their categories/results; participants can manage only their own enrolments and profiles.

## Response mapping
| Condition | Typical response |
| --- | --- |
| Invalid request | 400 Bad Request |
| No authentication | 401 Unauthorized |
| Wrong role or owner | 403 Forbidden |
| Missing entity | 404 Not Found |
| Duplicate/dependent data | 409 Conflict |
| Successful create | 201 Created |
| Successful update/delete | 204 No Content |

## Test ideas
Test missing required names, invalid email, future birth date, category distance zero, negative entry fee, duplicate enrolment, full category, closed registration and mismatched result enrolment IDs. Distinguish tests proposed here from tests already implemented.
