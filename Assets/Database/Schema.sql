PRAGMA foreign_keys = OFF;


CREATE TABLE IF NOT EXISTS Clients (
    ClientId INTEGER PRIMARY KEY AUTOINCREMENT,
    Type VARCHAR(20) CHECK (Type IN ('company', 'individual')),
    FullName VARCHAR(100) NOT NULL,
    CompanyName VARCHAR(100),
    Email VARCHAR(100),
    PhoneNumber VARCHAR(20),
    BillingAddress VARCHAR(255),
    City VARCHAR(50),
    Postcode VARCHAR(20),
    Country VARCHAR(50),
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsActive BIT DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Jobs (
    JobId INTEGER PRIMARY KEY AUTOINCREMENT,
    ClientId INTEGER NOT NULL,
    Title TEXT,
    Description TEXT,
    Status VARCHAR(20) CHECK (Status IN ('New', 'InProgress', 'Completed', 'Invoiced', 'Paid')),
    EstimatedPrice TEXT,
    FinalPrice TEXT,
    CreatedDate DATETIME,
    StartDate DATETIME,
    CompletedDate DATETIME,
    DueDate DATETIME,
    FolderPath TEXT,
    Notes TEXT,
    IsArchived BOOLEAN,
    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId)
);

CREATE TABLE IF NOT EXISTS Invoices (
    InvoiceId INTEGER PRIMARY KEY AUTOINCREMENT,
    JobId INTEGER NOT NULL,
    InvoiceNumber TEXT,
    Subtotal TEXT,
    TaxAmount TEXT,
    TotalAmount TEXT,
    IssueDate DATETIME,
    DueDate DATETIME,
    IsPaid BOOLEAN,
    PaidDate DATETIME,
    PaymentMethod TEXT,
    Notes TEXT,
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId)
);

PRAGMA foreign_keys = ON;