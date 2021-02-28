#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Shared/Shared.Abstraction/Shared.Abstraction.csproj Shared/Shared.Abstraction/
COPY Shared/Shared.Extensions/Shared.Extensions.csproj Shared/Shared.Extensions/
COPY Shared/Shared.Extensions/Shared.Extensions.csproj Shared/Shared.Extensions/
COPY Shared/Shared.Services/Shared.Services.csproj Shared/Shared.Services/
COPY Shared/Shared.Validations/Shared.Validations.csproj Shared/Shared.Validations/

COPY Services/FileConversion.Service/FileConversion.Api/FileConversion.Api.csproj Services/FileConversion.Service/FileConversion.Api/
RUN dotnet restore "Services/FileConversion.Service/FileConversion.Api/FileConversion.Api.csproj"
COPY . .
WORKDIR "/src/Services/FileConversion.Service/FileConversion.Api"
RUN dotnet restore "FileConversion.Api.csproj"
RUN dotnet build "FileConversion.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileConversion.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileConversion.Api.dll"]
