# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY PlatformaBackend/*.csproj ./PlatformaBackend/
COPY Platforma.Infrastructure/*.csproj ./Platforma.Infrastructure/
COPY Platforma.Domain/*.csproj ./Platforma.Domain/
COPY Platforma.Application/*.csproj ./Platforma.Application/
RUN dotnet restore

# copy everything else and build app
COPY PlatformaBackend/. ./PlatformaBackend/
COPY Platforma.Infrastructure/. ./Platforma.Infrastructure/
COPY Platforma.Domain/. ./Platforma.Domain/
COPY Platforma.Application/. ./Platforma.Application/
WORKDIR /source/PlatformaBackend
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "PlatformaBackend.dll"]