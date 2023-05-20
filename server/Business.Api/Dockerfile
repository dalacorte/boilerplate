#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Business.Api/Business.Api.csproj", "Business.Api/"]
COPY ["Business.Application/Business.Application.csproj", "Business.Application/"]
COPY ["Business.Domain/Business.Domain.csproj", "Business.Domain/"]
COPY ["Business.Background/Business.Background.csproj", "Business.Background/"]
COPY ["Business.Repository/Business.Repository.csproj", "Business.Repository/"]
COPY ["Business.Database/Business.Database.csproj", "Business.Database/"]
COPY ["Business.Service/Business.Service.csproj", "Business.Service/"]
RUN dotnet restore "Business.Api/Business.Api.csproj"
COPY . .
WORKDIR "/src/Business.Api"
RUN dotnet build "Business.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Business.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Business.Api.dll"]