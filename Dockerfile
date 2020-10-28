#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt update
#Install Java runtime to run Apache Ignite library.
RUN mkdir -p /usr/share/man/man1
RUN apt-get install -y default-jre

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs
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
CMD dotnet Emails.dll --urls=http://*:$PORT
