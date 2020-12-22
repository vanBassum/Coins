#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BinanceSync/BinanceSync.csproj", "BinanceSync/"]
COPY ["lib/STDLib/STDLib.csproj", "lib/STDLib/"]
RUN dotnet restore "BinanceSync/BinanceSync.csproj"
COPY . .
WORKDIR "/src/BinanceSync"
RUN dotnet build "BinanceSync.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BinanceSync.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BinanceSync.dll"]