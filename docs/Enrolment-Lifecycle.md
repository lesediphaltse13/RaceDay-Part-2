# Enrolment Lifecycle

## Actors
An Organiser creates events and categories. A Participant enrols in an existing category. The API identifies each actor using JWT claims rather than client-supplied user IDs.

## Creation
1. A participant sends `POST /api/enrolments` with `categoryId`.
2. The API looks up the category and its event.
3. It checks the event registration closing date.
4. It checks that this participant is not already enrolled in that category.
5. If `MaximumParticipants` is configured, it counts confirmed enrolments and rejects a full category.
6. The API creates an `Enrolment` and responds with HTTP 201.

## Viewing
- `GET /api/enrolments/my`: participant's own enrolments.
- `GET /api/enrolments/{id}`: participant owner or organiser of the related event.
- `GET /api/events/{eventId}/enrolments`: event owner only.

## Cancellation
`DELETE /api/enrolments/{id}` requires the participant role and enrolment ownership. If a result exists, the request returns 409. Otherwise the controller changes `Status` to `Cancelled` and returns 204. **Cancellation is a status update, not a physical row deletion.**

## Capacity and uniqueness
- A unique database index on `(ParticipantId, CategoryId)` prevents repeated enrolment records for the same pair.
- Capacity is based on enrolments whose status is `Confirmed`.
- A cancelled record remains in the database, so attempting to enrol in the same category again currently triggers the duplicate check.
- The capacity check and insert are separate operations; concurrent requests may require stronger transaction handling in a production deployment.

## Suggested tests
Try duplicate enrolment, full capacity, closed registration, viewing another participant's enrolment, cancellation with an existing result, and re-enrolment after cancellation. Record actual results rather than assuming all scenarios are covered by the current automated suite.
