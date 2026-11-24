namespace ETulong;
public sealed class AidProgram {public int Id{get;set;} public string Name{get;set;}="";}
public sealed class RequirementType {public int Id{get;set;} public string Name{get;set;}="";}
public sealed class ProgramRequirement {public int ProgramId{get;set;} public int RequirementId{get;set;} public bool Mandatory{get;set;}=true;}
public sealed class AssistanceBatch {public int Id{get;set;} public int ProgramId{get;set;} public string Name{get;set;}=""; public int Capacity{get;set;} public int ReservedCount{get;set;} public bool IsOpen{get;set;}=true;}
public sealed class Applicant {public int Id{get;set;} public string Reference{get;set;}=""; public string FullName{get;set;}=""; public string Barangay{get;set;}=""; public string? Mobile{get;set;}}
public sealed class StaffUser {public int Id{get;set;} public string FullName{get;set;}=""; public string Role{get;set;}=""; public override string ToString()=>$"{FullName} ({Role})";}
public sealed class ApplicationCase {public int Id{get;set;} public int ApplicantId{get;set;} public int BatchId{get;set;} public string Status{get;set;}="Draft"; public DateTimeOffset CreatedAt{get;set;} public DateTimeOffset? SubmittedAt{get;set;} public DateOnly? ClaimDate{get;set;}}
public sealed class SubmittedDocument {public int Id{get;set;} public int CaseId{get;set;} public int RequirementId{get;set;} public byte[] Content{get;set;}=[]; public string FileName{get;set;}=""; public string ContentType{get;set;}=""; public string Decision{get;set;}="Pending"; public string? Remarks{get;set;} public int? ReviewedBy{get;set;} public DateTimeOffset? ReviewedAt{get;set;}}
public sealed class SlotReservation {public int Id{get;set;} public int CaseId{get;set;} public int BatchId{get;set;} public string State{get;set;}="Reserved"; public DateTimeOffset CreatedAt{get;set;}}
public sealed class Approval {public int Id{get;set;} public int CaseId{get;set;} public int OfficialId{get;set;} public string Decision{get;set;}=""; public string Remarks{get;set;}=""; public DateTimeOffset DecidedAt{get;set;}}
public sealed class ClaimVerification {public int Id{get;set;} public int CaseId{get;set;} public int OfficerId{get;set;} public string Result{get;set;}=""; public string Notes{get;set;}=""; public DateTimeOffset RecordedAt{get;set;}}
public sealed class DeskException(int status,string code,string message):Exception(message) {public int Status{get;}=status;public string Code{get;}=code;}
public interface IClock {DateTimeOffset Now{get;} DateOnly Today{get;}}
public sealed class ManilaClock:IClock {public DateTimeOffset Now=>DateTimeOffset.UtcNow;public DateOnly Today=>DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(Now,"Asia/Manila").DateTime);}
