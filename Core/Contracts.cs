using System.Text.Json.Serialization;
namespace ETulong;
public record DraftInput([property:JsonRequired]string ApplicantReference,[property:JsonRequired]string FullName,[property:JsonRequired]string Barangay,string? Mobile,[property:JsonRequired]int BatchId);
public record DocumentInput([property:JsonRequired]int RequirementId,[property:JsonRequired]string FileName,[property:JsonRequired]byte[] Content);
public record ReviewInput([property:JsonRequired]int DocumentId,[property:JsonRequired]string Decision,string? Remarks);
public record DecisionInput([property:JsonRequired]bool Approved,[property:JsonRequired]string Remarks);
public record ScheduleInput([property:JsonRequired]DateOnly Date);
public record VerificationInput([property:JsonRequired]bool OriginalsMatch,[property:JsonRequired]string Notes,bool PreviousHoldResolved=false);
public record BatchInfo(int Id,string Program,string Name,int Capacity,int ReservedCount,bool IsOpen) {public int Available=>Capacity-ReservedCount;public override string ToString()=>$"{Name} | {Available} of {Capacity} slots available";}
public record RequirementInfo(int Id,string Name,bool Mandatory) {public override string ToString()=>Name+(Mandatory?" (required)":" (optional)");}
public record ReferenceData(StaffUser[] Staff,BatchInfo[] Batches);
public record CaseRow(int Id,string Reference,string Applicant,string Barangay,string Batch,string Status,DateOnly? ClaimDate) {public override string ToString()=>$"{Reference}  {Applicant}\n{Status} | {Batch}";}
public record CasePage(CaseRow[] Items,int Total,int Page,int PageSize);
public record DocumentInfo(int Id,int RequirementId,string Requirement,string FileName,string Decision,string? Remarks,string? Reviewer);
public record DecisionInfo(string Decision,string Official,string Remarks,DateTimeOffset At);
public record VerificationInfo(string Result,string Officer,string Notes,DateTimeOffset At);
public record CaseDetail(int Id,string Reference,Applicant Applicant,BatchInfo Batch,string Status,DateTimeOffset? SubmittedAt,DateOnly? ClaimDate,RequirementInfo[] Requirements,DocumentInfo[] Documents,string? ReservationState,DecisionInfo? Approval,VerificationInfo[] Verifications);
public record StoredImage(byte[] Content,string ContentType);
public interface IDeskStore {
 Task<ReferenceData> Reference();Task<StaffUser?> Staff(int id);Task<CasePage> Cases(string? search,string? status,int page,int size);Task<CaseDetail> Detail(int id);Task<StoredImage> Image(int documentId);
 Task<CaseDetail> Draft(DraftInput input,StaffUser actor);Task<CaseDetail> Attach(int id,DocumentInput input,StaffUser actor);Task<CaseDetail> Submit(int id,StaffUser actor);Task<CaseDetail> Review(int id,ReviewInput input,StaffUser actor);Task<CaseDetail> Endorse(int id,StaffUser actor);Task<CaseDetail> Decide(int id,DecisionInput input,StaffUser actor);Task<CaseDetail> Schedule(int id,ScheduleInput input,StaffUser actor);Task<CaseDetail> Verify(int id,VerificationInput input,StaffUser actor);
}
public sealed class DeskService(IDeskStore store) {
 async Task<StaffUser> Actor(int id,string role){var s=await store.Staff(id)??throw new DeskException(403,"actor","Select a valid demonstration operator.");if(s.Role!=role)throw new DeskException(403,"role",$"This operation requires the {role} role.");return s;}
 public Task<ReferenceData> Reference()=>store.Reference();public Task<CasePage> Cases(string? search=null,string? status=null,int page=1,int size=25)=>store.Cases(search,status,page,size);public Task<CaseDetail> Detail(int id)=>store.Detail(id);public Task<StoredImage> Image(int id)=>store.Image(id);
 public async Task<CaseDetail> Draft(DraftInput x,int actor)=>await store.Draft(x,await Actor(actor,"Staff"));
 public async Task<CaseDetail> Attach(int id,DocumentInput x,int actor)=>await store.Attach(id,x,await Actor(actor,"Staff"));
 public async Task<CaseDetail> Submit(int id,int actor)=>await store.Submit(id,await Actor(actor,"Staff"));
 public async Task<CaseDetail> Review(int id,ReviewInput x,int actor)=>await store.Review(id,x,await Actor(actor,"Staff"));
 public async Task<CaseDetail> Endorse(int id,int actor)=>await store.Endorse(id,await Actor(actor,"Staff"));
 public async Task<CaseDetail> Decide(int id,DecisionInput x,int actor)=>await store.Decide(id,x,await Actor(actor,"Official"));
 public async Task<CaseDetail> Schedule(int id,ScheduleInput x,int actor)=>await store.Schedule(id,x,await Actor(actor,"Staff"));
 public async Task<CaseDetail> Verify(int id,VerificationInput x,int actor)=>await store.Verify(id,x,await Actor(actor,"Releasing officer"));
}
public static class CaseRules {
 public static string Required(string? x,string field)=>string.IsNullOrWhiteSpace(x)?throw new DeskException(400,"validation",field+" is required."):x.Trim();
 public static void State(ApplicationCase c,params string[] allowed){if(!allowed.Contains(c.Status))throw new DeskException(409,"state",$"This operation is unavailable while the case is {c.Status}.");}
 public static void CompleteDocuments(ProgramRequirement[] requirements,SubmittedDocument[] documents,bool accepted){if(requirements.Where(x=>x.Mandatory).Any(x=>!documents.Any(d=>d.RequirementId==x.RequirementId&&(!accepted||d.Decision=="Accepted"))))throw new DeskException(409,"documents",accepted?"Every mandatory document must be accepted before endorsement.":"Attach every mandatory document before submitting. No slot was reserved.");}
 public static string ImageType(byte[]? content){if(content is null||content.Length==0||content.Length>2*1024*1024)throw new DeskException(400,"image","Choose a PNG or JPEG image no larger than 2 MB.");if(content.Length>8&&content.Take(8).SequenceEqual(new byte[]{137,80,78,71,13,10,26,10}))return "image/png";if(content.Length>3&&content[0]==255&&content[1]==216&&content[2]==255)return "image/jpeg";throw new DeskException(400,"image","Only PNG or JPEG document images are supported.");}
}
