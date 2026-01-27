DROP TABLE IF EXISTS Clients;
DROP TABLE IF EXISTS Jobs;
DROP TABLE IF EXISTS Invoices;

CREATE TABLE Clients (
    ClientId INT PRIMARY KEY,
    Name TEXT,
    Email TEXT,
    Phone TEXT
);

CREATE TABLE Jobs (
    JobId INTEGER PRIMARY KEY,
    ClientId INTEGER,
    Description TEXT,
    Status TEXT,
    Price REAL,
    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId)
);

CREATE TABLE Invoices (
    InvoiceId INTEGER PRIMARY KEY,
    JobId INTEGER,
    Amount REAL,
    IssueDate DATETIME,
    IsPaid BOOLEAN,
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId)
);
