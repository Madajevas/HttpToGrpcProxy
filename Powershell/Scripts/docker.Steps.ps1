BeforeEachScenario {
    return;
    $location = Get-Location

    try {
        # TODO: normal location
        cd ../..
        docker build -f ./HttpToGrpcProxy/Dockerfile -t proxy .
    } finally {
        Set-Location $location
    }

    docker run -d -p 5000:5000 -p 6000:6000 --name proxy proxy

    While ($true) {
        $logs = docker container logs proxy

        if ($logs -match 'Now listening on:') {
            break;
        }
    }
}

AfterEachScenario {
    docker container rm -f proxy
}
