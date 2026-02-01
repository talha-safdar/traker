PRAGMA foreign_keys = OFF;

DROP TABLE IF EXISTS Clients;
DROP TABLE IF EXISTS Jobs;
DROP TABLE IF EXISTS Invoices;


CREATE TABLE Clients (
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
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP, # on add new row (auto)
    IsActive BIT DEFAULT 1
);

CREATE TABLE Jobs (
    JobId INTEGER PRIMARY KEY AUTOINCREMENT,
    ClientId INTEGER NOT NULL,
    Title TEXT,
    Description TEXT,
    Status VARCHAR(20) CHECK (Status IN ('New', 'InProgress', 'Completed', 'Invoiced', 'Paid')), # on add new row (auto)
    EstimatedPrice TEXT,
    FinalPrice TEXT,
    CreatedDate DATETIME, # on add new row (auto)
    StartDate DATETIME,
    CompletedDate DATETIME,
    DueDate DATETIME,
    FolderPath TEXT,
    Notes TEXT,
    IsArchived BOOLEAN,
    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId)
);

CREATE TABLE Invoices (
    InvoiceId INTEGER PRIMARY KEY AUTOINCREMENT,
    JobId INTEGER NOT NULL,
    InvoiceNumber TEXT,
    Subtotal TEXT,
    TaxAmount TEXT,
    TotalAmount TEXT,
    IssueDate DATETIME,
    DueDate DATETIME,
    IsPaid BOOLEAN, # on add new row (auto)
    PaidDate DATETIME,
    PaymentMethod TEXT,
    Notes TEXT,
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId)
);

INSERT INTO Clients (ClientId, Type, FullName, CompanyName, Email, PhoneNumber, BillingAddress, City, Postcode, Country, CreatedDate, IsActive) VALUES
(1, 'individual', 'John Doe', NULL, 'john.doe@example.com', '1234567890', '123 Elm St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(2, 'company', 'Jane Smith', 'Smith Enterprises', 'jane.smith@example.com', '0987654321', '456 Oak St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(3, 'individual', 'Alice Johnson', NULL, 'alice.johnson@example.com', '2345678901', '789 Pine St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(4, 'company', 'Bob Brown', 'Brown LLC', 'bob.brown@example.com', '3456789012', '321 Maple St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(5, 'individual', 'Charlie Davis', NULL, 'charlie.davis@example.com', '4567890123', '654 Cedar St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(6, 'company', 'Diana Evans', 'Evans Corp', 'diana.evans@example.com', '5678901234', '987 Birch St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(7, 'individual', 'Ethan Foster', NULL, 'ethan.foster@example.com', '6789012345', '159 Spruce St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(8, 'company', 'Fiona Green', 'Green Solutions', 'fiona.green@example.com', '7890123456', '753 Fir St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(9, 'individual', 'George Harris', NULL, 'george.harris@example.com', '8901234567', '852 Willow St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(10, 'company', 'Hannah Ives', 'Ives Industries', 'hannah.ives@example.com', '9012345678', '951 Chestnut St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(11, 'individual', 'Ian Johnson', NULL, 'ian.johnson@example.com', '1234567890', '123 Elm St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(12, 'company', 'Jack King', 'King Enterprises', 'jack.king@example.com', '0987654321', '456 Oak St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(13, 'individual', 'Laura Lee', NULL, 'laura.lee@example.com', '2345678901', '789 Pine St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(14, 'company', 'Mike Moore', 'Moore LLC', 'mike.moore@example.com', '3456789012', '321 Maple St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(15, 'individual', 'Nina Nelson', NULL, 'nina.nelson@example.com', '4567890123', '654 Cedar St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(16, 'company', 'Oscar Owens', 'Owens Corp', 'oscar.owens@example.com', '5678901234', '987 Birch St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(17, 'individual', 'Paula Parker', NULL, 'paula.parker@example.com', '6789012345', '159 Spruce St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(18, 'company', 'Quinn Reed', 'Reed Solutions', 'quinn.reed@example.com', '7890123456', '753 Fir St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(19, 'individual', 'Ryan Scott', NULL, 'ryan.scott@example.com', '8901234567', '852 Willow St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(20, 'company', 'Sara Taylor', 'Taylor Industries', 'sara.taylor@example.com', '9012345678', '951 Chestnut St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(21, 'individual', 'Tommy Upton', NULL, 'tommy.upton@example.com', '1234567890', '123 Elm St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(22, 'company', 'Uma Vance', 'Vance Enterprises', 'uma.vance@example.com', '0987654321', '456 Oak St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(23, 'individual', 'Victor White', NULL, 'victor.white@example.com', '2345678901', '789 Pine St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(24, 'company', 'Wendy Young', 'Young LLC', 'wendy.young@example.com', '3456789012', '321 Maple St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(25, 'individual', 'Xander Zane', NULL, 'xander.zane@example.com', '4567890123', '654 Cedar St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(26, 'company', 'Yara Adams', 'Adams Corp', 'yara.adams@example.com', '5678901234', '987 Birch St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(27, 'individual', 'Zoe Baker', NULL, 'zoe.baker@example.com', '6789012345', '159 Spruce St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(28, 'company', 'Aaron Clark', 'Clark Solutions', 'aaron.clark@example.com', '7890123456', '753 Fir St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(29, 'individual', 'Bella Diaz', NULL, 'bella.diaz@example.com', '8901234567', '852 Willow St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1),
(30, 'company', 'Cody Evans', 'Evans Industries', 'cody.evans@example.com', '9012345678', '951 Chestnut St', 'Springfield', '12345', 'USA', CURRENT_TIMESTAMP, 1);

INSERT INTO Jobs (JobId, ClientId, Title, Description, Status, EstimatedPrice, FinalPrice, CreatedDate, StartDate, CompletedDate, DueDate, FolderPath, Notes, IsArchived) VALUES
(1, 1, 'Website Development', 'Develop a new company website', 'Invoiced', 1545.00, 67.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-01', '/projects/website', 'Initial project setup', 0),
(2, 2, 'SEO Optimization', 'Optimize website for search engines', 'InProgress', 800.00, 44.50, CURRENT_TIMESTAMP, '2023-10-01', '2023-10-01', '2023-11-01', '/projects/seo', 'Focus on keywords', 0),
(3, 3, 'Mobile App', 'Create a mobile application', 'Completed', 2000.00, 1800.00, CURRENT_TIMESTAMP, '2023-05-01', '2023-09-01', '2023-10-01', '/projects/app', 'Final version delivered', 0),
(4, 4, 'Graphic Design', 'Design marketing materials', 'Invoiced', 500.00, 520.23, CURRENT_TIMESTAMP, '2023-06-01', '2023-06-30', '2023-07-01', '/projects/design', 'Invoice sent', 0),
(5, 5, 'Content Writing', 'Write blog posts', 'Paid', 300.00, 300.00, CURRENT_TIMESTAMP, '2023-08-01', '2023-09-01', '2023-09-15', '/projects/content', 'Payment received', 0),
(6, 6, 'Social Media Management', 'Manage social media accounts', 'InProgress', 600.00, 40.10, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-15', '/projects/social', 'Initial setup', 0),
(7, 7, 'Email Marketing', 'Create email campaigns', 'InProgress', 480.00, 0.0, CURRENT_TIMESTAMP, '2023-10-15', '2023-10-15', '2023-11-15', '/projects/email', 'Campaign in progress', 0),
(8, 8, 'E-commerce Setup', 'Set up online store', 'Completed', 2500.00, 2400.00, CURRENT_TIMESTAMP, '2023-05-15', '2023-09-15', '2023-10-01', '/projects/ecommerce', 'Final version delivered', 0),
(9, 9, 'Brand Strategy', 'Develop brand strategy', 'Invoiced', 700.00, 700.00, CURRENT_TIMESTAMP, '2023-06-15', '2023-07-10', '2023-07-15', '/projects/brand', 'Invoice sent', 0),
(10, 10, 'Video Production', 'Create promotional video', 'Paid', 1200.00, 1240.56, CURRENT_TIMESTAMP, '2023-08-15', '2023-09-15', '2023-09-30', '/projects/video', 'Payment received', 0),
(11, 11, 'Website Maintenance', 'Maintain existing website', 'Invoiced', 400.00, 0.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-20', '/projects/maintenance', 'Initial setup', 0),
(12, 12, 'Market Research', 'Conduct market research', 'InProgress', 905.00, 0.0, CURRENT_TIMESTAMP, '2023-10-20', '2023-10-20', '2023-11-20', '/projects/research', 'Research ongoing', 0),
(13, 13, 'Logo Design', 'Design a new logo', 'Completed', 300.00, 300.00, CURRENT_TIMESTAMP, '2023-05-20', '2023-09-20', '2023-10-05', '/projects/logo', 'Final version delivered', 0),
(14, 14, 'App Maintenance', 'Maintain mobile application', 'Invoiced', 500.00, 510.23, CURRENT_TIMESTAMP, '2023-06-20', '2023-07-15', '2023-07-20', '/projects/app-maintenance', 'Invoice sent', 0),
(15, 15, 'Content Strategy', 'Develop content strategy', 'Paid', 605.00, 600.00, CURRENT_TIMESTAMP, '2023-08-20', '2023-09-20', '2023-09-25', '/projects/content-strategy', 'Payment received', 0),
(16, 16, 'PPC Campaign', 'Manage pay-per-click campaigns', 'Invoiced', 800.00, 0.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-25', '/projects/ppc', 'Initial setup', 0),
(17, 17, 'Influencer Marketing', 'Collaborate with influencers', 'InProgress', 705.00, 78.30, CURRENT_TIMESTAMP, '2023-10-25', '2023-10-25', '2023-11-25', '/projects/influencer', 'Campaign ongoing', 0),
(18, 18, 'Website Audit', 'Conduct website audit', 'Completed', 400.00, 400.00, CURRENT_TIMESTAMP, '2023-05-25', '2023-09-25', '2023-10-10', '/projects/audit', 'Final version delivered', 0),
(19, 19, 'Email List Building', 'Build email list', 'Invoiced', 300.00, 300.00, CURRENT_TIMESTAMP, '2023-06-25', '2023-07-20', '2023-07-25', '/projects/email-list', 'Invoice sent', 0),
(20, 20, 'Webinar Hosting', 'Host a webinar', 'Paid', 500.00, 500.00, CURRENT_TIMESTAMP, '2023-08-25', '2023-09-25', '2023-09-30', '/projects/webinar', 'Payment received', 0),
(21, 21, 'User Testing', 'Conduct user testing', 'New', 600.00, 0.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-30', '/projects/testing', 'Initial setup', 0),
(22, 22, 'Sales Funnel Creation', 'Create sales funnel', 'InProgress', 900.00, 40.0, CURRENT_TIMESTAMP, '2023-10-30', '2023-10-30', '2023-11-30', '/projects/funnel', 'Funnel in progress', 0),
(23, 23, 'Affiliate Marketing', 'Manage affiliate program', 'Completed', 1200.00, 1240.01, CURRENT_TIMESTAMP, '2023-05-30', '2023-09-30', '2023-10-15', '/projects/affiliate', 'Final version delivered', 0),
(24, 24, 'Customer Feedback', 'Gather customer feedback', 'Invoiced', 400.00, 426.00, CURRENT_TIMESTAMP, '2023-06-30', '2023-07-25', '2023-07-30', '/projects/feedback', 'Invoice sent', 0),
(25, 25, 'Branding Workshop', 'Conduct branding workshop', 'Paid', 800.00, 800.00, CURRENT_TIMESTAMP, '2023-08-30', '2023-09-30', '2023-09-30', '/projects/workshop', 'Payment received', 0),
(26, 26, 'CRM Setup', 'Set up customer relationship management', 'New', 1050.00, 24.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-31', '/projects/crm', 'Initial setup', 0),
(27, 27, 'Data Analysis', 'Analyze customer data', 'InProgress', 500.00, 0.0, CURRENT_TIMESTAMP, '2023-10-31', '2023-10-31', '2023-11-21', '/projects/data-analysis', 'Analysis ongoing', 0),
(28, 28, 'Event Planning', 'Plan a corporate event', 'Completed', 1500.00, 1407.00, CURRENT_TIMESTAMP, '2023-05-31', '2023-09-30', '2023-10-20', '/projects/event', 'Final version delivered', 0),
(29, 29, 'Training Program', 'Develop training program', 'Invoiced', 600.00, 630.00, CURRENT_TIMESTAMP, '2023-06-25', '2023-07-25', '2023-07-31', '/projects/training', 'Invoice sent', 0),
(30, 30, 'Product Launch', 'Launch new product', 'Paid', 2000.00, 2010.00, CURRENT_TIMESTAMP, '2023-08-31', '2023-09-30', '2023-09-30', '/projects/launch', 'Payment received', 0);

INSERT INTO Invoices (InvoiceId, JobId, InvoiceNumber, Subtotal, TaxAmount, TotalAmount, IssueDate, DueDate, IsPaid, PaidDate, PaymentMethod, Notes) VALUES
(1, 1, 'INV-0001', 1500.00, 150.00, 1653.00, CURRENT_TIMESTAMP, '2027-12-01', 0, '1900-01-01', 'Credit Card', 'Initial invoice'),
(2, 2, 'INV-0002', 800.00, 80.00, 880.00, CURRENT_TIMESTAMP, '2023-11-01', 0, '1900-01-01', 'PayPal', 'Invoice for SEO services'),
(3, 3, 'INV-0003', 2000.00, 200.00, 2200.00, CURRENT_TIMESTAMP, '2027-10-01', 1, '2023-09-30', 'Bank Transfer', 'Final invoice for app development'),
(4, 4, 'INV-0004', 500.00, 50.00, 550.50, CURRENT_TIMESTAMP, '2023-07-01', 0, '1900-01-01', 'Credit Card', 'Invoice for design services'),
(5, 5, 'INV-0005', 300.00, 30.00, 330.00, CURRENT_TIMESTAMP, '2023-09-15', 1, '2023-09-15', 'PayPal', 'Invoice for content writing'),
(6, 6, 'INV-0006', 600.00, 60.00, 660.00, CURRENT_TIMESTAMP, '2027-12-15', 0, '1900-01-01', 'Credit Card', 'Initial invoice for social media management'),
(7, 7, 'INV-0007', 400.00, 40.00, 444.00, CURRENT_TIMESTAMP, '2023-11-15', 0, '1900-01-01', 'PayPal', 'Invoice for email marketing'),
(8, 8, 'INV-0008', 2500.00, 250.00, 2750.00, CURRENT_TIMESTAMP, '2023-10-01', 1, '2023-09-30', 'Bank Transfer', 'Final invoice for e-commerce setup'),
(9, 9, 'INV-0009', 700.00, 70.00, 770.00, CURRENT_TIMESTAMP, '2023-07-15', 0, '1900-01-01', 'Credit Card', 'Invoice for brand strategy'),
(10, 10, 'INV-0010', 1200.00, 120.00, 1320.00, CURRENT_TIMESTAMP, '2023-09-30', 1, '2023-09-30', 'PayPal', 'Final invoice for video production'),
(11, 11, 'INV-0011', 400.00, 40.00, 440.00, CURRENT_TIMESTAMP, '2026-12-20', 0, '1900-01-01', 'Credit Card', 'Initial invoice for website maintenance'),
(12, 12, 'INV-0012', 900.00, 90.00, 990.00, CURRENT_TIMESTAMP, '2023-11-20', 0, '1900-01-01', 'PayPal', 'Invoice for market research'),
(13, 13, 'INV-0013', 300.00, 30.00, 330.60, CURRENT_TIMESTAMP, '2026-10-05', 1, '2023-10-05', 'Bank Transfer', 'Final invoice for logo design'),
(14, 14, 'INV-0014', 500.00, 50.00, 550.20, CURRENT_TIMESTAMP, '2023-07-20', 0, '1900-01-01', 'Credit Card', 'Invoice for app maintenance'),
(15, 15, 'INV-0015', 600.00, 60.00, 660.44, CURRENT_TIMESTAMP, '2027-09-25', 1, '2023-09-25', 'PayPal', 'Final invoice for content strategy'),
(16, 16, 'INV-0016', 800.00, 80.00, 880.00, CURRENT_TIMESTAMP, '2023-12-25', 0, '1900-01-01', 'Credit Card', 'Initial invoice for PPC campaign'),
(17, 17, 'INV-0017', 700.00, 70.00, 770.75, CURRENT_TIMESTAMP, '2023-11-25', 0, '1900-01-01', 'PayPal', 'Invoice for influencer marketing'),
(18, 18, 'INV-0018', 400.00, 40.00, 444.00, CURRENT_TIMESTAMP, '2026-10-10', 1, '2023-10-10', 'Bank Transfer', 'Final invoice for website audit'),
(19, 19, 'INV-0019', 300.00, 30.00, 330.40, CURRENT_TIMESTAMP, '2023-07-30', 0, '1900-01-01', 'Credit Card', 'Invoice for customer feedback'),
(20, 20, 'INV-0020', 500.00, 50.00, 550.00, CURRENT_TIMESTAMP, '2023-09-30', 1, '2023-09-30', 'PayPal', 'Final invoice for webinar hosting'),
(21, 21, 'INV-0021', 600.00, 60.00, 660.05, CURRENT_TIMESTAMP, '2023-12-30', 0, '1900-01-01', 'Credit Card', 'Initial invoice for user testing'),
(22, 22, 'INV-0022', 900.00, 90.00, 990.30, CURRENT_TIMESTAMP, '2026-11-30', 0, '1900-01-01', 'PayPal', 'Invoice for sales funnel creation'),
(23, 23, 'INV-0023', 1200.00, 120.00, 1320.00, CURRENT_TIMESTAMP, '2023-10-15', 1, '2023-10-15', 'Bank Transfer', 'Final invoice for affiliate marketing'),
(24, 24, 'INV-0024', 400.00, 40.00, 440.80, CURRENT_TIMESTAMP, '2026-07-30', 0, '1900-01-01', 'Credit Card', 'Invoice for branding workshop'),
(25, 25, 'INV-0025', 800.00, 80.00, 880.00, CURRENT_TIMESTAMP, '2023-09-30', 1, '2023-09-30', 'PayPal', 'Final invoice for training program'),
(26, 26, 'INV-0026', 1000.00, 100.00, 1100.05, CURRENT_TIMESTAMP, '2023-12-31', 0, '1900-01-01', 'Credit Card', 'Initial invoice for CRM setup'),
(27, 27, 'INV-0027', 500.00, 50.00, 550.00, CURRENT_TIMESTAMP, '2026-11-30', 0, '1900-01-01', 'PayPal', 'Invoice for data analysis'),
(28, 28, 'INV-0028', 1500.00, 150.00, 1650.00, CURRENT_TIMESTAMP, '2023-10-20', 1, '2023-10-20', 'Bank Transfer', 'Final invoice for event planning'),
(29, 29, 'INV-0029', 600.00, 60.00, 664.00, CURRENT_TIMESTAMP, '2023-07-31', 0, '1900-01-01', 'Credit Card', 'Invoice for product launch'),
(30, 30, 'INV-0030', 2000.00, 200.00, 2200.00, CURRENT_TIMESTAMP, '2023-09-30', 1, '2023-09-30', 'PayPal', 'Final invoice for product launch');

PRAGMA foreign_keys = ON;