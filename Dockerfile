ARG VERSION=0.0.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /

COPY ./nuget.config .
COPY ./*.sln .
COPY ./Directory.Build.props .
COPY ./src/Kevsoft.WLED/*.csproj ./src/Kevsoft.WLED/
COPY ./src/Kevsoft.WLED.DependencyInjection/*.csproj ./src/Kevsoft.WLED.DependencyInjection/
COPY ./test/Kevsoft.WLED.Tests/*.csproj ./test/Kevsoft.WLED.Tests/
COPY ./samples/BasicConsole/*.csproj ./samples/BasicConsole/ 
RUN dotnet restore

FROM restore AS build
ARG VERSION
COPY ./icon.png .
COPY ./src/Kevsoft.WLED/ ./src/Kevsoft.WLED/
COPY ./src/Kevsoft.WLED.DependencyInjection/ ./src/Kevsoft.WLED.DependencyInjection/
RUN dotnet build ./src/Kevsoft.WLED.DependencyInjection/Kevsoft.WLED.DependencyInjection.csproj --configuration Release -p:Version=${VERSION} --no-restore

FROM build AS build-tests
ARG VERSION
COPY ./test/Kevsoft.WLED.Tests/ ./test/Kevsoft.WLED.Tests/
RUN dotnet build ./test/**/*.csproj --configuration Release -p:Version=${VERSION} --no-restore

FROM build-tests AS test
ENTRYPOINT ["dotnet", "test", "./test/Kevsoft.WLED.Tests/Kevsoft.WLED.Tests.csproj", "--configuration", "Release", "--no-restore", "--no-build"]
CMD ["--logger" , "trx", "--results-directory", "./TestResults"]

FROM build AS pack
ARG VERSION
RUN dotnet pack ./src/Kevsoft.WLED/Kevsoft.WLED.csproj --configuration Release -p:Version=${VERSION} --no-build
RUN dotnet pack ./src/Kevsoft.WLED.DependencyInjection/Kevsoft.WLED.DependencyInjection.csproj --configuration Release -p:Version=${VERSION} --no-build

FROM pack AS push
RUN env

ENTRYPOINT ["dotnet", "nuget", "push", "./src/Kevsoft.WLED/bin/Release/*.nupkg", "--source", "NuGet.org"]