#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
RUN apt update && apt upgrade -y && apt clean

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt update && apt-get install -y nodejs && apt clean
COPY ["Emails/Emails.csproj", "Emails/"]
RUN dotnet restore "Emails/Emails.csproj"
COPY . .
WORKDIR "/src/Emails"
RUN dotnet build "Emails.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Emails.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN useradd runner
USER runner
ENTRYPOINT dotnet Emails.dll --urls=http://0.0.0.0:$PORT
