FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
# Remove line below for Prod
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# debug -> Release for Prod
ARG BUILD_CONFIGURATION=debug
# ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TicketAPI.csproj", "./"]
RUN dotnet restore "TicketAPI.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "TicketAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TicketAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketAPI.dll"]
