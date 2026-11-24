param([string]$Api='http://127.0.0.1:5291/')
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Net.Http
$headers=@{'X-Demo-Staff'='1'}
$reference=Invoke-RestMethod -Uri ($Api+'api/reference')
$batch=$reference.batches | Where-Object { $_.name -like 'Concurrency*' } | Select-Object -First 1
if(-not $batch -or $batch.available -ne 1){throw 'This demonstration needs an unconsumed one-slot batch. Use a new dedicated demo database; existing records are never erased.'}
$fixture=Join-Path $PSScriptRoot '..\demo-documents\01-valid-id.png'
$content=[Convert]::ToBase64String([IO.File]::ReadAllBytes((Resolve-Path -LiteralPath $fixture)))
$ids=@()
$prefix='RACE-'+[Guid]::NewGuid().ToString('N').Substring(0,8)
foreach($suffix in @('A','B')){
 $body=@{applicantReference=$prefix+'-'+$suffix;fullName='Fictional contender '+$suffix;barangay='Demo Barangay';mobile=$null;batchId=$batch.id}|ConvertTo-Json
 $case=Invoke-RestMethod -Method Post -Uri ($Api+'api/cases') -Headers $headers -ContentType 'application/json' -Body $body
 foreach($req in $case.requirements){
  $doc=@{requirementId=$req.id;fileName='concurrency-fixture.png';content=$content}|ConvertTo-Json
  $null=Invoke-RestMethod -Method Post -Uri ($Api+'api/cases/'+$case.id+'/documents') -Headers $headers -ContentType 'application/json' -Body $doc
 }
 $ids+= $case.id
}
$first=New-Object System.Net.Http.HttpClient
$second=New-Object System.Net.Http.HttpClient
try{
 $first.DefaultRequestHeaders.Add('X-Demo-Staff','1')
 $second.DefaultRequestHeaders.Add('X-Demo-Staff','1')
 $a=$first.PostAsync($Api+'api/cases/'+$ids[0]+'/submit',[System.Net.Http.HttpContent]$null)
 $b=$second.PostAsync($Api+'api/cases/'+$ids[1]+'/submit',[System.Net.Http.HttpContent]$null)
 [Threading.Tasks.Task]::WaitAll([Threading.Tasks.Task[]]@($a,$b))
 $codes=@([int]$a.Result.StatusCode,[int]$b.Result.StatusCode)|Sort-Object
 Write-Host ('Case '+$ids[0]+': HTTP '+[int]$a.Result.StatusCode)
 Write-Host ('Case '+$ids[1]+': HTTP '+[int]$b.Result.StatusCode)
 if(($codes -join ',') -ne '200,409'){throw 'Expected exactly one successful submission.'}
 $updated=(Invoke-RestMethod -Uri ($Api+'api/reference')).batches|Where-Object id -eq $batch.id
 if($updated.reservedCount -ne 1 -or $updated.available -ne 0){throw 'Unexpected persisted capacity.'}
 Write-Host 'PASS: two requests, one reservation, zero remaining slots. Refresh the desktop to inspect both cases.' -ForegroundColor Green
 $a.Result.Dispose()
 $b.Result.Dispose()
}finally{
 $first.Dispose()
 $second.Dispose()
}
