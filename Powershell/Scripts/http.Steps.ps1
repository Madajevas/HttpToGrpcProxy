Given 'service is running on (?<BaseAddress>https?:\/\/.*) base address' {
    param([string]$BaseAddress)
    $Script:baseAddress = $BaseAddress
}

Given 'proxy client is connected to (?<Address>.*)' {
    param([string]$Address)

    $Script:client = Get-Client -ProxyAddress $Address
}

Function Global:Invoke-RestMethodAsync {
    param($Uri, $Method, $Body)

    $invokeArgs = @{
        Uri = $Uri
        Method = $Method
    }

    if ($Method -match "(POST|PUT|PATCH)"){
        # TODO: other types
        $invokeArgs.Body = ConvertTo-Json $Body
    }

    Start-Job -ScriptBlock {
        $input.MoveNext() | Out-Null
        $args = $input.Current

        Invoke-RestMethod @args -Proxy http://localhost:8888
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

    $Script:interceptedRequestContext = Get-InterceptedRequest -Route $Path -Proxy $Script:client

    $Script:interceptedRequestContext.Route | Should -Be 'first'
}

And 'test responds with "(?<Body>.*)"' {
    param([string]$Body)
    $response = @{ Body = $Body; ContentType = "text/plain" }

    $Script:interceptedRequestContext.Respond($response)
}

Then 'response body is "(?<Body>.*)"' {
    param([string]$Body)
    $response = $global:responsePromise | Receive-Job -Wait

    $response | Should -Be $Body
}

When 'request (?<Method>(POST|PUT|PATCH)) (?<Path>/.*) is made with the following data and (?<ContentType>.*) content type' {
    param($Method, $Path, $ContentType, $Data)


}
