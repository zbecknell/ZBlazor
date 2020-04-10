Write-Output "build: Build started"

Push-Location $PSScriptRoot

if (Test-Path .\artifacts) {
    Write-Output "build: Cleaning .\artifacts"
    Remove-Item .\artifacts -Force -Recurse
}

& dotnet restore --no-cache

$branch = @{ $true = $env:APPVEYOR_REPO_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$NULL -ne $env:APPVEYOR_REPO_BRANCH];
$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$NULL -ne $env:APPVEYOR_BUILD_NUMBER];
$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision" }[$branch -eq "master" -and $revision -ne "local"]
$commitHash = $(git rev-parse --short HEAD)
$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]

Write-Output "build: Package version suffix is $suffix"
Write-Output "build: Build version suffix is $buildSuffix"

Push-Location ./src

foreach ($src in ls) {
    Push-Location $src

    Write-Output "build: Packaging project in $src"

    & dotnet build -c Release --version-suffix=$buildSuffix

    if ($suffix) {
        & dotnet pack -c Release --include-source --no-build -o ..\..\artifacts --version-suffix=$suffix
    }
    else {
        & dotnet pack -c Release --include-source --no-build -o ..\..\artifacts
    }
    if ($LASTEXITCODE -ne 0) { exit 1 }

    Pop-Location
}

Pop-Location

# foreach ($test in ls test/*.Tests) {
#     Push-Location $test

#     Write-Output "build: Testing project in $test"

#     & dotnet test -c Release
#     if ($LASTEXITCODE -ne 0) { exit 3 }

#     Pop-Location
# }

Pop-Location
