using ETulong;
using System.Net.Http.Json;
using System.Text.Json;
namespace ETulongDesktop;
public sealed class DeskClient(HttpClient http) {
 static readonly JsonSerializerOptions Json=new(JsonSerializerDefaults.Web);
 public int ActorId{get;set;}=1;
 async Task<T> Send<T>(HttpMethod method,string path,object? body=null){try{using var request=new HttpRequestMessage(method,path);request.Headers.Add("X-Demo-Staff",ActorId.ToString());if(body is not null)request.Content=JsonContent.Create(body);using var response=await http.SendAsync(request);if(!response.IsSuccessStatusCode){var error=await response.Content.ReadFromJsonAsync<ApiError>(Json);throw new DeskException((int)response.StatusCode,error?.Code??"http",error?.Message??"The request failed. Refresh before continuing.");}return await response.Content.ReadFromJsonAsync<T>(Json)??throw new Exception("The service returned an empty response.");}catch(HttpRequestException){throw new Exception(method==HttpMethod.Get?"Cannot reach the eTulong API. Start the server and refresh. No HTTP response was received.":"Connection lost. The save outcome may be unknown. Refresh and verify the case before retrying.");}catch(TaskCanceledException){throw new Exception(method==HttpMethod.Get?"The service request timed out. Check connectivity and refresh.":"The request timed out. Verify the saved case before retrying; the outcome may be unknown.");}catch(JsonException){throw new Exception("The service returned unusable data. Restore the service and refresh.");}}
 record ApiError(string Code,string Message);
 public Task<ReferenceData> Reference()=>Send<ReferenceData>(HttpMethod.Get,"api/reference");
 public Task<CasePage> Cases(string search,string status,int page)=>Send<CasePage>(HttpMethod.Get,$"api/cases?search={Uri.EscapeDataString(search)}&status={Uri.EscapeDataString(status)}&page={page}&size=25");
 public Task<CaseDetail> Detail(int id)=>Send<CaseDetail>(HttpMethod.Get,$"api/cases/{id}");
 public Task<CaseDetail> Draft(DraftInput x)=>Send<CaseDetail>(HttpMethod.Post,"api/cases",x);
 public Task<CaseDetail> Action(int id,string action,object? body=null)=>Send<CaseDetail>(HttpMethod.Post,$"api/cases/{id}/{action}",body);
 public async Task<byte[]> Image(int id){try{return await http.GetByteArrayAsync($"api/documents/{id}/image");}catch(Exception ex)when(ex is HttpRequestException or TaskCanceledException){throw new Exception("Could not load this document image. Check API connectivity.");}}
}
