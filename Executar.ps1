Get-Command dotnet -ErrorAction Stop | Out-Null

$locationPushed = $false

try {
    Push-Location $PSScriptRoot
    $locationPushed = $true

    dotnet run --project (Join-Path $PSScriptRoot "CrudCarros.csproj")
    if ($LASTEXITCODE -ne 0) {
        throw "A aplicação terminou com código de saída $LASTEXITCODE."
    }
}
finally {
    if ($locationPushed) {
        Pop-Location
    }
}
