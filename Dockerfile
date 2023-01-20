﻿FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env

WORKDIR /app
ADD /src .
RUN dotnet publish Starfish.GrpcService.Server/Starfish.GrpcService.Server.csproj -c Release -o ./output

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /app
COPY --from=build-env /app/output .

EXPOSE 80
ENTRYPOINT ["dotnet", "Starfish.GrpcService.Server.dll"]