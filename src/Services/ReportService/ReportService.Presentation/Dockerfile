# Базовый образ для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5114

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Services

# Копируем файлы проекта и зависимости
COPY ReportService/ReportService.Presentation/ReportService.Presentation.csproj ReportService/ReportService.Presentation/
COPY ReportService/ReportService.Application/ReportService.Application.csproj ReportService/ReportService.Application/
COPY ReportService/ReportService.Domain/ReportService.Domain.csproj ReportService/ReportService.Domain/
COPY ReportService/ReportService.Infrastructure/ReportService.Infrastructure.csproj ReportService/ReportService.Infrastructure/
RUN dotnet restore ReportService/ReportService.Presentation/ReportService.Presentation.csproj

# Копируем весь код сервиса
COPY ReportService ./ReportService
COPY ../Contracts ./Contracts

# Сборка
WORKDIR /src/Services/ReportService/ReportService.Presentation
RUN dotnet build ReportService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ReportService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportService.Presentation.dll"]
