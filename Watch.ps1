param (
    [string] $ServerOrWasm = "wasm"
)

Write-Host "Executing 'dotnet watch' with sample project Blazor$ServerOrWasm"

If ($IsMacOS -eq $true)
{
    dotnet watch --project "Samples/Blazor$ServerOrWasm/Blazor$ServerOrWasm.csproj" run
}
Else
{
    dotnet watch --project ".\Samples\Blazor$ServerOrWasm\Blazor$ServerOrWasm.csproj" run
}
