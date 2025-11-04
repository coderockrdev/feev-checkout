FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . .

EXPOSE 5265

ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5265"]
