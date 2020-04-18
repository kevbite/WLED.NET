ARG VERSION=0.0.0
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS restore
ARG VERSION
WORKDIR /

COPY ./nuget.config .
COPY ./*.sln .
COPY ./Directory.Build.props .
COPY ./src/Kevsoft.WLED/*.csproj ./src/Kevsoft.WLED/
COPY ./test/Kevsoft.WLED.Tests/*.csproj ./test/Kevsoft.WLED.Tests/
RUN dotnet restore

FROM restore as build
ARG VERSION
COPY ./icon.png .
COPY ./src/Kevsoft.WLED/ ./src/Kevsoft.WLED/
RUN dotnet build ./src/**/*.csproj --configuration Release -p:Version=${VERSION} --no-restore

FROM build as build-tests
ARG VERSION
COPY ./test/Kevsoft.WLED.Tests/ ./test/Kevsoft.WLED.Tests/
RUN dotnet build ./test/**/*.csproj --configuration Release -p:Version=${VERSION} --no-restore

FROM build-tests as test
ENTRYPOINT ["dotnet", "test", "./test/Kevsoft.WLED.Tests/Kevsoft.WLED.Tests.csproj", "--configuration", "Release", "--no-restore", "--no-build"]
CMD ["--logger" , "trx", "--results-directory", "./TestResults"]

FROM build as pack
ARG VERSION
RUN dotnet pack --configuration Release -p:Version=${VERSION} --no-build

FROM pack as push
RUN env

ENTRYPOINT ["dotnet", "nuget", "push", "./src/Kevsoft.WLED/bin/Release/*.nupkg", "--source", "NuGet.org"]