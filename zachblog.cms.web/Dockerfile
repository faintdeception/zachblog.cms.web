#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["zachblog.cms.web/zachblog.cms.web.csproj", "zachblog.cms.web/"]
RUN dotnet restore "zachblog.cms.web/zachblog.cms.web.csproj"
COPY . .
WORKDIR "/src/zachblog.cms.web"
RUN dotnet build "zachblog.cms.web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "zachblog.cms.web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "zachblog.cms.web.dll"]