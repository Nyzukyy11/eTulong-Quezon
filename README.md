# eTulong-Quezon: Staff Validation and Release Desk

Prepared for Nicolle Jan P. Bustamante, 2024303917, BSIT 3C, using the actual eTulong Midterm proposal. Final group membership and instructor scope approval need confirmation; see docs/SCOPE_RECONCILIATION.md. This is a classroom prototype using fictional data and demonstration operators.

## Quick start on this laptop

1. Start the existing local PostgreSQL server. Start-Local-Postgres.cmd locates the portable tools already in this workspace. Keep its console open. If this cluster is already listening on port55432, use the running instance.
2. The dedicated etulong_demo_1113 database was created during verification. Double-click Start-eTulong.cmd. It applies migrations/reference seeds, starts the published API and opens the published desktop. Existing cases are preserved.
3. Select the appropriate demonstration operator: Intake and review staff, Approving official, or Claim verification officer. These identities have server-side role checks but are not production authentication.
4. Follow DEMO_GUIDE.md. Fictional PNG documents are in demo-documents. Slots are reserved at SUBMISSION, not approval. Drafts consume no capacity. Denial returns capacity; approved/released awards continue consuming a slot.

Both published applications include their Windows x64 .NET runtime. PostgreSQL remains a separate dependency. The local database helper is specific to this workspace and does not install a database server on another machine.

## On another Windows x64 computer

Extract the entire ZIP. Keep publish/Api and publish/Desktop intact. Start PostgreSQL16 or newer and create a new empty database:

```powershell
createdb -h localhost -p 5432 -U postgres etulong_demo
```

Copy settings.example.cmd to settings.local.cmd. Set ETULONG_DB to your connection, for example Host=localhost;Port=5432;Database=etulong_demo;Username=YOUR_USER;Password=YOUR_PASSWORD. Keep credentials outside submitted files. The launcher uses loopback API port5291. It uses Windows PowerShell without changing execution policy.

To initialize and run components separately:

```powershell
$env:ETULONG_DB='Host=localhost;Database=etulong_demo;Username=YOUR_USER;Password=YOUR_PASSWORD'
./publish/Api/Api.exe --initialize
./publish/Api/Api.exe --urls http://127.0.0.1:5291
# Separate terminal or Explorer:
./publish/Desktop/eTulong.Desktop.exe
```

Desktop reads appsettings.json beside its executable, overridden by ETULONG_API. Only the API receives database credentials. Dates use Asia/Manila. Normal startup never truncates operational records.

## Source builds and tests

Install the .NET8 SDK. PostgreSQL16 was used for verification.

```powershell
dotnet restore eTulong.sln
dotnet build eTulong.sln --no-restore -m:1
$env:ETULONG_TEST_ADMIN='Host=localhost;Port=5432;Database=postgres;Username=YOUR_DISPOSABLE_TEST_USER;Password=YOUR_PASSWORD;Pooling=false'
dotnet test Tests/Tests.csproj --no-restore -m:1 --logger 'trx;LogFileName=acceptance.trx' --results-directory TestResults
dotnet publish Desktop/Desktop.csproj -c Release -r win-x64 --self-contained true -o publish/Desktop -m:1
dotnet publish Api/Api.csproj -c Release -r win-x64 --self-contained true -o publish/Api -m:1
```

Tests require a local PostgreSQL role allowed to create/drop disposable databases. Default test address is loopback55432, postgres, no password. ETULONG_TEST_ADMIN overrides this; ETULONG_DOTNET can select a specific .NET host. Each test creates an owned etulong_GUID_test database, applies the real migrations, starts a real API process, checks independent saved state and drops only that owned database. No AWS dependency. Fixed test date: 2030-01-15. Eight meaningful scenarios are implemented in Tests/AcceptanceTests.cs.

Data/Migrations contains the EF Core migration and model snapshot. Running the API with --initialize safely migrates and seeds reference data, never operational applications.

## Architecture and integrity

Core defines entities, DTOs, business policies, persistence contracts and the role-checking application service. Data implements EF Core/PostgreSQL storage and atomic operations using those policies. Api composes dependencies and exposes HTTP. Desktop uses asynchronous HttpClient and MVVM, with no database reference. Generic MVVM, error-handling and PostgreSQL patterns were reused from laboratory work; the domain and workflows are eTulong-specific.

Submission locks the current case, conditionally increments available batch capacity, creates its reservation and sets Submitted inside one transaction. Competing updates wait and recheck capacity. A case has at most one reservation; an applicant reference has at most one case per batch. Repeating submission returns the existing result. Failure rolls back related changes. Transaction order determines allocation; the app does not guarantee earliest-click ordering.

Images must exist at submission and mandatory images must be accepted at endorsement. Replacement resets review. Decisions require an endorsed case and the Official role. Scheduling requires approval. Release requires an arrived claim date, matching originals, an approved decision and explicit resolution of any previous hold. A release record and case status are saved together. Actual eligibility judgment remains with the official; cash movement remains outside the app.

## Required documents and limitations

- DEMO_GUIDE.md: rehearsal and technical explanations.
- docs/SCOPE_RECONCILIATION.md: completed proposal for instructor review; approval is not fabricated.
- docs/ERD.md: updated entity relationships and Mermaid diagram.
- docs/ACCEPTANCE_SCENARIOS.md and VERIFICATION.md: coverage and verification evidence.
- docs/AWS_DEPLOYMENT.md: optional deployment instructions, not a claim of AWS deployment.
- docs/API_REFERENCE.md: routes and payloads.

The seeded medical requirements are a demonstration checklist, not a validated official policy. PNG/JPEG attachments are limited to2MB and stored in PostgreSQL. This scope excludes OCR/AI analysis, a chatbot/mobile applicant client, real SMS, document revision archives, automated reservation expiry, waitlists and cash transfers. Current-copy replacement resets the staff review. Reservations persist until denial or an award consumes the slot.

The demonstration-operator header is not secure authentication. Use fictional data on loopback or a controlled private demonstration network. A public/production deployment requires authenticated identities, authorization, maintained supported runtime/dependencies and the data-protection work described in the AWS guide. This is not an official assistance service.

AI assistance: OpenAI Codex contributed substantially to implementation, documentation and verification. The student/group must understand the work, acknowledge reused sources and comply with institutional policy. Nothing was submitted to the LMS or deployed to AWS by this assistant.
