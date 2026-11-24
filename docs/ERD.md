# Updated relational design

All entities below correspond to Data/DeskDb.cs and its generated EF Core migration/model snapshot. PK/FK relationships use restrictive deletion. A case belongs to one applicant and one batch. ProgramRequirement is the program/checklist many-to-many junction.

```mermaid
erDiagram
 Programs ||--o{ AssistanceBatches : offers
 Programs ||--o{ ProgramRequirements : requires
 Requirements ||--o{ ProgramRequirements : defines
 Applicants ||--o{ Applications : submits
 AssistanceBatches ||--o{ Applications : receives
 Applications ||--o{ Documents : contains
 Requirements ||--o{ Documents : identifies
 Staff ||--o{ Documents : reviews
 Applications ||--o| Reservations : reserves
 AssistanceBatches ||--o{ Reservations : allocates
 Applications ||--o| Approvals : receives
 Staff ||--o{ Approvals : decides
 Applications ||--o{ Verifications : records
 Staff ||--o{ Verifications : verifies
 Programs {
  int Id PK
  string Name
 }
 Requirements {
  int Id PK
  string Name
 }
 ProgramRequirements {
  int ProgramId PK,FK
  int RequirementId PK,FK
  bool Mandatory
 }
 AssistanceBatches {
  int Id PK
  int ProgramId FK
  string Name
  int Capacity
  int ReservedCount
  bool IsOpen
 }
 Applicants {
  int Id PK
  string Reference UK
  string FullName
  string Barangay
  string Mobile
 }
 Applications {
  int Id PK
  int ApplicantId FK
  int BatchId FK
  string Status
  datetime CreatedAt
  datetime SubmittedAt
  date ClaimDate
 }
 Documents {
  int Id PK
  int CaseId FK
  int RequirementId FK
  bytes Content
  string FileName
  string ContentType
  string Decision
  string Remarks
  int ReviewedBy FK
  datetime ReviewedAt
 }
 Reservations {
  int Id PK
  int CaseId FK,UK
  int BatchId FK
  string State
  datetime CreatedAt
 }
 Staff {
  int Id PK
  string FullName
  string Role
 }
 Approvals {
  int Id PK
  int CaseId FK,UK
  int OfficialId FK
  string Decision
  string Remarks
  datetime DecidedAt
 }
 Verifications {
  int Id PK
  int CaseId FK
  int OfficerId FK
  string Result
  string Notes
  datetime RecordedAt
 }
```

Database uniqueness: Applicants.Reference; Applications(ApplicantId,BatchId); Documents(CaseId,RequirementId); Reservations.CaseId; Approvals.CaseId; a partial unique Verifications.CaseId WHERE Result='Released'. ProgramRequirement has a composite key. Capacity has a CHECK requiring0<=ReservedCount<=Capacity and Capacity>0. Case/document/reservation allowed-state checks are in the migration.

Application invariants additionally coordinate batch count and reservation state in transactions. A database CHECK on one row alone cannot enforce the cross-table count. ReservedCount includes Reserved and Awarded reservations; Released denial reservations retain history without occupying capacity. Application state and a matching reservation are created together at submission. Approved/released awards retain their allocated slot.

Midterm revisions: AssistanceBatch/SlotReservation make capacity explicit. Current image bytes replace client-local image_path, so different API clients retrieve the same copy. NotificationLog and advisory AI flags are deferred. No document revision history is claimed. Demo staff roles identify an actor; they do not authenticate a real user.
