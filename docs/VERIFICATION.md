# Verification and release record

Date: 2026-11-13, Asia/Manila. Environment: Windows x64, .NET SDK8.0.428/runtime8.0.34, Avalonia11.3.9, EF Core/Npgsql8.0.11, PostgreSQL16.15, xUnit2.9.3 and VSTest adapter3.1.4. Local PostgreSQL and fictional data only. No AWS execution is claimed.

## Automated acceptance

Initial completed run:8 passed,0 failed,0 skipped. Final completed run after extending FP01 with actual MainVm attachment/cancellation verification: **8 passed,0 failed,0 skipped**, VSTest duration3minutes8seconds. Raw evidence: evidence/initial.trx and final.trx. TEST_RESULTS.txt/results.json provide the matching test names. ACCEPTANCE_SCENARIOS.md maps the eight requirements to actual methods and assertions.

Each scenario migrates/seeds an independent owned PostgreSQL database and starts the actual Kestrel API. HTTP status/body claims use real routing and JSON handling. Fresh contexts/connections check saved state. No SQLite/InMemory replacement is used for PostgreSQL integrity claims. The only controlled doubles are the explicitly scoped client transport/acknowledgement-loss handler and file-picker callbacks; actual state claims still use real PostgreSQL.

FP02 deliberately holds the batch row lock, starts two independent HTTP clients, observes both backend capacity updates waiting in pg_stat_activity, then releases the gate. Assertions require exactly one200 and one409, one reservation, one Submitted case, one Draft case and capacity1. A duplicate reservation inserted directly into PostgreSQL must fail with SQLSTATE23505. This is evidence of genuine contention, not sequential calls labelled concurrent.

FP06 injects a fault after actual SaveChanges but before commit on an owned _test database. Submission failure must roll back reservation/counter/status together; release failure must roll back verification/status together. Fresh independent state is identical afterward. FP07 kills the original API process and starts another, verifies all case values/images/history and reruns safe initialization without resetting operations. FP08 actually commits a submission, then discards its acknowledgement: the client reports uncertainty, makes one attempt, reads fresh committed state and safely reuses the reservation on an explicit retry.

## Investigation and correction record

| Finding | Diagnosis and action | Retest/evidence |
|---|---|---|
| Initial test compilation failed: Seed was not in scope | Added the missing ETulongData namespace import to AcceptanceTests. No business assertion or expected result changed. | Both complete eight-scenario runs passed afterward. |
| Native image-picker automation could open/cancel the dialog but could not reliably target its filename control | This was a computer-control limitation, not evidence of a successful native file selection. Cancelled safely; used an independent HTTP client for the manual-session image uploads. Added real MainVm AttachCommand/cancellation checks with a controlled picker callback and actual HTTP/PostgreSQL persistence in FP01. | Native picker open/cancel observed; attachment workflow tested; actual persisted PNG preview observed. A successful physical filename selection is not claimed. |
| First normal-launcher test returned while the execution environment disposed its background children | Changed launcher to wait for the desktop process. It now remains attached for the demo session. | Launcher reinitialized without erasing data, started the published API/desktop, and the reopened desktop displayed the same Released case/history. |
| Final extra screenshot attempts reported 'foreground window did not report a process id' | Rebound the exact returned application window and inspected its current accessibility state. Stopped further desktop inputs after recovery did not permit another screenshot. | Restarted case/history was visibly inspected before this capture limitation. No missing screenshot was fabricated. |

No failure was hidden by skipping a test or reducing expected behavior. These results establish the exercised scenarios, not total correctness or production readiness.

## Observed published release check

Method: agent-operated Windows desktop controls and accessibility/screenshots, combined with independent HTTP readback. These are not claimed as actions performed by a human student. Rehearse the steps personally before the presentation.

Launched publish/Api/Api.exe and publish/Desktop/eTulong.Desktop.exe outside Visual Studio, from a different working directory. Desktop startup was also checked with DOTNET_ROOT pointing to an empty location and multilevel lookup disabled. Both publish folders include their runtime/native assets; hashes are recorded in published-files-sha256.json. A physically separate machine without any installed runtime was unavailable, so that stronger machine-environment check is not claimed.

Observed: created a fictional draft through the desktop; submission before required attachments displayed a clear rejection and unchanged capacity. The native picker opened and cancellation recovered safely. Fictional PNGs were attached through an independent HTTP client, refreshed in the desktop and previewed from persisted image bytes. Submitted through the desktop: case became Submitted, reservation Reserved, main-batch availability changed5to4. Staff review of the ID was saved through the UI; the other two reviews were supplied by the independent client. The desktop refreshed, endorsed the case, selected the official and recorded approval. Approval changed Reserved to Awarded while capacity stayed4available. Staff scheduled today's claim. The releasing officer recorded a mismatch hold, then explicit hold resolution and matching originals, producing Released with both events retained.

Stopped the owned API and refreshed: the desktop showed 'Cannot reach the eTulong API... No HTTP response was received' while retaining the previously displayed case/history. Closed the desktop. Ran Start-eTulong.cmd: initialization reported an up-to-date schema without erasing cases; the launcher opened both published components. The reopened desktop retrieved ET-000001/Ana Demo as Released with the same approval, claim date, Held and Released verification events. Accessibility state and earlier release screenshots support this observation. The final additional screenshot failed as described above.

Evidence screenshots/text:01-missing-documents;02-submitted-slot-reserved;03-held-before-release;04-released-history;05-api-unavailable. Asynchronous accessibility and screenshot capture may observe slightly different frames; no image was edited to fabricate state. independent-document-attachment.json identifies the external upload method; manual-readback.json records final independent HTTP/PostgreSQL readback.

## Live concurrency demonstration

Executed Scripts/Concurrency-Demo.ps1 against the published API and the separate one-slot batch. Case2 returned409; case3 returned200. The script confirmed one reservation and zero remaining capacity. The structured observed-result record is live-concurrency-demo.json; the stronger lock-gated proof is FP02 in final.trx. This script consumes the demonstration slot and refuses to erase/reset records on repeat use.

## Scope and delivery limits

Local classroom prototype only. Demo roles do not authenticate real users. Medical checklist and documents are fictional fixtures. Real chatbot/AI/SMS, payments, automatic expiry/waitlist and public AWS hosting are deferred. Cloud deployment instructions are supplied separately. The scope reconciliation is completed for review but instructor approval and other final-member IDs have not been supplied. No approval, official policy validation, human test observation, AWS deployment or LMS submission is fabricated.
