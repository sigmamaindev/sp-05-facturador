# =======================
#  STAGE 1 — RESTORE
# =======================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

# Copiar solamente los csproj (mejora cache)
COPY *.sln ./
COPY Core/*.csproj Core/
COPY Infrastructure/*.csproj Infrastructure/
COPY API/*.csproj API/

# Restaurar dependencias usando cache
RUN dotnet restore

# =======================
#  STAGE 2 — BUILD
# =======================
FROM restore AS build
WORKDIR /src
COPY . .

RUN dotnet publish API/API.csproj -c Release -o /app/out

# =======================
#  STAGE 3 — RUNTIME
# =======================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]
