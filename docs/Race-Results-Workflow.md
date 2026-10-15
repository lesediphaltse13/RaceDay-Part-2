# Race Result Recording Workflow

## Workflow
1. An organiser creates an event and category.
2. A participant enrols and receives a confirmed enrolment.
3. The event organiser submits `POST /api/enrolments/{enrolmentId}/result`.
4. The API verifies the path ID matches `ResultRequest.EnrolmentId`.
5. It loads the enrolment, category and event to verify organiser ownership.
6. It rejects cancelled or otherwise unconfirmed enrolments and duplicate results.
7. A valid result is stored and returned with HTTP 201.
8. The participant retrieves their result using `GET /api/results/my`.

## Available endpoints
| Method | Route | Access |
| --- | --- | --- |
| POST | `/api/enrolments/{enrolmentId}/result` | Event organiser |
| GET | `/api/results/my` | Participant |
| GET | `/api/results/{id}` | Result participant or event organiser |
| GET | `/api/events/{eventId}/results` | Event organiser |
| PUT | `/api/results/{id}` | Event organiser |
| DELETE | `/api/results/{id}` | Event organiser |

## Validation
The result status cannot be empty. Position may be omitted but must be positive when supplied. Finish time is optional. A result belongs to exactly one enrolment. Updating a result cannot change its enrolment ID; it updates finish time, position, status and recorded timestamp.

## Data protection
Participant results are filtered by the signed-in participant ID. Event-wide result lists require organiser ownership. A valid token alone does not grant access to another user's result.

## Manual evidence
Record one successful result creation and one rejected attempt (duplicate result or unauthorised organiser). Save response status and a redacted body; do not publish JWTs.
