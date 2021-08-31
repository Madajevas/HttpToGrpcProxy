#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DockerComposeTests/TestApp.Tests/TestApp.Tests.csproj", "DockerComposeTests/TestApp.Tests/"]
COPY ["ProxyInterceptorTestsClient/ProxyInterceptorTestsClient.csproj", "ProxyInterceptorTestsClient/"]
COPY ["HttpToGrpcProxy.Commons/HttpToGrpcProxy.Commons.csproj", "HttpToGrpcProxy.Commons/"]
RUN dotnet restore "DockerComposeTests/TestApp.Tests/TestApp.Tests.csproj"
COPY . .
WORKDIR "/src/DockerComposeTests/TestApp.Tests"
RUN dotnet build "TestApp.Tests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestApp.Tests.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS test
WORKDIR /test
COPY --from=publish /app/publish .
RUN dotnet test "TestApp.Tests.dll" --results-directory /test/results

FROM scratch as output
COPY --from=test /test/results ./test_results