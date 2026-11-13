# Shopping System Work Plan

Checklist for Petro and Ihor implementing the [assignment](README.md). Petro owns Customers, Supermarkets, order views, and the shared layout. Ihor owns Products including views, the order backend, and database setup. This split aims for a similar workload; review the balance together after the first working features.

## Ownership

| Area | Owner |
| --- | --- |
| Customer, Supermarket, and Order views, shared layout, and navigation | Petro |
| Product views | Ihor |
| Customer and Supermarket models, validation, and controllers | Petro |
| Customer sorting and filtering; supermarket pagination | Petro |
| Product, Order, and OrderDetails models, validation, and controllers | Ihor |
| Order creation, related data queries, and total calculation | Ihor |
| Initial MVC project creation | Petro |
| EF Core packages, ShoppingContext, SQL Server configuration, migrations, and SampleData | Ihor |
| View and controller contracts, integration, and final verification | Both |

Each owner implements and checks their area. Both developers review changes that connect their areas.

## Progress tracking

Use this file as the shared checklist linked from [README.md](README.md#work-plan-and-progress). An unchecked box (`- [ ]`) means work remains; a checked box (`- [x]`) means the task is completed and merged. Commit and push checkbox updates with the corresponding changes. Project creation below is already complete.

## Agree before implementation

- [ ] **Both:** Choose the .NET and EF Core versions and confirm the SQL Server setup. Agree how to satisfy the README's Startup.cs and Program.cs requirements for the chosen version.
- [ ] **Both:** Agree on model property names and types using the [database schema](images/1.png), including string lengths, foreign keys, and navigation properties.
- [ ] **Both:** Agree on controller actions, routes, view models, form fields, dropdown data, and query parameters before building the views and backend separately.
- [ ] **Both:** Agree on validation rules, deletion behavior for related records, and whether the customer's discount affects the displayed total; the README does not specify the discount calculation.
- [ ] **Both:** Use separate feature branches and pull requests. Check off completed tasks in this file when their changes are merged.
- [ ] **Both:** Keep shared files under Ihor's ownership during setup. Coordinate edits to ShoppingContext, Program.cs, Startup.cs, appsettings.json, SampleData.cs, and migrations.
- [ ] **Both:** Scaffold only your assigned controllers and views. Ihor owns Product views; Petro owns Customer, Supermarket, and Order views. Coordinate changes to the shared layout.

## Project foundation

Petro has created the MVC project. Petro can prepare the page layout while Ihor configures EF Core and the database.

- [x] **Petro:** Create the ASP.NET Core MVC project.
- [ ] **Ihor:** Add the required EF Core and SQL Server packages.
- [ ] **Ihor:** Configure ShoppingContext registration and the connection string. Document local setup without committing private credentials.
- [ ] **Ihor:** Merge the runnable project skeleton so both developers can build from the same starting point.
- [ ] **Petro:** Prepare the shared layout, home page, and navigation links for Products, Customers, Supermarkets, and Orders.

## Petro implementation

### Customers

- [ ] Create the Customer model with primary key, first name, last name, address, discount, and navigation to orders, matching the agreed schema mapping.
- [ ] Implement CustomersController CRUD actions: list, details, create, edit, and delete.
- [ ] Create Customer views for all CRUD actions, including validation messages.
- [ ] Add ascending and descending sorting by last name and address.
- [ ] Add substring filtering by first name or last name, and preserve the filter when changing sort order.
- [ ] Check CRUD, each sorting direction, filtering, and empty results.

### Supermarkets

- [ ] Create the Supermarket model with primary key, name, address, and navigation to orders, matching the agreed schema mapping.
- [ ] Implement SupermarketsController CRUD actions: list, details, create, edit, and delete.
- [ ] Create Supermarket views for all CRUD actions, including validation messages.
- [ ] Add pagination to the supermarket list, including page controls and handling for empty or invalid pages.
- [ ] Check CRUD and navigation through the first, middle, and last pages.

### Order views

Build these against the contracts agreed with Ihor. Final checks depend on his controllers and view data.

- [ ] Create the Orders list showing the order date, customer name, and supermarket name.
- [ ] Create the selected order view showing products, quantities, prices, line totals, and the total cost.
- [ ] Create order forms for selecting a customer and supermarket, choosing products, and entering quantities with visible prices.
- [ ] Create views or controls to edit and delete orders and to add, edit, and remove order details.
- [ ] Display readable names in related entity dropdowns and lists, and display backend validation errors.
- [ ] Connect order views to the agreed actions and check links, submissions, and validation with Ihor.

## Ihor implementation

### Models and database

- [ ] Create Product with primary key, name, price, and navigation to order details.
- [ ] Create Order with primary key, customer foreign key, supermarket foreign key, order date, and navigation properties.
- [ ] Create OrderDetails with primary key, order foreign key, product foreign key, quantity, and navigation properties.
- [ ] Integrate Petro's Customer and Supermarket models into ShoppingContext with DbSets for all five entities.
- [ ] Configure relationships, column mappings, constraints, and the agreed deletion behavior.
- [ ] After all models are integrated, create the initial migration and verify that it creates the expected SQL Server schema. Own subsequent migrations to avoid conflicting snapshots.
- [ ] Implement SampleData.cs and invoke seeding through the service provider in Program.cs. Include customers, supermarkets, products, orders, and order details; obtain Customer and Supermarket sample values from Petro.
- [ ] Ensure repeated application startup does not duplicate seed records.

### Products

- [ ] Implement ProductsController CRUD actions and validation.
- [ ] Create Product views for list, details, create, edit, and delete, showing product prices and validation messages.
- [ ] Check the complete Product workflow through the views.

### Order backend

- [ ] Implement order CRUD actions and provide customer and supermarket names for lists and selection controls.
- [ ] Implement CRUD for order details, either through order actions or a dedicated controller, using the agreed view contracts.
- [ ] Save an order with its selected supermarket, customer, date, products, and quantities; keep the order and its details consistent if saving fails.
- [ ] Load Customers and Supermarkets for the order list and load OrderDetails with Products for the selected order.
- [ ] Calculate line totals and the total order cost on the server using stored prices and the agreed discount rule.
- [ ] Validate submitted relationships and quantities, and return useful validation errors to the views.
- [ ] Check Order and OrderDetails CRUD, related data loading, and total calculations.

## Integration and completion

- [ ] **Both:** Merge in dependency order: project skeleton, models and database, controllers and views, then sorting, filtering, pagination, and order workflow.
- [ ] **Both:** Run the documented setup against a fresh local database and confirm migrations and seeding succeed.
- [ ] **Both:** Create, read, update, and delete records for all five entities through the application, following the agreed rules for related records.
- [ ] **Both:** Create an order with multiple products and quantities; verify its customer, supermarket, date, product prices, line totals, and total cost.
- [ ] **Both:** Select different orders and verify that each displays only its own order details.
- [ ] **Both:** Verify customer sorting and filtering, supermarket pagination, readable relationship names, and navigation from the home page.
- [ ] **Both:** Check invalid input, missing records, and deletion of records that have related data.
- [ ] **Ihor:** Add build, database migration, and run instructions to README.md.
- [ ] **Both:** Build the final merged application and review it against every requirement in README.md before submission.
