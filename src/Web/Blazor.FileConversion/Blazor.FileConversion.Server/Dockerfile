FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Web/Blazor.FileConversion/Blazor.FileConversion.Client/Blazor.FileConversion.Client.csproj Web/Blazor.FileConversion/Blazor.FileConversion.Client/
COPY Web/Blazor.FileConversion/Blazor.FileConversion.Server/Blazor.FileConversion.Server.csproj Web/Blazor.FileConversion/Blazor.FileConversion.Server/
RUN dotnet restore "Web/Blazor.FileConversion/Blazor.FileConversion.Server/Blazor.FileConversion.Server.csproj"

COPY . .
WORKDIR "/src/Web/Blazor.FileConversion/Blazor.FileConversion.Server/"
RUN dotnet restore "Blazor.FileConversion.Server.csproj"
RUN dotnet build "Blazor.FileConversion.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blazor.FileConversion.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blazor.FileConversion.Server.dll"]
