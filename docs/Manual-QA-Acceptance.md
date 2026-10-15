# Manual Quality Assurance and Acceptance Checklist

## Test record
- Date:
- Tester:
- Commit SHA:
- Environment: Development / other
- Database: SQL Server instance
- API URL:
- Result: Pass / Fail / Blocked

## Functional acceptance cases
| ID | Test | Expected result | Actual / evidence |
| --- | --- | --- | --- |
| QA-01 | Browse events anonymously | HTTP 200 | |
| QA-02 | Register a new organiser | HTTP 201 | |
| QA-03 | Register a participant with valid DOB | HTTP 201 | |
| QA-04 | Register duplicate email | HTTP 409 | |
| QA-05 | Log in with wrong password | HTTP 401 | |
| QA-06 | Create event without JWT | HTTP 401 | |
| QA-07 | Create event as Participant | HTTP 403 | |
| QA-08 | Create event as Organiser | HTTP 201 | |
| QA-09 | Create category for own event | HTTP 201 | |
| QA-10 | Enrol Participant in open category | HTTP 201 | |
| QA-11 | Repeat same enrolment | HTTP 409 | |
| QA-12 | Read own enrolments | HTTP 200 | |
| QA-13 | Record result as owning Organiser | HTTP 201 | |
| QA-14 | Read own results as Participant | HTTP 200 | |
| QA-15 | Access another participant's result | HTTP 403 | |
| QA-16 | Update another organiser's event | HTTP 403 | |
| QA-17 | Cancel enrolment with existing result | HTTP 409 | |
| QA-18 | Attempt invalid category distance | HTTP 400 | |
| QA-19 | Run automated tests | Record actual pass/fail count | |
| QA-20 | Review GitHub Actions | Record actual status and run URL | |

## Evidence rules
Record real outputs only. Use separate accounts for ownership tests, use future dates for enrolment, and redact tokens/passwords. A blank result column means **not yet executed**, not passed.

## Exit criteria
No unresolved blocking failures in core registration, login, event/category creation, enrolment, result recording or access control; passing automated suite; and an inspected CI run.
