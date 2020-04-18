FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS restore
WORKDIR /

COPY ./nuget.config .
COPY ./*.sln .
COPY ./Directory.Build.props .
COPY ./src/Kevsoft.WLED/*.csproj ./src/Kevsoft.WLED/
COPY ./test/Kevsoft.WLED.Tests/*.csproj ./test/Kevsoft.WLED.Tests/
RUN dotnet restore

FROM restore as build
COPY ./icon.png .
COPY ./src/Kevsoft.WLED/ ./src/Kevsoft.WLED/
RUN dotnet build ./src/**/*.csproj --configuration Release --no-restore

FROM build as build-tests
COPY ./test/Kevsoft.WLED.Tests/ ./test/Kevsoft.WLED.Tests/
RUN dotnet build ./test/**/*.csproj --configuration Release --no-restore

FROM build-tests as test
ENTRYPOINT ["dotnet", "test", "./test/Kevsoft.WLED.Tests/Kevsoft.WLED.Tests.csproj", "--configuration", "Release", "--no-restore", "--no-build"]
CMD ["--logger" , "trx", "--results-directory", "./TestResults"]

FROM build as pack
RUN dotnet pack --configuration Release --no-build

FROM pack
ENTRYPOINT ["dotnet", "nuget", "push", "./**/*.nupkg"]
CMD ["--source", "NuGet.org"]