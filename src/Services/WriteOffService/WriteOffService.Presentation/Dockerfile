# Базовый образ для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5112

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Services

# Копируем файлы проекта и зависимости
COPY MovementService/MovementService.Presentation/MovementService.Presentation.csproj MovementService/MovementService.Presentation/
COPY MovementService/MovementService.Application/MovementService.Application.csproj MovementService/MovementService.Application/
COPY MovementService/MovementService.Domain/MovementService.Domain.csproj MovementService/MovementService.Domain/
COPY MovementService/MovementService.Infrastructure/MovementService.Infrastructure.csproj MovementService/MovementService.Infrastructure/
RUN dotnet restore MovementService/MovementService.Presentation/MovementService.Presentation.csproj

# Копируем весь код сервиса
COPY MovementService ./MovementService
COPY ../Contracts ./Contracts

# Сборка
WORKDIR /src/Services/MovementService/MovementService.Presentation
RUN dotnet build MovementService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish MovementService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovementService.Presentation.dll"]
