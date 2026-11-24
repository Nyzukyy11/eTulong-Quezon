using ETulong;
using Microsoft.EntityFrameworkCore;
namespace ETulongData;
public static class Seed {
 public static async Task Run(DeskDb d){if(await d.Programs.AnyAsync())return;await using var tx=await d.Database.BeginTransactionAsync();var program=new AidProgram{Name="Medical assistance - demonstration checklist"};d.Programs.Add(program);var req=new[]{new RequirementType{Name="Valid ID"},new RequirementType{Name="Barangay endorsement"},new RequirementType{Name="Medical certificate"}};d.Requirements.AddRange(req);d.Staff.AddRange(new StaffUser{FullName="Intake and review staff",Role="Staff"},new StaffUser{FullName="Approving official",Role="Official"},new StaffUser{FullName="Claim verification officer",Role="Releasing officer"});await d.SaveChangesAsync();d.ProgramRequirements.AddRange(req.Select(x=>new ProgramRequirement{ProgramId=program.Id,RequirementId=x.Id,Mandatory=true}));d.Batches.AddRange(new AssistanceBatch{ProgramId=program.Id,Name="Medical assistance - main batch",Capacity=5},new AssistanceBatch{ProgramId=program.Id,Name="Concurrency demonstration - last slot",Capacity=1});await d.SaveChangesAsync();await tx.CommitAsync();}
}
