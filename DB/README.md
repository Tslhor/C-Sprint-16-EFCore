# Local SQLite Database

## Overview

The application uses a local SQLite database. No database server or OS-specific configuration is required.

Database file:

```text
DB/shopping_system_local.db
```

The initial schema is created automatically by the seeder before data is inserted.

## Initialization and seeding

1. Prepare the database folder:
   - Windows PowerShell: run `init-db.ps1`.
   - macOS/Linux: run `./init-db.sh`.
2. Start the application.
3. The seeder reads data from `ShoppingSystem/DB_Seeder/DB_Seeder_Data` and inserts:
   - 5 supermarkets with names and addresses selected from the source data.
   - 10 customers with first names, last names, addresses, and discounts selected from the source data.
   - 15 products selected from the source data.

Order seeding is currently disabled in `DatabaseSeeder.SeedIfEmptyAsync`. The separate `SeedOrdersAsync` method prepares 5 orders with 6 products per order, each with quantity `3`. It currently assumes customer and supermarket IDs 1–5 and product IDs 1–15; review those assumptions before enabling it for a database that has been reseeded.

`Customers` contains `Id`, `FirstName`, `LastName`, `Address`, and `Discount`. `SuperMarkets` contains `Id`, `Name`, and `Address`. The seeder inserts these values using SQL parameters.

The initialization scripts only prepare the storage directory. The application creates the tables and seeds the base data on startup. `CREATE TABLE IF NOT EXISTS` does not add columns to existing tables; use a migration or create a new test database after backing up the existing file. Controllers still use temporary data until database access is connected.

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

The application does not automatically migrate an existing SQLite file. Follow this process whenever `schema.sql` or the model schema changes:

1. Stop the application.
2. Make a backup copy of `shopping_system_local.db`.
3. Update `schema.sql` to describe the desired schema.
4. Create a separate SQL migration script in this folder.
   - Use `ALTER TABLE` for additive changes.
   - Include data-conversion statements when existing values must be transformed.
5. Test the migration against a copy of the database using SQLite Viewer or the `sqlite3` command-line tool.
6. Apply the migration to the working database.
7. Verify the tables, columns, foreign keys, constraints, and data.
8. Update the matching C# models and seeder logic.
9. Build the application and verify that it starts successfully.
10. Record the migration script in the project so other developers can reproduce the change. Do not edit an applied migration; create a new migration script instead.

For destructive changes, or changes SQLite cannot apply directly:

1. Create a new database from `schema.sql`.
2. Copy the required data into the new database.
3. Verify the new database.
4. Replace the old database only after keeping the backup.

## Resetting the local database

To create a clean local database:

1. Stop the application.
2. Back up or delete `shopping_system_local.db`.
3. Start the application.

The seeder will create the current tables from `schema.sql` and insert the sample data.
