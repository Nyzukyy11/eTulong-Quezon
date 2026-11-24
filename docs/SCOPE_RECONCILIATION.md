# ITSD81 Final Project Scope Reconciliation

Scope version: eTulong-Final-1.0. Prepared for instructor review, not marked approved.

## A. Original Midterm baseline

| Item | Entry |
|---|---|
| Project / section | eTulong-Quezon: Staff Validation and Release Desk; BSIT3C |
| Actual source | D:/Codes/Desktop/eTulong_Midterm_Shortened.pptx; unchanged copy and hash in docs/source |
| Original listed members | Francis Roy Veranda; Nicolle Jan Bustamante; Kamil Cairbre A. Cloma. These are Midterm names, not a claim of confirmed final participation. |
| Confirmed submitting identity | Nicolle Jan P. Bustamante, 2024303917. Other final member names/IDs: ____________________. |
| Problem / users | Office staff manually review assistance documents while applicants repeatedly inquire about requirements/status. The user additionally identified simultaneous competition for limited assistance slots. |
| Desktop boundary | Office case queue, document review, official decision, claim scheduling, verification and release record. Cash movement and eligibility-policy judgment remain external. |
| Original process | Receive case; inspect advisory flags/photos; decide per document; endorse; record official approval/denial; schedule claim; verify originals and record release/hold. |
| Original architecture / model | Avalonia MVVM; application services and repository/gateway interfaces; EF Core/PostgreSQL; Applicant, Application, ProgramRequirement, SubmittedDocument, Approval, ClaimVerification, StaffUser and NotificationLog. |
| Original technical decision / risk | Shared PostgreSQL on office LAN. Interrupted external intake could duplicate or partially save a case; SMS and intake depend on connectivity. |

## B. Final workflow1: submit an application and reserve assistance capacity

Actor: intake staff recording an applicant's submission on their behalf. Purpose: accept a complete document submission and reserve exactly one available batch slot.

Trigger/preconditions: staff chooses an open assistance batch and starts a new case. Required inputs: stable applicant reference, full name, barangay, optional mobile, batch and current required document images. The checklist is demonstration reference data pending office/instructor validation.

Success: validate and save a draft/applicant; attach the required PNG/JPEG images; submit; lock the case; conditionally reserve available batch capacity; save one reservation and Submitted timestamp/status in the SAME transaction; display the result and refreshed capacity. The applicant reference may have only one case per batch.

Rejections/failures: blank required inputs, invalid/oversized image, missing mandatory document, closed/full batch, duplicate applicant case, database write failure. Drafts consume no slots. Competing submissions for the last slot produce one success and one conflict. Retrying the same submitted case returns its existing result without consuming another slot. A known pre-commit failure leaves case/capacity/reservation unchanged.

Observable outcome: a persisted Submitted case, exactly one Reserved row and consistent remaining capacity, retrievable after restart.

## C. Final workflow2: review and process the submitted case through decision and claim

Actors: review staff, official and releasing officer. Purpose: process the reserved case to a recorded decision and, when approved, a verified release or hold.

Success: retrieve workflow1's case/images; record each staff document decision; accept every mandatory current copy; endorse; official records a human approval decision; staff schedules the claim; on or after the claim date the releasing officer confirms matching originals and records release. The release event and case status commit together. Approval changes Reserved to Awarded without taking a second slot.

Alternatives: a document needing replacement blocks endorsement. Replacement resets that document to Pending and clears its reviewer until re-reviewed. An official may deny an endorsed case, atomically recording denial and returning reserved capacity. A mismatch records Held with a reason and blocks release. A held case requires explicit resolution plus matching originals before release. The application rejects wrong roles, premature claim dates, missing reasons and repeated release.

Observable outcome: persisted document reviews, official decision, claim date and verification/release history associated with the same applicant/case. Denied cases cannot schedule/release. Released awards continue consuming assistance capacity.

## D. Relationship

Workflow1 establishes the applicant, case, current document copies, batch association and reservation. Workflow2 retrieves and updates those same records. Stable foreign keys preserve this relationship; moving between screens alone is not the relationship. Incorrect references or duplicated reservations would affect the wrong applicant or overbook capacity, so the service and database enforce association/uniqueness and atomic updates.

## E. Data origin and persistence

| Information | Source / entry | Local persistence and integrity |
|---|---|---|
| Applicant/case details | Staff enters applicant-provided information; fictional in demo | Applicants and Applications; unique applicant reference and unique applicant/batch case |
| Program/checklist/batch | Seeded demonstration reference values | Programs, Requirements, ProgramRequirements, AssistanceBatches; PK/FK and capacity checks |
| Document images | Staff selects externally obtained PNG/JPEG files; fictional fixtures | PostgreSQL bytea, current copy per case/requirement; replacement resets review |
| Reservation | Application-managed at SUBMISSION, explicitly confirmed by user | Unique case reservation plus guarded capacity update in one transaction |
| Reviews and decisions | Human staff/official entries; no automatic eligibility decision | Reviewer/official foreign keys, timestamps, state/role rules |
| Claim/release/hold | Releasing officer's original-document check | Related verification events and atomic application status; unique release per case |

No seeded operational applications replace either workflow. AWS is a future hosting option, not an externally authoritative source of eligibility or slot decisions.

## F. Architecture and ERD revisions

Views/ViewModels: one MainWindow/MainVm with Intake, Review and Claim workspaces, case queue and asynchronous commands. DeskClient is HTTP-only. DeskService applies role checks; CaseRules defines input/state/document policies. DeskStore uses fresh PostgreSQL contexts and those policies inside transactions. API composes dependencies through DI.

Retain the Midterm's meaningful entities and program-requirement junction. Add AssistanceBatch and SlotReservation to model limited capacity. Use PostgreSQL image bytes instead of workstation image paths so API clients retrieve the same document. NotificationLog, real SMS and AI flags are deferred. Current-document replacement remains simplified without historical image revisions. See ERD.md and generated EF migration/model snapshot.

## G. Advanced capability

Selected option: **A, transactional business processing**. At submission the user initiates an operation coordinating available capacity, a reservation and case state. It must produce one consistent outcome, including under concurrent requests. Denial returns capacity atomically; release saves a verification event and state together. Controlled post-write faults demonstrate rollback through independent PostgreSQL reads. FP02 proves actual concurrent HTTP requests both waiting at the database capacity row, with exactly one success after the gate is released. No additional advanced feature is assumed necessary.

## H. Proposed scope boundaries

Mandatory: both workflows, submission-time reservation/concurrency protection, review/role/state rules, image preview, durable PostgreSQL records, transactions, meaningful tests, migrations and published Windows x64 app/API.

Supporting: search/status filtering with bounded25-row pages, demonstration operators, one demonstration medical checklist, two batches (capacity5 and capacity1), documented API, fictional sample images and demo instructions.

Deferred: applicant chatbot/mobile interface, real SMS, AI analysis, document revision archive, automatic reservation expiry/waitlist, production account management and live AWS deployment. A server-side demo role selector is not claimed as secure authentication.

Excluded Capstone work: real payments/cash transfer, automated eligibility judgment, full public assistance-service operation, institutional deployment and original research validation.

Important revisions for review: manual staff intake replaces chatbot import for this final mini-system. User confirmed reservation at submission. Denial returns capacity as an explicit proposed policy. Local PostgreSQL delivery first, AWS instructions only, as requested. Original LAN-oriented storage is retained locally; cloud deployment would require internet and updated operational assumptions.

## I. Acceptance scenarios

Eight scenarios, all implemented with xUnit, are fully specified in ACCEPTANCE_SCENARIOS.md: FP01 intake/submission/retry; FP02 concurrent last slot; FP03 review/replacement/endorsement; FP04 decision/denial capacity; FP05 claim hold/release; FP06 rollback; FP07 persistence/restart; FP08 protocol/transport/uncertain acknowledgement. Separate observed release checks are in VERIFICATION.md.

## J. Instructor approval

Decision: ____________________

Required revisions: __________________________________________________________

Approved scope version: ____________________  Approval date: ____________________

Instructor acknowledgment / documented approval reference: ____________________

Approval has not been supplied or invented. This completed sheet is ready for instructor review before implementation assessment.
