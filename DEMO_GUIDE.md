# eTulong demonstration and rehearsal

Target: approximately8–10minutes plus technical questions. Run the published application outside Visual Studio. Use fictional data. Open README.md first for PostgreSQL and launcher setup. Do not reset operational records to repeat a demo: use a new applicant reference or a newly created dedicated database.

## Before the presentation

- Obtain instructor approval of docs/SCOPE_RECONCILIATION.md and confirm final group membership/IDs.
- Verify the PostgreSQL server, API health and the published desktop. Keep required consoles available. Rehearse on the actual presentation machine.
- Prepare three files from demo-documents and a fresh applicant reference, for example DEMO-002 if DEMO-001 already exists. The checklist is a fictional demonstration policy pending validation.
- Open docs/evidence/TEST_RESULTS.txt and docs/ERD.md. Keep the full publish folders together. No new slide deck is required by the handout.

## Demonstration sequence

| Time | Action and explanation |
|---|---|
| 0:00–1:00 | Explain the original eTulong staff desk, the two final workflows and limited capacity. Identify who supplies information and who makes approval decisions. |
| 1:00–3:00 | As Intake and review staff, choose the main batch, enter applicant reference/name/barangay and Save new draft. Submit before attaching images to show rejection and unchanged capacity. Select each required document type and attach its corresponding fictional PNG. Scroll the intake pane if necessary. Submit successfully and show capacity decreasing once. |
| 3:00–5:00 | Open Review. Select a document to preview it. Save Accepted decisions for all mandatory documents. Scroll the review pane to its actions. Endorse the case. Select Approving official, enter required decision remarks and Approve. Point out that approval does not take a second slot. |
| 5:00–7:00 | Select Staff, open Claim, set today's YYYY-MM-DD date and Save claim date. Select Claim verification officer. With originals-match unchecked, enter a hold reason and record verification. Show Held. Then mark originals matching, explicitly resolve the previous hold, enter notes and record release. Explain that no actual money is transferred by this app. |
| 7:00–8:00 | Close/reopen the published desktop, refresh and open the same case. Show persisted approval, claim and release history. |
| 8:00–10:00 | Show the eight passing acceptance scenarios and the concurrency/rollback evidence. The last-slot test uses two HTTP clients and two database requests both waiting on the capacity row; exactly one succeeds. Explain a known failure rolling back related writes. |

All staff/official/releasing identities are demonstration actors; the app does not implement production login. Normal clicks and successful messages are evidence of user interaction, while tests/fresh database reads establish the concurrency and transactional claims.

## Optional live last-slot demonstration

The one-slot batch is distinct from the five-slot main batch. Use Scripts/Concurrency-Demo.ps1 against a running API and an unused one-slot demonstration batch. It creates two clearly fictional cases, attaches images and starts two independent HTTP submissions. Expected results: one200, one409, one saved reservation and zero remaining slots. It does not erase prior records, and refuses to run if the one-slot batch is already consumed. The stronger automated FP02 test additionally observes both waiting database requests before releasing its controlled lock.

## Questions to rehearse

- Why reserve on submission? The confirmed business rule is that a complete applicant submission takes one slot; a draft does not. Approval records the official's decision about an already reserved case.
- What if two users click together? The server transaction and conditional database update arbitrate the shared remaining capacity. Stale availability on a screen cannot override the database.
- Why does AWS alone not solve it? The same unsafe check-then-save logic can race in the cloud. Transactions/constraints remain necessary.
- What if the response is lost? The client reports unknown outcome and does not automatically retry. Refresh first; repeating submission for the same case reuses its reservation.
- What if a document is replaced? Current-copy replacement clears its previous review. Every mandatory current copy must be accepted before endorsement.
- How do workflows connect? The same application, document and reservation IDs pass from submission into review/decision and claim verification.
- What is deferred? Real chatbot, AI, SMS, production authentication, reservation expiry/waitlist and actual cash handling. AWS instructions are supplied; AWS deployment is not claimed.
- Why this ERD? ProgramRequirement models shared requirement types; AssistanceBatch/SlotReservation make capacity explicit; approval and verification rows retain decisions/events.

## Submission checklist

Source solution; reviewed/approved scope sheet; updated ERD; xUnit project and actual verification record; self-contained Windows x64 desktop/API folders; concise README. Keep acknowledgments and explain your own understanding. The package cannot provide instructor approval or confirm unknown group-member IDs.
