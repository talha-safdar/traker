CREATE TABLE Client (
    ClientId INT PRIMARY KEY IDENTITY(1,1),
    Type VARCHAR(20) NOT NULL CHECK (Type IN ('company', 'individual')),
    FullName VARCHAR(100) NOT NULL,
    CompanyName VARCHAR(100) NULL,
    Email VARCHAR(100) NULL,
    PhoneNumber VARCHAR(20) NULL,
    BillingAddress VARCHAR(255) NOT NULL,
    City VARCHAR(50) NOT NULL,
    Postcode VARCHAR(20) NOT NULL,
    Country VARCHAR(50) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    isActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Jobs (
    JobId INTEGER PRIMARY KEY,
    ClientId INTEGER NOT NULL,
    Title TEXT,
    Description TEXT,
    Status VARCHAR(20) CHECK (Status IN ('New', 'InProgress', 'Done', 'Invoiced', 'Paid')),
    EstimatedPrice REAL,
    FinalPrice REAL,
    CreatedDate DATETIME,
    StartDate DATETIME,
    CompletedDate DATETIME,
    DueDate DATETIME,
    FolderPath TEXT,
    Notes TEXT,
    IsArchived BOOLEAN,
    FOREIGN KEY (ClientId) REFERENCES Client(ClientId)
);

CREATE TABLE Invoices (
    InvoiceId INTEGER PRIMARY KEY,
    JobId INTEGER NOT NULL,
    InvoiceNumber TEXT,
    Subtotal REAL,
    TaxAmount REAL,
    TotalAmount REAL,
    IssueDate DATETIME,
    DueDate DATETIME,
    IsPaid BOOLEAN,
    PaidDate DATETIME,
    PaymentMethod TEXT,
    Notes TEXT,
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId)
);
