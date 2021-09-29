BeforeEachScenario {
    # docker compose -f ..\..\DockerComposeTests\docker-compose-services.yml -f ..\..\DockerComposeTests\docker-compose-proxy.yml build
    docker compose -f ..\..\DockerComposeTests\docker-compose-services.yml -f ..\..\DockerComposeTests\docker-compose-proxy.yml up -d | Out-Null

    While ($true) {
        $logs = docker compose -f ..\..\DockerComposeTests\docker-compose-services.yml -f ..\..\DockerComposeTests\docker-compose-proxy.yml logs proxy

        if ($logs -match 'Now listening on:') {
            break;
        }
    }
}

AfterEachScenario {
    docker compose -f ..\..\DockerComposeTests\docker-compose-services.yml -f ..\..\DockerComposeTests\docker-compose-proxy.yml down | Out-Null
}
