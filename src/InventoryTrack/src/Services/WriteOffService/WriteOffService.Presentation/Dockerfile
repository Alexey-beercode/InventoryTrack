FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WriteOffService.Presentation/WriteOffService.Presentation.csproj", "WriteOffService.Presentation/"]
COPY ["WriteOffService.Application/WriteOffService.Application.csproj", "WriteOffService.Application/"]
COPY ["WriteOffService.Domain/WriteOffService.Domain.csproj", "WriteOffService.Domain/"]
COPY ["WriteOffService.Infrastructure/WriteOffService.Infrastructure.csproj", "WriteOffService.Infrastructure/"]
RUN dotnet restore "WriteOffService.Presentation/WriteOffService.Presentation.csproj"
COPY . .
WORKDIR "/src/WriteOffService.Presentation"
RUN dotnet build "WriteOffService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WriteOffService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WriteOffService.Presentation.dll"]
