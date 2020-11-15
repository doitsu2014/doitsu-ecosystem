#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY Identity.Service.OpenIdServer/Identity.Service.OpenIdServer.csproj Identity.Service.OpenIdServer/
RUN dotnet restore "Identity.Service.OpenIdServer/Identity.Service.OpenIdServer.csproj"
COPY . .
WORKDIR "/src/Identity.Service.OpenIdServer"
RUN dotnet build "Identity.Service.OpenIdServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Service.OpenIdServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Service.OpenIdServer.dll"]
