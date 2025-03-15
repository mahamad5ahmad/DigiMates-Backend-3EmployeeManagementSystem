# Use the official MySQL image from Docker Hub
FROM mysql:latest

# Set MySQL environment variables (modify these values)
ENV MYSQL_DATABASE=TestDB
ENV MYSQL_USER=rooot
ENV MYSQL_PASSWORD=0193247637mM!
ENV MYSQL_ROOT_PASSWORD=0193247637mM!

# Expose MySQL default port
EXPOSE 3306

# Run MySQL
CMD ["mysqld"]


# Use the official .NET 8.0 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET 8.0 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["EmployeeManagementSystem.csproj", "./"]
RUN dotnet restore "./EmployeeManagementSystem.csproj"

# Copy the entire project and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagementSystem.dll"]
