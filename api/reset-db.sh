rm -rf ./Migrations

dotnet ef database drop --force
dotnet ef migrations add InitialCreate
