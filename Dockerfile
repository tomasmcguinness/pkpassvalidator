# Stage for restoring packages
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS packages
WORKDIR /app

COPY PassValidator.Lib/*.csproj ./PassValidator.Lib/
COPY PassValidator.API/*.csproj ./PassValidator.API/

RUN dotnet restore PassValidator.Lib/PassValidator.Lib.csproj
RUN dotnet restore PassValidator.API/PassValidator.API.csproj

# Stage for building lib and api
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS builder
WORKDIR /app

# Copy deps from packages stage
COPY --from=packages /app/PassValidator.Lib/* ./PassValidator.Lib/
COPY --from=packages /app/PassValidator.API/* ./PassValidator.API/

COPY PassValidator.Lib/* ./PassValidator.Lib/
COPY PassValidator.API/* ./PassValidator.API/

# Publish the final DLL
RUN dotnet publish -c Release -o output PassValidator.API/PassValidator.API.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1
WORKDIR /app
COPY --from=builder /app/PassValidator.API/output .
ENTRYPOINT ["dotnet", "PassValidator.API.dll"]