Given 'service is running on (?<BaseAddress>https?:\/\/.*) base address' {
    param([string]$BaseAddress)
    $Script:baseAddress = $BaseAddress
}

Given 'proxy client is connected to (?<Address>.*)' {
    param([string]$Address)

    $Script:client = Get-Client -ProxyAddress $Address
}

Given 'request (?<Method>(GET|POST|PATCH|DELETE)) (?<Path>/.*) is made' {
    param([string]$Method, [string]$Path)
    $uri = $Script:baseAddress + $Path

    $global:responsePromise = Start-Job `
        -ScriptBlock {
            $url =  $input.Uri
            $method = "GET"

            Invoke-RestMethod `
                -Method $method `
                -Uri $url
        } `
        -InputObject @{ Uri = $uri; Method = $Method }
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
