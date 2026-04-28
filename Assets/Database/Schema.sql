PRAGMA foreign_keys = OFF;


CREATE TABLE IF NOT EXISTS Clients (
    ClientId INTEGER PRIMARY KEY AUTOINCREMENT,
    Type VARCHAR(20) CHECK (Type IN ('Company', 'Individual')),
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    CompanyName VARCHAR(100),
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
    Status VARCHAR(20),
    EstimatedPrice TEXT,
    FinalPrice TEXT,
    AmountReceived TEXT,
    CreatedDate DATETIME,
    StartDate DATETIME,
    CompletedDate DATETIME,
    DueDate DATETIME,
    FolderPath TEXT,
    Notes TEXT,
    IsArchived BOOLEAN,
    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Invoices (
    InvoiceId INTEGER PRIMARY KEY AUTOINCREMENT,
    JobId INTEGER NOT NULL,
    InvoiceNumber INTEGER UNIQUE NOT NULL,
    Subtotal TEXT,
    TaxAmount INTEGER,
    TotalAmount TEXT,
    IssueDate DATETIME,
    DueDate DATETIME,
    BillingName TEXT,
    BillingAddress TEXT,
    BillingCity, TEXT,
    BillingPostcode TEXT,
    BillingCountry TEXT,
    Status VARCHAR(20), -- Overdue, Paid, Unpaid
    InvoiceName TEXT,
    IsDeleted BIT DEFAULT 0,
    PaidDate DATETIME,
    PaymentMethod TEXT,
    Notes TEXT,
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId) ON DELETE CASCADE
);

-- user info
CREATE TABLE IF NOT EXISTS User (
    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT,
    Email TEXT,
    Phone TEXT
);

-- company information
CREATE TABLE IF NOT EXISTS Business (
    BusinessId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    BusinessName TEXT,
    BusinessType VARCHAR(20) CHECK (BusinessType IN ('Company', 'Individual')),
    Country TEXT,
    City TEXT,
    Address TEXT,
    Postcode TEXT,
    VatNumber TEXT, -- for company
    RegistrationNumber TEXT, -- for company
    FOREIGN KEY (UserId) REFERENCES User(UserId) ON DELETE CASCADE
);

-- bank information
CREATE TABLE IF NOT EXISTS Bank (
    BankId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    BankName TEXT,
    AccountName TEXT,
    AccountNumber TEXT,
    SortCode TEXT,
    IBAN TEXT,
    BIC TEXT,
    FOREIGN KEY (UserId) REFERENCES User(UserId) ON DELETE CASCADE
);

PRAGMA foreign_keys = ON;