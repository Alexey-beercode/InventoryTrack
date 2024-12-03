FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5115
EXPOSE 5115

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReportService.Presentation/ReportService.Presentation.csproj", "ReportService.Presentation/"]
COPY ["ReportService.Domain/ReportService.Domain.csproj", "ReportService.Domain/"]
COPY ["ReportService.Infrastructure/ReportService.Infrastructure.csproj", "ReportService.Infrastructure/"]
COPY ["ReportService.Application/ReportService.Application.csproj", "ReportService.Application/"]
RUN dotnet restore "ReportService.Presentation/ReportService.Presentation.csproj"
COPY . .
WORKDIR "/src/ReportService.Presentation"
RUN dotnet build "ReportService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ReportService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportService.Presentation.dll"]
