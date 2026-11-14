-- SQLite command that enables the enforcement of foreign key constraints
PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Customers (
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
);

CREATE TABLE IF NOT EXISTS SuperMarkets (
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
);

CREATE TABLE IF NOT EXISTS Products (
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	Name TEXT NOT NULL,
	-- Changed from REAL/FLOAT to NUMERIC(10, 2) to prevent floating-point rounding errors in monetary values
	Price NUMERIC(10, 2) NOT NULL CHECK (Price > 0)
);

CREATE TABLE IF NOT EXISTS Orders (
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	CustomerId INTEGER NOT NULL,
	SupermarketId INTEGER NOT NULL,
	OrderDate TEXT NOT NULL,
	FOREIGN KEY (CustomerId) REFERENCES Customers (Id) ON DELETE RESTRICT,
	FOREIGN KEY (SupermarketId) REFERENCES SuperMarkets (Id) ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS OrderDetails (
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	OrderId INTEGER NOT NULL,
	ProductId INTEGER NOT NULL,
	-- Changed from REAL/FLOAT to NUMERIC(10, 3) for exact fractional weights/quantities without floating-point inaccuracies
	Quantity NUMERIC(10, 3) NOT NULL CHECK (Quantity > 0),
	FOREIGN KEY (OrderId) REFERENCES Orders (Id) ON DELETE CASCADE,
	FOREIGN KEY (ProductId) REFERENCES Products (Id) ON DELETE RESTRICT,
	UNIQUE (OrderId, ProductId)
);