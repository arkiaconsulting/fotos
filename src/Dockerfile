FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build-env
WORKDIR /source

# Copy project file and restore as distinct layers
COPY --link Akc.Framework.Mediator/*.csproj ./Akc.Framework.Mediator/
COPY --link Fotos.App/*.csproj ./Fotos.App/
COPY --link Fotos.Adapters/*.csproj ./Fotos.Adapters/
COPY --link Fotos.Application/*.csproj ./Fotos.Application/
COPY --link Fotos.Core/*.csproj ./Fotos.Core/
COPY --link Fotos.Tests/*.csproj ./Fotos.Tests/
COPY --link *.props .
COPY --link *.sln .
RUN dotnet restore Fotos.sln

# Copy source code
COPY ./Fotos.App ./Fotos.App
COPY ./Fotos.Adapters ./Fotos.Adapters
COPY ./Fotos.Application ./Fotos.Application
COPY ./Fotos.Core ./Fotos.Core
COPY ./Akc.Framework.Mediator ./Akc.Framework.Mediator

# Build aplication
RUN dotnet build ./Fotos.App/Fotos.App.csproj --no-restore -c Release -o /build

FROM build-env AS publish-env

# Publish aplication
RUN dotnet publish ./Fotos.App/Fotos.App.csproj --no-build --no-restore  -c Release -o /publish /p:OutDir=/build /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
EXPOSE 8080
WORKDIR /app
COPY --link --from=publish-env /publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Fotos.App.dll"]