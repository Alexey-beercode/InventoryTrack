# Базовый образ для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5111

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Services

# Копируем файлы проекта и зависимости
# Добавь этот шаг после копирования кода
COPY InventoryService/InventoryService.Application/Resources /app/Resources
COPY InventoryService/InventoryService.Presentation/InventoryService.Presentation.csproj InventoryService/InventoryService.Presentation/
COPY InventoryService/InventoryService.Application/InventoryService.Application.csproj InventoryService/InventoryService.Application/
COPY InventoryService/InventoryService.Domain/InventoryService.Domain.csproj InventoryService/InventoryService.Domain/
COPY InventoryService/InventoryService.Infrastructure/InventoryService.Infrastructure.csproj InventoryService/InventoryService.Infrastructure/
RUN dotnet restore InventoryService/InventoryService.Presentation/InventoryService.Presentation.csproj

# Копируем весь код сервиса
COPY InventoryService ./InventoryService
COPY ../Contracts ./Contracts

# Сборка
WORKDIR /src/Services/InventoryService/InventoryService.Presentation
RUN dotnet build InventoryService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish InventoryService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.Presentation.dll"]
