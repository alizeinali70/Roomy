FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Roomy.slnx ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

COPY src/roomy.API/roomy.API.csproj ./src/roomy.API/
COPY src/roomy.Application/roomy.Application.csproj ./src/roomy.Application/
COPY src/roomy.Domain/roomy.Domain.csproj ./src/roomy.Domain/
COPY src/roomy.Infrastructure/roomy.Infrastructure.csproj ./src/roomy.Infrastructure/
COPY src/roomy.Tests/roomy.Tests.csproj ./src/roomy.Tests/

RUN dotnet restore Roomy.slnx

COPY . .
RUN dotnet publish src/roomy.API/roomy.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "roomy.API.dll"]