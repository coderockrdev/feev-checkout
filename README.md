# Feev Checkout

## Getting Started

### Requirements

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/) and npm
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- (Optional) [Make](https://www.gnu.org/software/make/) - to run Makefile commands from `api/`
- (Optional) [Docker](https://docs.docker.com/get-docker/) - alternative way to run PostgreSQL

---

## Setup

### 1. Clone the repository

```bash
git clone git@github.com:coderockrdev/feev-checkout.git
cd feev-checkout
```

### 2. Configure the API

Create `appsettings.Development.json` based on `appsettings.json` with your PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=feev;Username=feev;Password=secret"
  }
}
```

### 3. Setup PostgreSQL

#### Using local PostgreSQL

Create the database and user:

```bash
psql -U postgres -c "CREATE USER feev WITH PASSWORD 'secret';"
psql -U postgres -c "CREATE DATABASE feev OWNER feev;"
```

#### Using Docker

```bash
docker volume create feev-postgres-data
```

```bash
docker run -d \
  --name feev-postgres \
  -e POSTGRES_USER=feev \
  -e POSTGRES_PASSWORD=secret \
  -e POSTGRES_DB=feev \
  -p 5432:5432 \
  -v feev-postgres-data:/var/lib/postgresql/data \
  postgres:16
```

### 4. Run the API

```bash
cd api
dotnet restore
dotnet watch run
```

The API will be available at http://localhost:5265

### 5. Configure Web Environment

```bash
cd web
cp src/environments/environment.ts src/environments/environment.development.ts
```

### 6. Run the Web Frontend

```bash
cd web
npm install
npm start
```

The frontend will be available at http://localhost:4200

---

## Create an Establishment

Before using the API, you need to create at least one establishment. Run the interactive CLI:

```bash
cd api

# Using Make
make create-establishment

# Or directly with dotnet
dotnet run -- establishment add
```

The CLI will prompt for:
- Full name and short name
- CNPJ
- Domain
- Payment methods (Boleto, Pix, Credit Card)
- Provider configurations (JSON) for each selected payment method

After creation, you'll receive the `ClientId` and `ClientSecret` credentials for API authentication.

---

## API Documentation

Once running, access the Swagger UI at: http://localhost:5265/swagger

## Useful Commands

From the `api/` directory:

```bash
make help                 # Show available commands
make create-establishment # Create a new establishment
make reset-db             # Reset database and recreate migrations
```
