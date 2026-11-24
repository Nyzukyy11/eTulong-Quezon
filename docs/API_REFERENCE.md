# API reference

Default local address: http://127.0.0.1:5291/. Dates are YYYY-MM-DD; timestamps are UTC. JSON uses camelCase. Mutations carry X-Demo-Staff with an existing demonstration identity. Seed roles:1 Staff,2 Official,3 Releasing officer; clients should resolve identities from /api/reference. This header is a demo mechanism, not authentication.

| Method / route | Request / result |
|---|---|
| GET /health | PostgreSQL readiness;200 ready,503 unavailable |
| GET /api/reference | Demo staff identities and batches with capacity/reserved/available |
| GET /api/cases?search=&status=All&page=1&size=25 | Bounded case page; size1–50; empty Items array for no matches |
| GET /api/cases/{id} | Complete case detail excluding image bytes;404 missing |
| GET /api/documents/{id}/image | PNG/JPEG bytes from persisted current copy |
| POST /api/cases | {applicantReference,fullName,barangay,mobile,batchId};201 plus Location, Draft consumes no capacity |
| POST /api/cases/{id}/documents | {requirementId,fileName,content}; content is base64 bytes, maximum decoded2MB; replacement resets review |
| POST /api/cases/{id}/submit | No body;200 saved case;409 full/incomplete; repeat returns existing submission |
| POST /api/cases/{id}/review | {documentId,decision,remarks}; decision Accepted/Resubmit/Rejected; Staff |
| POST /api/cases/{id}/endorse | No body; all mandatory current documents Accepted; Staff |
| POST /api/cases/{id}/decision | {approved,remarks}; Official; denial frees capacity atomically |
| POST /api/cases/{id}/schedule | {date}; Staff; approved case, not before today |
| POST /api/cases/{id}/verify | {originalsMatch,notes,previousHoldResolved}; Releasing officer; mismatch records Held, match permits Released only under valid state/date/hold conditions |

Errors use {code,message}:400 invalid input;403 actor/role;404 missing;409 business/concurrency conflict;500 safe database/server or unknown commit outcome. A transport failure has no HTTP response. Mutation timeout/lost acknowledgement must be verified before retrying; the client performs no automatic retries. --initialize applies migrations/reference seed and exits. Normal serving never resets operational data.

Development-only test configuration: ETULONG_TEST_DATE injects a fixed clock in Development. ETULONG_FAULT_AFTER_WRITE=1 is enabled only in Development with a database name ending _test and raises after actual SaveChanges before commit. Never enable test fault configuration on demo/operational data.
