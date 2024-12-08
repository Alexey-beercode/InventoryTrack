FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5112

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MovementService.Presentation/MovementService.Presentation.csproj", "MovementService.Presentation/"]
COPY ["MovementService.Application/MovementService.Application.csproj", "MovementService.Application/"]
COPY ["MovementService.Domain/MovementService.Domain.csproj", "MovementService.Domain/"]
COPY ["MovementService.Infrastructure/MovementService.Infrastructure.csproj", "MovementService.Infrastructure/"]
RUN dotnet restore "MovementService.Presentation/MovementService.Presentation.csproj"
COPY . .
WORKDIR "/src/MovementService.Presentation"
RUN dotnet build "MovementService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MovementService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovementService.Presentation.dll"]
