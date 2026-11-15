# ShoppingSystem
This document describes the application structure and serves as the main application documentation. It will be expanded later with detailed application logic and usage instructions.
## Project layers
### Data layer
The `Data` layer contains the database context and Entity Framework Core migrations.
### Database storage
The `DB` folder stores database SQL scripts and the local SQLite database file.
### Database seeder
The `DB_Seeder` layer contains the database seeder logic that creates and/or populates the database with test data. It reads seed data from the internal data pool located at `DB_Seeder/SeedData`.
### Domain layer
The `Domain` layer contains the application data models and domain entities.
### Web layer
The `ShoppingSystem` web layer contains the web application and its main application logic.
## Documentation
Sprint assignment materials, the work plan, and related images are maintained in [`SprintTasks`](SprintTasks).
