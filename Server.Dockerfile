FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY gRPCServer/*.csproj .
RUN dotnet restore *.csproj

# copy everything else and build app
COPY gRPCServer .
COPY ./proto ../proto
RUN dotnet publish *.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "gRPCServer.dll"]
EXPOSE 80