FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Starfish.GrpcService.Server/Starfish.GrpcService.Server.csproj", "src/Starfish.GrpcService.Server/"]
RUN dotnet restore "src/Starfish.GrpcService.Server/Starfish.GrpcService.Server.csproj"
COPY . .
WORKDIR "/src/src/Starfish.GrpcService.Server"
RUN dotnet build "Starfish.GrpcService.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Starfish.GrpcService.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Starfish.GrpcService.Server.dll"]
