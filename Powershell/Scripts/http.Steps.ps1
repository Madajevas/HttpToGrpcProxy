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

    # $a = $Script:responsePromise | Receive-Job -Wait
    # Write-Host $a
    # $Script:responsePromise | Remove-Job -Force
    # Start-Sleep -Seconds 5
}

When 'outgoing request to (?<Path>/.*) intercepted' {
    param([string]$Path)

    $Script:interceptedRequestContext = Get-InterceptedRequest -Route $Path -Proxy $Script:client

    $Script:interceptedRequestContext.Route | Should -Be 'first'
}

And 'respond with' {
    $response = @{ Body = "responding from unit test"; ContentType = "text/plain" }

    $Script:interceptedRequestContext.Respond($response)
}

Then 'response status code is (?<StatusCode>\d+)' {
    param([int]$StatusCode)
    Write-Host "*****************************"
    $response = $global:responsePromise | Receive-Job -Wait
    Write-Host (ConvertTo-Json $response )

    $response | Should -Be "responding from unit test"
}
