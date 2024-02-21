FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR "/src"

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ["src/Aff.YB.DataSync.Service.Core/Aff.YB.DataSync.Service.Core.csproj", "./Aff.YB.DataSync.Service.Core/"]
RUN dotnet restore "Aff.YB.DataSync.Service.Core/Aff.YB.DataSync.Service.Core.csproj" -v n

COPY ["src/Aff.YB.DataSync.Service.Infrastructure/Aff.YB.DataSync.Service.Infrastructure.csproj", "./Aff.YB.DataSync.Service.Infrastructure/"]
RUN dotnet restore "Aff.YB.DataSync.Service.Infrastructure/Aff.YB.DataSync.Service.Infrastructure.csproj" -v n

COPY ["src/Aff.YB.DataSync.Service.Host/Aff.YB.DataSync.Service.Host.csproj", "./Aff.YB.DataSync.Service.Host/"]
RUN dotnet restore "Aff.YB.DataSync.Service.Host/Aff.YB.DataSync.Service.Host.csproj" -v n

COPY "/src" .

WORKDIR "/src/Aff.YB.DataSync.Service.Host"
RUN dotnet build "Aff.YB.DataSync.Service.Host.csproj" -c Release -o /app/build

RUN dotnet publish "Aff.YB.DataSync.Service.Host.csproj" -c Release -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR "/app"

COPY --from=builder /app/publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Aff.YB.DataSync.Service.Host.dll"]
