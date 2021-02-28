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
EXPOSE 80
COPY --from=publish /app/publish/wwwroot ./
COPY Web/Blazor.FileConversion/Blazor.FileConversion.Client/nginx.conf ./
COPY Web/Blazor.FileConversion/Blazor.FileConversion.Client/start.sh ./start.sh
# This command is optional,   
# it is just used to make sure all the EOL unix compliant  
RUN sed -i -e "s/\r$//" start.sh  
# The command that will be executed when the container starts  
CMD /bin/sh ./start.sh 
