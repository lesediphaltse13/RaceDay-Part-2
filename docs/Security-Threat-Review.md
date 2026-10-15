# RaceDay Security Threat Review

## Context
RaceDay exposes authentication and race-management APIs. This is an educational threat review, not a penetration test or security certification.

| Threat | Current protection | Residual risk / recommendation |
| --- | --- | --- |
| Password disclosure | ASP.NET Core PasswordHasher stores password hashes | Require HTTPS and strong password policy in deployment |
| Forged JWT | HMAC-SHA256 signature with issuer/audience/lifetime validation | Keep signing key secret and rotate it if exposed |
| Missing JWT | Bearer authentication rejects protected requests | Confirm every sensitive endpoint is appropriately decorated |
| Privilege escalation | `[Authorize(Roles=...)]` and ownership checks | Test cross-account and cross-organiser access |
| Duplicate registration | Email existence check; unique indexes per account table | Consider concurrency and cross-table uniqueness strategy |
| Duplicate enrolment | Unique index on participant/category | Confirm database exceptions are handled cleanly |
| Oversubscribed category | Controller checks confirmed enrolment count | Concurrent requests can race; consider transaction/locking |
| Accidental data deletion | Restrictive foreign-key delete behavior | Plan retention and administrative recovery |
| Exposed credentials | User secrets/environment configuration recommended | Never commit production secrets or live tokens |
| Sensitive errors | Standard HTTP errors for common failures | Avoid returning internal exception details in production |

## Security boundaries
- Organiser role manages events and their categories/results.
- Participant role enrols and views their own records.
- JWT claims identify the authenticated user.
- SQL Server is the production database provider; SQLite is used for integration tests.

## Manual security tests
1. Access a protected route without a token: expect 401.
2. Authenticate as Participant and attempt event creation: expect 403.
3. Authenticate as another Organiser and attempt to update an event not owned by them: expect 403.
4. Attempt to retrieve another participant's enrolment or result: expect 403.
5. Try duplicate registration and duplicate enrolment: expect 409.
6. Verify secrets and tokens are absent from committed files and screenshots.

## Scope limitations
No claim is made here that rate limiting, token revocation, account lockout, refresh tokens, security headers, centralized audit logging or a production security assessment is implemented.
