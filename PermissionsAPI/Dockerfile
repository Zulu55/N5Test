# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["/PermissionsAPI/PermissionsAPI.csproj", "PermissionsAPI/"]
RUN dotnet restore "PermissionsAPI/PermissionsAPI.csproj"
COPY . .
WORKDIR "/src/PermissionsAPI"
RUN dotnet build "PermissionsAPI.csproj" -c Release -o /app/build

# Publish the app to the /app/publish directory
FROM build AS publish
RUN dotnet publish "PermissionsAPI.csproj" -c Release -o /app/publish

# Use the base image to create a final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PermissionsAPI.dll"]
