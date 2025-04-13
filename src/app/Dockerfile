FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080:8080
EXPOSE 4040:4040

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
WORKDIR /app
RUN dotnet restore "Web/Web.csproj"
RUN dotnet build "./Web/Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Web/Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]
