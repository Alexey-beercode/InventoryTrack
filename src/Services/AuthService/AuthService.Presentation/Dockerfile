FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthService.Presentation/AuthService.Presentation.csproj", "AuthService.Presentation/"]
COPY ["AuthService.BLL/AuthService.BLL.csproj", "AuthService.BLL/"]
COPY ["AuthService.Domain/AuthService.Domain.csproj", "AuthService.Domain/"]
COPY ["AuthService.DAL/AuthService.DAL.csproj", "AuthService.DAL/"]
RUN dotnet restore "AuthService.Presentation/AuthService.Presentation.csproj"
COPY . .
WORKDIR "/src/AuthService.Presentation"
RUN dotnet build "AuthService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.Presentation.dll"]
