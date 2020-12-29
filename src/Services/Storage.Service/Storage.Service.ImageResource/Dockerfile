#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

COPY Storage.Service.ApplicationCore/Storage.Service.ApplicationCore.csproj Storage.Service.ApplicationCore/
RUN dotnet restore "Storage.Service.ApplicationCore/Storage.Service.ApplicationCore.csproj"

COPY Storage.Service.ImageResource/Storage.Service.ImageResource.csproj Storage.Service.ImageResource/
RUN dotnet restore "Storage.Service.ImageResource/Storage.Service.ImageResource.csproj"

COPY . .
WORKDIR "/src/Storage.Service.ImageResource"
RUN dotnet build "Storage.Service.ImageResource.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Storage.Service.ImageResource.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Storage.Service.ImageResource.dll"]
