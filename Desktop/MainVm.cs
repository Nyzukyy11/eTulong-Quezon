using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ETulong;
namespace ETulongDesktop;
public partial class MainVm(DeskClient client):ObservableObject {
 [ObservableProperty]bool busy;public bool Ready=>!Busy;partial void OnBusyChanged(bool value){OnPropertyChanged(nameof(Ready));ChangedPermissions();}
 [ObservableProperty]string message="Start the local API, then refresh to load eTulong.";
 [ObservableProperty]StaffUser[] operators=[];[ObservableProperty]StaffUser? selectedOperator;
 [ObservableProperty]BatchInfo[] batches=[];[ObservableProperty]BatchInfo? selectedBatch;
 [ObservableProperty]CaseRow[] cases=[];[ObservableProperty]CaseRow? selectedCase;
 [ObservableProperty]CaseDetail? detail;
 [ObservableProperty]string search="";[ObservableProperty]string statusFilter="All";
 [ObservableProperty]int page=1;[ObservableProperty]string pageLabel="No cases loaded";
 [ObservableProperty]string applicantReference="";[ObservableProperty]string fullName="";[ObservableProperty]string barangay="";[ObservableProperty]string? mobile;
 [ObservableProperty]RequirementInfo[] requirements=[];[ObservableProperty]RequirementInfo? selectedRequirement;
 [ObservableProperty]DocumentInfo[] documents=[];[ObservableProperty]DocumentInfo? selectedDocument;
 [ObservableProperty]Bitmap? preview;[ObservableProperty]string previewLabel="Select a saved document to preview it.";
 [ObservableProperty]string reviewDecision="Accepted";[ObservableProperty]string reviewRemarks="";
 [ObservableProperty]string decisionRemarks="";
 [ObservableProperty]string claimDate=new ManilaClock().Today.ToString("yyyy-MM-dd");
 [ObservableProperty]string verificationNotes="";[ObservableProperty]bool originalsMatch;[ObservableProperty]bool previousHoldResolved;
 [ObservableProperty]string history="No case selected.";
 [ObservableProperty]int workspaceTab;
 public string[] Statuses {get;}=["All","Draft","Submitted","NeedsResubmission","Endorsed","Approved","Denied","Scheduled","Held","Released"];
 public string[] ReviewChoices {get;}=["Accepted","Resubmit","Rejected"];
 public Func<Task<(string Name,byte[] Bytes)?>>? PickDocumentAsync{get;set;}
 public bool IsStaff=>Ready&&SelectedOperator?.Role=="Staff";public bool IsOfficial=>Ready&&SelectedOperator?.Role=="Official";public bool IsReleasingOfficer=>Ready&&SelectedOperator?.Role=="Releasing officer";
 public string CaseHeading=>Detail is null?"New assistance application":$"{Detail.Reference} - {Detail.Applicant.FullName}";
 public string CaseState=>Detail is null?"Create a draft, attach required documents, then submit to reserve a slot.":$"{Detail.Status} | Slot: {Detail.ReservationState??"not reserved"} | {Detail.Batch.Name}";
 public string CapacityLabel=>SelectedBatch is null?"Choose an assistance batch.":$"{SelectedBatch.Available} available / {SelectedBatch.Capacity} total slots";
 void ChangedPermissions(){OnPropertyChanged(nameof(IsStaff));OnPropertyChanged(nameof(IsOfficial));OnPropertyChanged(nameof(IsReleasingOfficer));}
 partial void OnSelectedOperatorChanged(StaffUser? value){client.ActorId=value?.Id??0;ChangedPermissions();}
 partial void OnSelectedBatchChanged(BatchInfo? value)=>OnPropertyChanged(nameof(CapacityLabel));
 partial void OnDetailChanged(CaseDetail? value){OnPropertyChanged(nameof(CaseHeading));OnPropertyChanged(nameof(CaseState));}
 partial void OnSelectedDocumentChanged(DocumentInfo? value){if(value is not null)LoadImageCommand.Execute(null);}
 async Task Run(Func<Task> work){if(Busy)return;Busy=true;Message="Working...";try{await work();}catch(Exception ex){Message=ex.Message;}finally{Busy=false;}}
 async Task References(){var oldOperator=SelectedOperator?.Id;var oldBatch=SelectedBatch?.Id;var data=await client.Reference();Operators=data.Staff;Batches=data.Batches;SelectedOperator=Operators.FirstOrDefault(x=>x.Id==oldOperator)??Operators.FirstOrDefault();SelectedBatch=Batches.FirstOrDefault(x=>x.Id==oldBatch)??Batches.FirstOrDefault();}
 async Task Rows(){var data=await client.Cases(Search,StatusFilter,Page);Cases=data.Items;PageLabel=$"Page {Page} | {data.Total} matching cases";}
 void Apply(CaseDetail c){var req=SelectedRequirement?.Id;Detail=c;ApplicantReference=c.Applicant.Reference;FullName=c.Applicant.FullName;Barangay=c.Applicant.Barangay;Mobile=c.Applicant.Mobile;SelectedBatch=Batches.FirstOrDefault(x=>x.Id==c.Batch.Id);Requirements=c.Requirements;SelectedRequirement=Requirements.FirstOrDefault(x=>x.Id==req)??Requirements.FirstOrDefault();SelectedDocument=null;Documents=c.Documents;Preview?.Dispose();Preview=null;PreviewLabel="Select a saved document, then choose Preview document.";ClaimDate=c.ClaimDate?.ToString("yyyy-MM-dd")??new ManilaClock().Today.ToString("yyyy-MM-dd");OriginalsMatch=false;PreviousHoldResolved=false;VerificationNotes="";History=$"Submitted: {c.SubmittedAt?.ToLocalTime().ToString("g")??"not submitted"}\nClaim date: {c.ClaimDate?.ToString("yyyy-MM-dd")??"not scheduled"}\n"+(c.Approval is null?"No official decision recorded.":$"{c.Approval.Decision} by {c.Approval.Official}: {c.Approval.Remarks}")+"\n"+string.Join("\n",c.Verifications.Select(x=>$"{x.At.ToLocalTime():g} | {x.Result} by {x.Officer}: {x.Notes}"));}
 int Id()=>Detail?.Id??throw new Exception("Open a saved case first.");
 async Task Save(string action,object? body=null){var c=await client.Action(Id(),action,body);await References();Apply(c);await Rows();Message=$"{c.Reference}: {c.Status}. Saved and refreshed.";}
 [RelayCommand]Task Refresh()=>Run(async()=>{var id=Detail?.Id;await References();await Rows();if(id is not null)Apply(await client.Detail(id.Value));Message="Connected. Case list and slot availability refreshed.";});
 [RelayCommand]Task Find()=>Run(async()=>{Page=1;await Rows();Message="Search results refreshed.";});
 [RelayCommand]Task Previous()=>Run(async()=>{if(Page>1)Page--;await Rows();Message="Case list refreshed.";});
 [RelayCommand]Task Next()=>Run(async()=>{if(Cases.Length==25)Page++;await Rows();Message="Case list refreshed.";});
 [RelayCommand]Task Open()=>Run(async()=>{Apply(await client.Detail(SelectedCase?.Id??throw new Exception("Select a case from the list.")));Message="Case loaded. Select Intake, Review or Claim to continue.";});
 [RelayCommand]void New(){if(Busy)return;Detail=null;SelectedCase=null;ApplicantReference="";FullName="";Barangay="";Mobile="";Requirements=[];Documents=[];SelectedDocument=null;Preview?.Dispose();Preview=null;ReviewRemarks="";DecisionRemarks="";History="No case selected.";WorkspaceTab=0;Message="Enter an applicant reference, name and barangay; select a batch and create a draft.";}
 [RelayCommand]Task CreateDraft()=>Run(async()=>{if(Detail is not null)throw new Exception("Choose New application before creating another draft.");var c=await client.Draft(new(ApplicantReference,FullName,Barangay,Mobile,SelectedBatch?.Id??0));await References();Apply(c);await Rows();Message=$"Draft {c.Reference} saved. Attach every required document before submission. No slot is reserved yet.";});
 [RelayCommand]Task Attach()=>Run(async()=>{var id=Id();var req=SelectedRequirement??throw new Exception("Select the document requirement.");if(PickDocumentAsync is null)throw new Exception("Document picker is unavailable.");var file=await PickDocumentAsync();if(file is null){Message="Document selection cancelled.";return;}var c=await client.Action(id,"documents",new DocumentInput(req.Id,file.Value.Name,file.Value.Bytes));Apply(c);Message="Document saved. A replacement always requires a new staff review.";});
 [RelayCommand]Task LoadImage()=>Run(async()=>{var doc=SelectedDocument??throw new Exception("Select a saved document.");var bytes=await client.Image(doc.Id);using var stream=new MemoryStream(bytes);var bitmap=new Bitmap(stream);Preview?.Dispose();Preview=bitmap;PreviewLabel=$"{doc.Requirement} | {doc.Decision} | {doc.FileName}";Message="Document preview loaded.";});
 [RelayCommand]Task Submit()=>Run(()=>Save("submit"));
 [RelayCommand]Task Review()=>Run(()=>Save("review",new ReviewInput(SelectedDocument?.Id??throw new Exception("Select the document to review."),ReviewDecision,ReviewRemarks)));
 [RelayCommand]Task Endorse()=>Run(()=>Save("endorse"));
 [RelayCommand]Task Approve()=>Run(()=>Save("decision",new DecisionInput(true,DecisionRemarks)));
 [RelayCommand]Task Deny()=>Run(()=>Save("decision",new DecisionInput(false,DecisionRemarks)));
 [RelayCommand]Task Schedule()=>Run(async()=>{if(!DateOnly.TryParseExact(ClaimDate,"yyyy-MM-dd",out var date))throw new Exception("Enter a claim date as YYYY-MM-DD.");await Save("schedule",new ScheduleInput(date));});
 [RelayCommand]Task Verify()=>Run(()=>Save("verify",new VerificationInput(OriginalsMatch,VerificationNotes,PreviousHoldResolved)));
}
