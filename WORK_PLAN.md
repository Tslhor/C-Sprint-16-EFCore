# Shopping System Work Plan

Checklist for Petro and Ihor implementing the [assignment](README.md). Petro owns all entity models and views, the shared layout, and the Customers and Supermarkets backend. Ihor owns the Products and Orders backend and database setup. Review the workload together as these responsibilities change.

## Ownership

| Area | Owner |
| --- | --- |
| All entity models, model validation, foreign keys, and navigation properties | Petro |
| All views, shared partials, shared layout, and navigation | Petro |
| Customer and Supermarket controllers | Petro |
| Customer sorting and filtering; supermarket pagination | Petro |
| Product, Order, and OrderDetails controllers and backend validation | Ihor |
| Order creation, related data queries, and total calculation | Ihor |
| Initial MVC project creation | Petro |
| EF Core packages, ShoppingContext, SQL Server configuration, migrations, and SampleData | Ihor |
| View and controller contracts, integration, and final verification | Both |

Each owner implements and checks their area. Both developers review changes that connect their areas.

All views use the Bootstrap 5 files already bundled with the MVC project and the shared layout. Use Bootstrap components and utilities for navigation, grids, cards, tables, forms, buttons, validation, and pagination. Add custom CSS only where Bootstrap does not cover the requirement.

Petro will focus first on models and UI, currently the Create Order form. Customer filtering and sorting and supermarket pagination can also be developed against temporary collections when those tasks resume. Database queries and persistence will follow once ShoppingContext is available. Controllers may use clearly marked temporary data to preview views; these previews do not complete the database-backed CRUD requirements.

## Progress tracking

Use this file as the shared checklist linked from [README.md](README.md#work-plan-and-progress). An unchecked box (`- [ ]`) means work remains; a checked box (`- [x]`) means the task is completed and merged. Commit and push checkbox updates with the corresponding changes. Project creation below is already complete.

Status reviewed against master on 2026-11-14, including merged PR #7 for the Orders page and Create form. Checked UI and controller skeleton tasks do not imply working database operations. Current controllers use temporary in-memory data; the Orders list is empty until data is supplied.

Before migrations, review the implemented model choices together: Customer first and last names are limited to 30 characters and address to 50; Supermarket name is limited to 30 and address to 50; Product price uses decimal; OrderDetails quantity uses float. These choices need explicit SQL mappings where they differ from the assignment schema.

## Agree before implementation

- [x] **Petro:** Create the project targeting .NET 10.
- [ ] **Both:** Agree on EF Core versions and SQL Server setup, and how to satisfy the README's Startup.cs configuration requirement using the current Program.cs structure.
- [ ] **Both:** Agree on model property names and types using the [database schema](images/1.png), including string lengths, foreign keys, and navigation properties.
- [ ] **Both:** Agree on controller actions, routes, view models, form fields, dropdown data, and query parameters before building the views and backend separately.
- [ ] **Both:** Agree on validation rules, deletion behavior for related records, and whether the customer's discount affects the displayed total; the README does not specify the discount calculation.
- [x] **Petro:** Use separate feature branches and pull requests for layout, models, Customer pages, Supermarket pages, Product pages, and Order pages.
- [ ] **Both:** Keep shared files under Ihor's ownership during setup. Coordinate edits to ShoppingContext, Program.cs, Startup.cs, appsettings.json, SampleData.cs, and migrations.
- [ ] **Both:** Scaffold only your assigned controllers and views. Petro owns all views; Ihor coordinates Product and Order controllers with Petro without overwriting his views. Coordinate changes to the shared layout.
- [ ] **Both:** Review the README's scaffolding requirement: existing controllers and views were prepared manually; confirm whether they need adapting once EF Core is available.

## Project foundation

Petro has created the MVC project. Petro can prepare the page layout while Ihor configures EF Core and the database.

- [x] **Petro:** Create the ASP.NET Core MVC project.
- [ ] **Ihor:** Add the required EF Core and SQL Server packages.
- [ ] **Ihor:** Configure ShoppingContext registration and the connection string. Document local setup without committing private credentials.
- [x] **Petro:** Merge the runnable project skeleton so both developers can build from the same starting point.
- [x] **Petro:** Prepare the shared layout, home page, and navigation links for Products, Customers, Supermarkets, and Orders.

## Petro implementation

### Customers

- [x] Create the Customer model with primary key, first name, last name, address, discount, validation attributes, and navigation to orders.
- [x] Prepare CustomersController action skeletons and basic Create/Edit ModelState validation with temporary sample data.
- [ ] Complete CustomersController CRUD: database queries, lookup by ID, persistence, and missing-record handling.
- [x] Create Customer views for list, details, create, edit, and delete, including validation markup and a shared details partial.
- [x] Prepare the Customer search form and sorting links in the UI.
- [ ] Add ascending and descending sorting by last name and address.
- [ ] Add substring filtering by first name or last name, and preserve the filter when changing sort order.
- [ ] Check CRUD, each sorting direction, filtering, and empty results.

### Supermarkets

- [x] Create the Supermarket model with primary key, name, address, validation attributes, and navigation to orders.
- [x] Prepare SupermarketsController action skeletons and basic Create/Edit ModelState validation with temporary sample data.
- [ ] Complete SupermarketsController CRUD: database queries, lookup by ID, persistence, and missing-record handling.
- [x] Create Supermarket views for list, details, create, edit, and delete, including validation markup and a shared details partial.
- [x] Add Previous/Next pagination controls to the index markup; controls are currently disabled.
- [ ] Add pagination to the supermarket list, including page controls and handling for empty or invalid pages.
- [ ] Check CRUD and navigation through the first, middle, and last pages.

### Products

- [x] Create Product with primary key, name, decimal price, basic validation attributes, and navigation to order details.
- [x] Create Product views for list, details, create, edit, and delete, showing product prices and validation markup, with a shared details partial.
- [x] Prepare ProductsController action skeletons and basic Create/Edit ModelState validation with temporary sample data for UI preview.
- [ ] Connect Product views to Ihor's controller actions and check submissions, validation, and navigation together.

### Order models and views

Build these against the contracts agreed with Ihor. Final checks depend on his controllers and view data.

- [x] Create Order with primary key, customer foreign key, supermarket foreign key, order date, reference navigations, and a collection of order details.
- [x] Create OrderDetails with primary key, order foreign key, product foreign key, float quantity, and reference navigations.
- [x] Add collection navigations to Customer, Supermarket, and Product.
- [ ] Review navigation properties and SQL mappings with Ihor before generating migrations.
- [x] Create OrdersIndexViewModel with an Orders collection and nullable SelectedOrder.
- [x] Create the Orders index template showing order date, customer name, and supermarket name, with an empty-list message.
- [x] Add the conditional selected-order panel with product names, quantities, prices, line totals, and total cost calculated from supplied data in the view.
- [ ] Implement selecting an order and populate SelectedOrder and its related data; it is currently null in the controller.
- [x] Create CreateOrderViewModel and OrderItemViewModel for the order form and submitted product positions.
- [x] Add basic form validation for positive customer/supermarket/product IDs and required order date, with validation message markup.
- [x] Create the initial Create Order form with customer/supermarket/product dropdowns, date, and one bound quantity input.
- [x] Implement GET Create with one empty position and sample dropdown data, a reusable PopulateCreateOptions method, and POST binding with ModelState validation and return of the submitted ViewModel.
- [ ] Complete validation of at least one position and acceptable quantity; positive-quantity validation is deliberately deferred.
- [ ] Display selected product prices and calculate line totals and the order total in the Create form; these outputs currently show placeholders. Calculate was deferred to a separate task.
- [ ] Enable adding and removing product rows, preserve all rows on POST, and keep field names and IDs correctly indexed.
- [ ] Add and validate the antiforgery token for the Create form POST.
- [ ] Verify selected values, quantity, date, validation errors, and dropdown choices survive a round trip in the browser.
- [ ] Create views or controls to edit and delete orders and to add, edit, and remove order details.
- [x] Bind readable customer, supermarket, and product names in the Create dropdowns, with IDs as submitted values.
- [ ] Complete readable relationship selection and validation feedback in the remaining order and order-detail forms.
- [ ] Connect order views to the agreed actions and check links, submissions, and validation with Ihor.

## Ihor implementation

### Database

- [ ] Integrate all five of Petro's entity models into ShoppingContext with the corresponding DbSets.
- [ ] Configure relationships, column mappings, constraints, and the agreed deletion behavior.
- [ ] After all models are integrated, create the initial migration and verify that it creates the expected SQL Server schema. Own subsequent migrations to avoid conflicting snapshots.
- [ ] Implement SampleData.cs and invoke seeding through the service provider in Program.cs. Include customers, supermarkets, products, orders, and order details; obtain Customer and Supermarket sample values from Petro.
- [ ] Ensure repeated application startup does not duplicate seed records.

### Product backend

- [ ] Implement ProductsController CRUD actions, server-side validation, and database persistence using Petro's model and view contracts.
- [ ] Check the complete Product workflow through Petro's views together.

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
