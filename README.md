echo "# Payment portal

This project uses Entity Framework Core with Code-First approach. The database connection is configured through the appsettings.json file, and migrations are used to create and update the database schema.

### EF Core Information
- Code-First Approach
- Uses `Add-Migration` and `Update-Database` commands
- SQL Server configured using default connection string

### Commands:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
