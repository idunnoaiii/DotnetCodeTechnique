FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WhereClauseDynamicLinq/WhereClauseDynamicLinq.csproj", "WhereClauseDynamicLinq/"]
RUN dotnet restore "WhereClauseDynamicLinq/WhereClauseDynamicLinq.csproj"
COPY . .
WORKDIR "/src/WhereClauseDynamicLinq"
RUN dotnet build "WhereClauseDynamicLinq.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WhereClauseDynamicLinq.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WhereClauseDynamicLinq.dll"]
