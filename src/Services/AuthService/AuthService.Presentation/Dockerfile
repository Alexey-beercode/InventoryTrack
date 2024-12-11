# Базовый образ для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5110

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Services

# Копируем файлы проекта и зависимости
COPY AuthService/AuthService.Presentation/AuthService.Presentation.csproj AuthService/AuthService.Presentation/
COPY AuthService/AuthService.BLL/AuthService.BLL.csproj AuthService/AuthService.BLL/
COPY AuthService/AuthService.Domain/AuthService.Domain.csproj AuthService/AuthService.Domain/
COPY AuthService/AuthService.DAL/AuthService.DAL.csproj AuthService/AuthService.DAL/
RUN dotnet restore AuthService/AuthService.Presentation/AuthService.Presentation.csproj

# Копируем весь код сервиса
COPY AuthService ./AuthService
COPY ../Contracts ./Contracts

# Сборка
WORKDIR /src/Services/AuthService/AuthService.Presentation
RUN dotnet build AuthService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish AuthService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.Presentation.dll"]
