FROM microsoft/dotnet:latest AS builder
ARG BUILD_VERSION  
WORKDIR /source

COPY . .
RUN dotnet publish /p:Version=$BUILD_VERSION --output /app/ --configuration Release

# Stage 2
FROM microsoft/dotnet:2.1.5-aspnetcore-runtime
WORKDIR /app
COPY --from=builder /app .
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "Wikiled.Text.Anomaly.Service.dll"]

