# Local SQLite Database

## Overview

The application uses a local SQLite database. No database server or OS-specific configuration is required.

Database file:

```text
DB/shopping_system_local.db
```

The schema is managed by EF Core migrations and applied automatically before data is inserted.

## Initialization and seeding

1. Prepare the database folder:
   - Windows PowerShell: run `init-db.ps1`.
   - macOS/Linux: run `./init-db.sh`.
2. Start the application.
3. The seeder reads data from `DB_Seeder/SeedData` and inserts:
   - 5 supermarkets with names and addresses selected from the source data.
   - 10 customers with first names, last names, addresses, and discounts selected from the source data.
   - 15 products selected from the source data.

The seeder prepares 5 orders with 6 products per order, each with quantity `3`. It queries generated database keys rather than assuming sequential IDs.

`Customers` contains `Id`, `FirstName`, `LastName`, `Address`, and `Discount`. `SuperMarkets` contains `Id`, `Name`, and `Address`. The seeder inserts these values using SQL parameters.

The initialization scripts only prepare the storage directory. The application applies pending EF Core migrations and seeds data on startup. 
Existing databases created by the legacy schema script should be backed up and recreated before first migration-based startup.

## Data rules

- Product prices are stored as plain decimal numeric values. Add the UAH currency label only when displaying prices.
- Every order has one required `CustomerId` and one required `SupermarketId`.
- Foreign keys ensure that order customers and supermarkets exist.
- The `OrderDetails` unique constraint prevents the same product from being added twice to one order.
- Order quantities must be positive.

## Viewing the database

For inspection in VS Code, install the **SQLite Viewer** extension and open:

```text
shopping_system_local.db
```

## Updating the database schema

The application automatically applies pending EF Core migrations. Follow this process whenever the model schema changes:

1. Stop the application.
2. Make a backup copy of `shopping_system_local.db`.
3. Update the C# models and `ShoppingDbContext` configuration.
4. Run `dotnet ef migrations add <MigrationName> --project ShoppingSystem/ShoppingSystem.csproj`.
5. Test the migration against a copy of the database.
6. Build the application and verify that it starts successfully.
7. Do not edit an applied migration; create a new migration instead.

For destructive changes, or changes SQLite cannot apply directly:

	1. Create a new database by deleting the local SQLite file and restarting the application.
2. Copy the required data into the new database.
3. Verify the new database.
4. Replace the old database only after keeping the backup.

## Resetting the local database

To create a clean local database:

1. Stop the application.
2. Back up or delete `shopping_system_local.db`.
3. Start the application.

The seeder will apply the current EF Core migrations and insert the sample data.
