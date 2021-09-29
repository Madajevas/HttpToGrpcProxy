Given 'service is running on (?<BaseAddress>https?:\/\/.*) base address' {
    param([string]$BaseAddress)
    $Script:baseAddress = $BaseAddress
}

Given 'proxy client is connected to (?<Address>.*)' {
    param([string]$Address)

    $Script:client = Get-Client -ProxyAddress $Address
}

Given 'http requests proxied through (?<Address>.*)' {
    param($Address)
    $Global:proxyAddress = $Address
}

Function Global:Invoke-RestMethodAsync {
    param($Uri, $Method, $Body, $ContentType)

    $invokeArgs = @{
        Uri = $Uri
        Method = $Method
        ContentType = $ContentType
        Proxy = $Global:proxyAddress
    }

    if ($Method -match "(POST|PUT|PATCH)") {
        # TODO: other types
        $invokeArgs.Body = ConvertTo-Json $Body
    }

    Start-Job -ScriptBlock {
        $input.MoveNext() | Out-Null
        $args = $input.Current

        Invoke-RestMethod @args
    } -InputObject $invokeArgs
}

Given 'request (?<Method>(GET|POST|PATCH|DELETE)) (?<Path>/.*) is made' {
    param([string]$Method, [string]$Path)
    $uri = $Script:baseAddress + $Path

    # TODO: maybe there is better way when usage of global
    $global:responsePromise = Invoke-RestMethodAsync -Uri $uri -Method $Method
}

When 'outgoing request to (?<Path>/.*) intercepted' {
    param([string]$Path)

    $Global:interceptedRequestContext = Get-InterceptedRequest -Route $Path -Proxy $Script:client

    $Global:interceptedRequestContext.Route | Should -Be $Path.Trim('/')
}

And 'test responds with "(?<Body>.*)"' {
    param([string]$Body)
    $response = @{ Body = $Body; ContentType = "text/plain" }

    $Global:interceptedRequestContext.Respond($response)
}

Then 'response body is "(?<Body>.*)"' {
    param([string]$Body)
    $response = $global:responsePromise | Receive-Job -Wait

    $response | Should -Be $Body
}

When 'request (?<Method>(POST|PUT|PATCH)) (?<Path>/.*) is made with the following data and (?<ContentType>.*) content type' {
    param($Method, $Path, $ContentType, $Table)
    # TODO: build url in single location
    $uri = $Script:baseAddress + $Path

    $global:responsePromise = Invoke-RestMethodAsync -Uri $uri -Method $Method -ContentType $ContentType -Body $Table
}

When 'requested method is (?<Method>(POST|PUT|PATCH))' {
    param($Method)

    $Global:interceptedRequestContext.Method | Should -Be $Method
}

When 'sent object is like' {
    param($Table)

    # TODO: other types
    $requestData = $Global:interceptedRequestContext.Body | ConvertFrom-Json

    $propertiesToAssert = Get-Member -InputObject $Table -MemberType Properties

    ForEach ($property in $Table.Keys)
    {
        $requestData.$property | Should -Be $Table.$property
    }
}

Function global:Debug-Object {
    param($InputObject)

    Write-Host ($InputObject | ConvertTo-Json)
}
