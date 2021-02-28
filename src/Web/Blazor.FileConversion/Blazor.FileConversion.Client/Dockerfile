FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Web/Blazor.FileConversion/Blazor.FileConversion.Client/Blazor.FileConversion.Client.csproj Web/Blazor.FileConversion/Blazor.FileConversion.Client/
RUN dotnet restore "Web/Blazor.FileConversion/Blazor.FileConversion.Client/Blazor.FileConversion.Client.csproj"

COPY . .
WORKDIR "/src/Web/Blazor.FileConversion/Blazor.FileConversion.Client/"
RUN dotnet restore "Blazor.FileConversion.Client.csproj"
RUN dotnet build "Blazor.FileConversion.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blazor.FileConversion.Client.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
# Set the working directory in the image
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot ./
COPY Web/Blazor.FileConversion/Blazor.FileConversion.Client/nginx.conf ./
COPY Web/Blazor.FileConversion/Blazor.FileConversion.Client/routes.json ./