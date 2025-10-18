FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

# Copy the project file and restore dependencies
COPY Lumo.csproj ./
RUN dotnet restore

# Copy the entire source code
COPY . ./

# Expose port for development
EXPOSE 5000

# Use dotnet watch for hot reload in development
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://+:5000"]
