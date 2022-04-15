# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Aimo.Application/*.csproj ./Aimo.Application/
COPY Aimo.Core/*.csproj ./Aimo.Core/
COPY Aimo.Data/*.csproj ./Aimo.Data/
COPY Aimo.Domain/*.csproj ./Aimo.Domain/
COPY Aimo.Web/*.csproj ./Aimo.Web/
COPY Aimo.Web.Framework/*.csproj ./Aimo.Web.Framework/
WORKDIR /app/Aimo.Web/
RUN dotnet restore

WORKDIR /app
# Copy everything else and build
COPY Aimo.Application/. ./Aimo.Application/
COPY Aimo.Core/. ./Aimo.Core/
COPY Aimo.Data/. ./Aimo.Data/
COPY Aimo.Domain/. ./Aimo.Domain/
COPY Aimo.Web/. ./Aimo.Web/
COPY Aimo.Web.Framework ./Aimo.Web.Framework/
COPY Firebase/. ./Firebase
RUN dotnet publish Aimo.Web/ -c Release -o out 

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "Aimo.Web.dll"]