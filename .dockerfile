# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy Solution File to support Multi-Project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY fh-referral-api.sln ./

# Copy Dependencies
COPY ["src/FamilyHubs.ReferralApi.Api/FamilyHubs.ReferralApi.Api.csproj", "src/FamilyHubs.ReferralApi.Api/"]
COPY ["src/FamilyHubs.ReferralApi.Core/FamilyHubs.ReferralApi.Core.csproj", "src/FamilyHubs.ReferralApi.Core/"]
COPY ["src/FamilyHubs.ReferralApi.Infrastructure/FamilyHubs.ReferralApi.Infrastructure.csproj", "src/FamilyHubs.ReferralApi.Infrastructure/"]
COPY ["src/FamilyHubs.ReferralApi.Common/FamilyHubs.ReferralApi.Common.csproj", "src/FamilyHubs.ReferralApi.Common/"]

# Restore Project
RUN dotnet restore "src/FamilyHubs.ReferralApi.Api/FamilyHubs.ReferralApi.Api.csproj"

# Copy Everything
COPY . .

# Build
WORKDIR "/src/src/FamilyHubs.ReferralApi.Api"
RUN dotnet build "FamilyHubs.ReferralApi.Api.csproj" -c Release -o /app/build

# publish
FROM build AS publish
WORKDIR "/src/src/FamilyHubs.ReferralApi.Api"
RUN dotnet publish "FamilyHubs.ReferralApi.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FamilyHubs.ReferralApi.Api.dll"]

