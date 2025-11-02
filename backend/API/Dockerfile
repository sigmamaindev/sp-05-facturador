# ====== Etapa 1: Build ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.sln ./
COPY Core/*.csproj Core/
COPY Infrastructure/*.csproj Infrastructure/
COPY API/*.csproj API/
RUN dotnet restore

# Copiar todo y compilar
COPY . .
WORKDIR /app/API
RUN dotnet publish -c Release -o /out

# ====== Etapa 2: Runtime ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar los archivos compilados
COPY --from=build /out .

# Puerto usado por Railway (variable de entorno)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "API.dll"]
