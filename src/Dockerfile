FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build-env
ARG TARGETARCH
WORKDIR /source

# Copy project file and restore as distinct layers
COPY --link Fotos.App/*.csproj .
COPY --link *.props .
RUN dotnet restore -a $TARGETARCH

# Copy source code
COPY --link ./Fotos.App .

# Build aplication
RUN dotnet build --no-restore -a $TARGETARCH -c Release -o /build

FROM build-env AS publish-env
ARG TARGETARCH

# Publish aplication
RUN dotnet publish --no-build --no-restore  -c Release -a $TARGETARCH -o /publish /p:OutDir=/build /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
EXPOSE 8080
WORKDIR /app
COPY --link --from=publish-env /publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Fotos.App.dll"]