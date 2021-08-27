# Http To Grpc Proxy
Solution intended to intercept communication with external services in integration tests.

## Problem
Predictable and isolated integration testing of application like in diagram is complicated.
```plantuml

package "External_World" {
    [external.service.one]
    [external.service.two]
}

package "Microservices" {
    [Front] -u-> [Intermediate]: grpc
    [Intermediate] -u-> [Edge1]: mq
    [Intermediate] -u-> [Edge2]: mq

    [Edge1] -u-> [external.service.one]: http
    [Edge2] -u-> [external.service.two]: http
}

actor User
User -u-> [Front]: interacts

note right of Microservices
A set of microservices that communicates internaly
Docker compose in local development
endnote

note right of User
User or another system that interacts with
one or more exposed services
endnote

note right of Front
For example: Front service communicates with
intermediate service with grpc protocol
endnote

note right of Intermediate
For example: Intermediate service communicates
with edge services with message protocol
endnote

note right of External_World
Any external services that application uses
Communication to those services are caried out through http
endnote

```

## Solution
In the following diagram `HttpToGrpcMock` is service hosted in docker compose with `external.service.one` and `external.service.two` names assigned to it. Edge services will have those URL's resolved to proxy service and no communication to external world will happen at this point.

```plantuml
package "Microservices" {
    [Front] -u-> [Intermediate]: grpc
    [Intermediate] -u-> [Edge1]: mq
    [Intermediate] -u-> [Edge2]: mq

    [Edge1] -u-> [HttpToGrpcMock]: http
    [Edge2] -u-> [HttpToGrpcMock]: http

    [HttpToGrpcMock]
}

package Integration_Tests {
    [GrpcClient] -l-> [HttpToGrpcMock]
}

note bottom of GrpcClient
When tests run, client connects to proxy living inside docker compose
and receives request to it proxied via grpc protocol. Grpc is used to
enable bidirectional comunication so not only request (to proxy) can
be recieved but responses posted back.
endnote
```

Sequence diagram may explain better how everyting works:
```plantuml
participant Test
participant GrpcClient
participant Front
participant Edge
participant HttpToGrpcMock

Test --> Front: POST /send/post
Test --> GrpcClient: instruct to wait for /external/call
Front -> Edge: Internal communication
Edge -> HttpToGrpcMock: http
note right of Edge
Thinks is calling actual
external service
endnote
HttpToGrpcMock -> GrpcClient: grpc
note right of HttpToGrpcMock
Info about received request
is forwarded to tests
endnote
GrpcClient -> Test
Test -> Test: Assert request
Test -> GrpcClient: Mocked response
GrpcClient -> HttpToGrpcMock
HttpToGrpcMock --> Edge: http
Edge --> Front
note right of Front
Responding back front may not be necessary in most cases.
Maybe it would be database update or something like that
endnote
Front --> Test: Resolve /send/post promise
Test -> Test: Assert response
```