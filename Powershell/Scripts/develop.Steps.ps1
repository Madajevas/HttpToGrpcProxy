BeforeEachScenario {
    $location = Get-Location
    try {
        Set-Location ../HttpToGrpcProxy.Powershell
        dotnet publish
        Import-Module ./bin/Debug/net5.0/publish/HttpToGrpcProxy.Powershell.dll
    } finally {
        Set-Location $location
    }
}

AfterEachScenario {
    Remove-Module HttpToGrpcProxy.Powershell
}
