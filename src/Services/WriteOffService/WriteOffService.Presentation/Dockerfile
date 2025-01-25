# Базовый образ для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5113

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Services

# Копируем файлы проекта и зависимости
COPY WriteOffService/WriteOffService.Presentation/WriteOffService.Presentation.csproj WriteOffService/WriteOffService.Presentation/
COPY WriteOffService/WriteOffService.Application/WriteOffService.Application.csproj WriteOffService/WriteOffService.Application/
COPY WriteOffService/WriteOffService.Domain/WriteOffService.Domain.csproj WriteOffService/WriteOffService.Domain/
COPY WriteOffService/WriteOffService.Infrastructure/WriteOffService.Infrastructure.csproj WriteOffService/WriteOffService.Infrastructure/
RUN dotnet restore WriteOffService/WriteOffService.Presentation/WriteOffService.Presentation.csproj

# Копируем весь код сервиса
COPY WriteOffService ./WriteOffService
COPY ../Contracts ./Contracts

# Сборка
WORKDIR /src/Services/WriteOffService/WriteOffService.Presentation
RUN dotnet build WriteOffService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish WriteOffService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WriteOffService.Presentation.dll"]
