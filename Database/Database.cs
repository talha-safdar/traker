using Caliburn.Micro;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Traker.Models;

namespace Traker.Database
{
    public class Database
    {
        private string _connectionString;

        public Database()
        {
            _connectionString = String.Empty;
        }

        #region Public Static Functions
        public async Task SetUpDatabase()
        {
            await Task.Run(() =>
            {
                try
                {
                    // set directory and database name
                    var folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Traker");
                    Directory.CreateDirectory(folder);
                    var dbPath = Path.Combine(folder, "traker.db");

                    // creeate database
                    _connectionString = $"Data Source={dbPath}";
                    using var conn = new SqliteConnection(_connectionString);
                    conn.Open();
                    var _sqliteCommand = conn.CreateCommand();
                    _sqliteCommand.CommandText = @"




PRAGMA foreign_keys = OFF;

DROP TABLE IF EXISTS Client;
DROP TABLE IF EXISTS Job;
DROP TABLE IF EXISTS Invoice;


CREATE TABLE Client (
    ClientId INT PRIMARY KEY,
    Type VARCHAR(20) NOT NULL CHECK (Type IN ('company', 'individual')),
    FullName VARCHAR(100) NOT NULL,
    CompanyName VARCHAR(100) NULL,
    Email VARCHAR(100) NULL,
    PhoneNumber VARCHAR(20) NULL,
    BillingAddress VARCHAR(255),
    City VARCHAR(50),
    Postcode VARCHAR(20),
    Country VARCHAR(50),
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsActive BIT NOT NULL DEFAULT 1
);

INSERT INTO Client (ClientId, Type, FullName, CompanyName, Email, PhoneNumber, BillingAddress, City, Postcode, Country, CreatedDate, IsActive) VALUES
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

CREATE TABLE Job (
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

INSERT INTO Job (JobId, ClientId, Title, Description, Status, EstimatedPrice, FinalPrice, CreatedDate, StartDate, CompletedDate, DueDate, FolderPath, Notes, IsArchived) VALUES
(1, 1, 'Website Development', 'Develop a new company website', 'New', 1500.00, 0.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-01', '/projects/website', 'Initial project setup', 0),
(2, 2, 'SEO Optimization', 'Optimize website for search engines', 'InProgress', 800.00, 0.0, CURRENT_TIMESTAMP, '2023-10-01', '2023-10-01', '2023-11-01', '/projects/seo', 'Focus on keywords', 0),
(3, 3, 'Mobile App', 'Create a mobile application', 'Done', 2000.00, 1800.00, CURRENT_TIMESTAMP, '2023-05-01', '2023-09-01', '2023-10-01', '/projects/app', 'Final version delivered', 0),
(4, 4, 'Graphic Design', 'Design marketing materials', 'Invoiced', 500.00, 500.00, CURRENT_TIMESTAMP, '2023-06-01', '2023-06-30', '2023-07-01', '/projects/design', 'Invoice sent', 0),
(5, 5, 'Content Writing', 'Write blog posts', 'Paid', 300.00, 300.00, CURRENT_TIMESTAMP, '2023-08-01', '2023-09-01', '2023-09-15', '/projects/content', 'Payment received', 0),
(6, 6, 'Social Media Management', 'Manage social media accounts', 'New', 600.00, 0.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-15', '/projects/social', 'Initial setup', 0),
(7, 7, 'Email Marketing', 'Create email campaigns', 'InProgress', 400.00, 0.0, CURRENT_TIMESTAMP, '2023-10-15', '2023-10-15', '2023-11-15', '/projects/email', 'Campaign in progress', 0),
(8, 8, 'E-commerce Setup', 'Set up online store', 'Done', 2500.00, 2400.00, CURRENT_TIMESTAMP, '2023-05-15', '2023-09-15', '2023-10-01', '/projects/ecommerce', 'Final version delivered', 0),
(9, 9, 'Brand Strategy', 'Develop brand strategy', 'Invoiced', 700.00, 700.00, CURRENT_TIMESTAMP, '2023-06-15', '2023-07-10', '2023-07-15', '/projects/brand', 'Invoice sent', 0),
(10, 10, 'Video Production', 'Create promotional video', 'Paid', 1200.00, 1200.00, CURRENT_TIMESTAMP, '2023-08-15', '2023-09-15', '2023-09-30', '/projects/video', 'Payment received', 0),
(11, 11, 'Website Maintenance', 'Maintain existing website', 'New', 400.00, 0.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-20', '/projects/maintenance', 'Initial setup', 0),
(12, 12, 'Market Research', 'Conduct market research', 'InProgress', 900.00, 0.0, CURRENT_TIMESTAMP, '2023-10-20', '2023-10-20', '2023-11-20', '/projects/research', 'Research ongoing', 0),
(13, 13, 'Logo Design', 'Design a new logo', 'Done', 300.00, 300.00, CURRENT_TIMESTAMP, '2023-05-20', '2023-09-20', '2023-10-05', '/projects/logo', 'Final version delivered', 0),
(14, 14, 'App Maintenance', 'Maintain mobile application', 'Invoiced', 500.00, 500.00, CURRENT_TIMESTAMP, '2023-06-20', '2023-07-15', '2023-07-20', '/projects/app-maintenance', 'Invoice sent', 0),
(15, 15, 'Content Strategy', 'Develop content strategy', 'Paid', 600.00, 600.00, CURRENT_TIMESTAMP, '2023-08-20', '2023-09-20', '2023-09-25', '/projects/content-strategy', 'Payment received', 0),
(16, 16, 'PPC Campaign', 'Manage pay-per-click campaigns', 'New', 800.00, 0.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-25', '/projects/ppc', 'Initial setup', 0),
(17, 17, 'Influencer Marketing', 'Collaborate with influencers', 'InProgress', 700.00, 0.0, CURRENT_TIMESTAMP, '2023-10-25', '2023-10-25', '2023-11-25', '/projects/influencer', 'Campaign ongoing', 0),
(18, 18, 'Website Audit', 'Conduct website audit', 'Done', 400.00, 400.00, CURRENT_TIMESTAMP, '2023-05-25', '2023-09-25', '2023-10-10', '/projects/audit', 'Final version delivered', 0),
(19, 19, 'Email List Building', 'Build email list', 'Invoiced', 300.00, 300.00, CURRENT_TIMESTAMP, '2023-06-25', '2023-07-20', '2023-07-25', '/projects/email-list', 'Invoice sent', 0),
(20, 20, 'Webinar Hosting', 'Host a webinar', 'Paid', 500.00, 500.00, CURRENT_TIMESTAMP, '2023-08-25', '2023-09-25', '2023-09-30', '/projects/webinar', 'Payment received', 0),
(21, 21, 'User Testing', 'Conduct user testing', 'New', 600.00, 0.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-30', '/projects/testing', 'Initial setup', 0),
(22, 22, 'Sales Funnel Creation', 'Create sales funnel', 'InProgress', 900.00, 0.0, CURRENT_TIMESTAMP, '2023-10-30', '2023-10-30', '2023-11-30', '/projects/funnel', 'Funnel in progress', 0),
(23, 23, 'Affiliate Marketing', 'Manage affiliate program', 'Done', 1200.00, 1200.00, CURRENT_TIMESTAMP, '2023-05-30', '2023-09-30', '2023-10-15', '/projects/affiliate', 'Final version delivered', 0),
(24, 24, 'Customer Feedback', 'Gather customer feedback', 'Invoiced', 400.00, 400.00, CURRENT_TIMESTAMP, '2023-06-30', '2023-07-25', '2023-07-30', '/projects/feedback', 'Invoice sent', 0),
(25, 25, 'Branding Workshop', 'Conduct branding workshop', 'Paid', 800.00, 800.00, CURRENT_TIMESTAMP, '2023-08-30', '2023-09-30', '2023-09-30', '/projects/workshop', 'Payment received', 0),
(26, 26, 'CRM Setup', 'Set up customer relationship management', 'New', 1000.00, 0.0, CURRENT_TIMESTAMP, '2023-01-01', '2023-01-01', '2023-12-31', '/projects/crm', 'Initial setup', 0),
(27, 27, 'Data Analysis', 'Analyze customer data', 'InProgress', 500.00, 0.0, CURRENT_TIMESTAMP, '2023-10-31', '2023-10-31', '2023-11-30', '/projects/data-analysis', 'Analysis ongoing', 0),
(28, 28, 'Event Planning', 'Plan a corporate event', 'Done', 1500.00, 1400.00, CURRENT_TIMESTAMP, '2023-05-31', '2023-09-30', '2023-10-20', '/projects/event', 'Final version delivered', 0),
(29, 29, 'Training Program', 'Develop training program', 'Invoiced', 600.00, 600.00, CURRENT_TIMESTAMP, '2023-06-25', '2023-07-25', '2023-07-31', '/projects/training', 'Invoice sent', 0),
(30, 30, 'Product Launch', 'Launch new product', 'Paid', 2000.00, 2000.00, CURRENT_TIMESTAMP, '2023-08-31', '2023-09-30', '2023-11-30', '/projects/launch', 'Payment received', 0);

CREATE TABLE Invoice (
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
    FOREIGN KEY (JobId) REFERENCES Job(JobId)
);

INSERT INTO Invoice (InvoiceId, JobId, InvoiceNumber, Subtotal, TaxAmount, TotalAmount, IssueDate, DueDate, IsPaid, PaidDate, PaymentMethod, Notes) VALUES
(1, 1, 'INV-0001', 1500.00, 150.00, 1650.00, CURRENT_TIMESTAMP, '2027-12-01', 0, '1900-01-01', 'Credit Card', 'Initial invoice'),
(2, 2, 'INV-0002', 800.00, 80.00, 880.00, CURRENT_TIMESTAMP, '2023-11-01', 0, '1900-01-01', 'PayPal', 'Invoice for SEO services'),
(3, 3, 'INV-0003', 2000.00, 200.00, 2200.00, CURRENT_TIMESTAMP, '2027-10-01', 1, '2023-09-30', 'Bank Transfer', 'Final invoice for app development'),
(4, 4, 'INV-0004', 500.00, 50.00, 550.00, CURRENT_TIMESTAMP, '2023-07-01', 0, '1900-01-01', 'Credit Card', 'Invoice for design services'),
(5, 5, 'INV-0005', 300.00, 30.00, 330.00, CURRENT_TIMESTAMP, '2023-09-15', 1, '2023-09-15', 'PayPal', 'Invoice for content writing'),
(6, 6, 'INV-0006', 600.00, 60.00, 660.00, CURRENT_TIMESTAMP, '2027-12-15', 0, '1900-01-01', 'Credit Card', 'Initial invoice for social media management'),
(7, 7, 'INV-0007', 400.00, 40.00, 440.00, CURRENT_TIMESTAMP, '2023-11-15', 0, '1900-01-01', 'PayPal', 'Invoice for email marketing'),
(8, 8, 'INV-0008', 2500.00, 250.00, 2750.00, CURRENT_TIMESTAMP, '2023-10-01', 1, '2023-09-30', 'Bank Transfer', 'Final invoice for e-commerce setup'),
(9, 9, 'INV-0009', 700.00, 70.00, 770.00, CURRENT_TIMESTAMP, '2023-07-15', 0, '1900-01-01', 'Credit Card', 'Invoice for brand strategy'),
(10, 10, 'INV-0010', 1200.00, 120.00, 1320.00, CURRENT_TIMESTAMP, '2023-09-30', 1, '2023-09-30', 'PayPal', 'Final invoice for video production'),
(11, 11, 'INV-0011', 400.00, 40.00, 440.00, CURRENT_TIMESTAMP, '2026-12-20', 0, '1900-01-01', 'Credit Card', 'Initial invoice for website maintenance'),
(12, 12, 'INV-0012', 900.00, 90.00, 990.00, CURRENT_TIMESTAMP, '2023-11-20', 0, '1900-01-01', 'PayPal', 'Invoice for market research'),
(13, 13, 'INV-0013', 300.00, 30.00, 330.00, CURRENT_TIMESTAMP, '2026-10-05', 1, '2023-10-05', 'Bank Transfer', 'Final invoice for logo design'),
(14, 14, 'INV-0014', 500.00, 50.00, 550.00, CURRENT_TIMESTAMP, '2023-07-20', 0, '1900-01-01', 'Credit Card', 'Invoice for app maintenance'),
(15, 15, 'INV-0015', 600.00, 60.00, 660.00, CURRENT_TIMESTAMP, '2027-09-25', 1, '2023-09-25', 'PayPal', 'Final invoice for content strategy'),
(16, 16, 'INV-0016', 800.00, 80.00, 880.00, CURRENT_TIMESTAMP, '2023-12-25', 0, '1900-01-01', 'Credit Card', 'Initial invoice for PPC campaign'),
(17, 17, 'INV-0017', 700.00, 70.00, 770.00, CURRENT_TIMESTAMP, '2023-11-25', 0, '1900-01-01', 'PayPal', 'Invoice for influencer marketing'),
(18, 18, 'INV-0018', 400.00, 40.00, 440.00, CURRENT_TIMESTAMP, '2026-10-10', 1, '2023-10-10', 'Bank Transfer', 'Final invoice for website audit'),
(19, 19, 'INV-0019', 300.00, 30.00, 330.00, CURRENT_TIMESTAMP, '2023-07-30', 0, '1900-01-01', 'Credit Card', 'Invoice for customer feedback'),
(20, 20, 'INV-0020', 500.00, 50.00, 550.00, CURRENT_TIMESTAMP, '2023-09-30', 1, '2023-09-30', 'PayPal', 'Final invoice for webinar hosting'),
(21, 21, 'INV-0021', 600.00, 60.00, 660.00, CURRENT_TIMESTAMP, '2023-12-30', 0, '1900-01-01', 'Credit Card', 'Initial invoice for user testing'),
(22, 22, 'INV-0022', 900.00, 90.00, 990.00, CURRENT_TIMESTAMP, '2026-11-30', 0, '1900-01-01', 'PayPal', 'Invoice for sales funnel creation'),
(23, 23, 'INV-0023', 1200.00, 120.00, 1320.00, CURRENT_TIMESTAMP, '2023-10-15', 1, '2023-10-15', 'Bank Transfer', 'Final invoice for affiliate marketing'),
(24, 24, 'INV-0024', 400.00, 40.00, 440.00, CURRENT_TIMESTAMP, '2026-07-30', 0, '1900-01-01', 'Credit Card', 'Invoice for branding workshop'),
(25, 25, 'INV-0025', 800.00, 80.00, 880.00, CURRENT_TIMESTAMP, '2023-09-30', 1, '2023-09-30', 'PayPal', 'Final invoice for training program'),
(26, 26, 'INV-0026', 1000.00, 100.00, 1100.00, CURRENT_TIMESTAMP, '2023-12-31', 0, '1900-01-01', 'Credit Card', 'Initial invoice for CRM setup'),
(27, 27, 'INV-0027', 500.00, 50.00, 550.00, CURRENT_TIMESTAMP, '2026-11-30', 0, '1900-01-01', 'PayPal', 'Invoice for data analysis'),
(28, 28, 'INV-0028', 1500.00, 150.00, 1650.00, CURRENT_TIMESTAMP, '2023-10-20', 1, '2023-10-20', 'Bank Transfer', 'Final invoice for event planning'),
(29, 29, 'INV-0029', 600.00, 60.00, 660.00, CURRENT_TIMESTAMP, '2023-07-31', 0, '1900-01-01', 'Credit Card', 'Invoice for product launch'),
(30, 30, 'INV-0030', 2000.00, 200.00, 2200.00, CURRENT_TIMESTAMP, '2023-09-30', 1, '2023-09-30', 'PayPal', 'Final invoice for product launch');

PRAGMA foreign_keys = ON;




                        ";

                    int rows = _sqliteCommand.ExecuteNonQuery();

                    if (rows >= 0)
                    {
                        Debug.WriteLine($"Database Created");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(
                        $"An error occurred while setting up the database. Please try again.\n\n{ex.Message}", 
                        "Database Setup", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);

                    Debug.WriteLine($"Database setup error: {ex.Message}");
                }
            });
        }

        //public List<string> FetchClientNames()
        //{
        //    /*
        //     * pass whole row to the variable then show the selected one on the table so when search the databased
        //     * already have id and other values linked to the selection
        //     * 
        //     * make variables like reels to collect each row e.g. 
        //     * List:
        //     * - Row 1:
        //     *      [name, email, phone..]
        //     *- Row 2:
        //     *      [name, email, phone..]
        //     * and so on...
        //     */


        //    List<string> clientNames = new List<string>();
        //    using (var conn = new SqliteConnection(_connectionString))
        //    {
        //        conn.Open();

        //        // 2. Create the command locally so it can't be "modified" by other parts of the app
        //        using (var cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = "SELECT Name FROM Clients";

        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    clientNames.Add(reader.GetString(0));
        //                    // Debug.WriteLine(reader.GetString(0));
        //                }
        //            }
        //        }
        //    }
        //    return clientNames;
        //}

        public List<ClientModel> FetchClientsTable()
        {
            List<ClientModel> clientsList = new List<ClientModel>();

            try
            {
                using (var conn = new SqliteConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Client";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var ClientId = reader["ClientId"];
                                var Type = reader["Type"];
                                var FullName = reader["FullName"];
                                var CompanyName = reader["CompanyName"];
                                var Email = reader["Email"];
                                var PhoneNumber = reader["PhoneNumber"];
                                var BillingAddress = reader["BillingAddress"];
                                var City = reader["City"];
                                var Postcode = reader["Postcode"];
                                var Country = reader["Country"];
                                var CreatedDate = reader["CreatedDate"];
                                var IsActive = reader["IsActive"];

                                if (string.IsNullOrEmpty(ClientId.ToString()) == false ||
                                    string.IsNullOrEmpty(Type.ToString()) == false ||
                                    string.IsNullOrEmpty(FullName.ToString()) == false ||
                                    string.IsNullOrEmpty(CompanyName.ToString()) == false ||
                                    string.IsNullOrEmpty(Email.ToString()) == false ||
                                    string.IsNullOrEmpty(PhoneNumber.ToString()) == false ||
                                    string.IsNullOrEmpty(BillingAddress.ToString()) == false ||
                                    string.IsNullOrEmpty(City.ToString()) == false ||
                                    string.IsNullOrEmpty(Postcode.ToString()) == false ||
                                    string.IsNullOrEmpty(Country.ToString()) == false ||
                                    string.IsNullOrEmpty(CreatedDate.ToString()) == false ||
                                    string.IsNullOrEmpty(IsActive.ToString()) == false)
                                {
                                    clientsList.Add(new ClientModel
                                    {
                                        ClientId = Convert.ToInt32(ClientId),
                                        Type = Type.ToString()!,
                                        FullName = FullName.ToString()!,
                                        CompanyName = CompanyName.ToString()!,
                                        Email = Email.ToString()!,
                                        PhoneNumber = PhoneNumber.ToString()!,
                                        BillingAddress = BillingAddress.ToString()!,
                                        City = City.ToString()!,
                                        Postcode = Postcode.ToString()!,
                                        Country = Country.ToString()!,
                                        CreatedDate = Convert.ToDateTime(CreatedDate)!,
                                        IsActive = Convert.ToBoolean(IsActive)
                                    });
                                }
                                // Debug.WriteLine(clientId + " " + clientName + " " + clientEmail + " " + clientPhone);
                            }
                        }
                    }
                }
                return clientsList;                
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while fetching 'Client' table. Please try again.\n\n{ex.Message}",
                    "Fetch Client Tables",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Debug.WriteLine($"Fetch Client error: {ex.Message}");
                System.Environment.Exit(1); // kill process
                throw; // necessary otherwsie cries about no return value
            }           
        }

        public List<JobModel> FetchJobsTable()
        {
            List<JobModel> jobsList = new List<JobModel>();

            try
            {

                using (var conn = new SqliteConnection(_connectionString))
                {
                    conn.Open();

                    // 2. Create the command locally so it can't be "modified" by other parts of the app
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Job";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var JobId = reader["JobId"];
                                var ClientId = reader["ClientId"];
                                var Title = reader["Title"];
                                var Description = reader["Description"];
                                var Status = reader["Status"];
                                var EstimatedPrice = reader["EstimatedPrice"];
                                var FinalPrice = reader["FinalPrice"];
                                var CreatedDate = reader["CreatedDate"];
                                var StartDate = reader["StartDate"];
                                var CompletedDate = reader["CompletedDate"];
                                var DueDate = reader["DueDate"];
                                var FolderPath = reader["FolderPath"];
                                var Notes = reader["Notes"];
                                var IsArchived = reader["IsArchived"];

                                if (string.IsNullOrEmpty(JobId.ToString()) == false ||
                                    string.IsNullOrEmpty(ClientId.ToString()) == false ||
                                    string.IsNullOrEmpty(Title.ToString()) == false ||
                                    string.IsNullOrEmpty(Description.ToString()) == false ||
                                    string.IsNullOrEmpty(Status.ToString()) == false ||
                                    string.IsNullOrEmpty(EstimatedPrice.ToString()) == false ||
                                    string.IsNullOrEmpty(FinalPrice.ToString()) == false ||
                                    string.IsNullOrEmpty(CreatedDate.ToString()) == false ||
                                    string.IsNullOrEmpty(StartDate.ToString()) == false ||
                                    string.IsNullOrEmpty(CompletedDate.ToString()) == false ||
                                    string.IsNullOrEmpty(DueDate.ToString()) == false ||
                                    string.IsNullOrEmpty(FolderPath.ToString()) == false ||
                                    string.IsNullOrEmpty(Notes.ToString()) == false ||
                                    string.IsNullOrEmpty(IsArchived.ToString()) == false)
                                {
                                    jobsList.Add(new JobModel
                                    {
                                        JobId = Convert.ToInt32(JobId),
                                        ClientId = Convert.ToInt32(ClientId),
                                        Title = Title.ToString()!,
                                        Description = Description.ToString()!,
                                        Status = Status.ToString()!,
                                        EstimatedPrice = Convert.ToDecimal(EstimatedPrice),
                                        FinalPrice = Convert.ToDecimal(FinalPrice),
                                        CreatedDate = Convert.ToDateTime(CreatedDate),
                                        StartDate = Convert.ToDateTime(StartDate),
                                        CompletedDate = Convert.ToDateTime(CompletedDate),
                                        DueDate = Convert.ToDateTime(DueDate),
                                        FolderPath = FolderPath.ToString()!,
                                        Notes = Notes.ToString()!,
                                        IsArchived = Convert.ToBoolean(IsArchived),
                                    });
                                }
                                // Debug.WriteLine(clientId + " " + clientName + " " + clientEmail + " " + clientPhone);
                            }
                        }
                    }
                }
                return jobsList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while fetching 'Job' table. Please try again.\n\n{ex.Message}",
                    "Fetch Job Tables",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Debug.WriteLine($"Fetch Job error: {ex.Message}");
                System.Environment.Exit(1); // kill process
                throw; // necessary otherwsie cries about no return value
            }
        }

        public List<InvoiceModel> FetchInvoiceTable()
        {
            List<InvoiceModel> invoicesList = new List<InvoiceModel>();

            try
            {

                using (var conn = new SqliteConnection(_connectionString))
                {
                    conn.Open();

                    // 2. Create the command locally so it can't be "modified" by other parts of the app
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Invoice";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var InvoiceId = reader["InvoiceId"];
                                var JobId = reader["JobId"];
                                var InvoiceNumber = reader["InvoiceNumber"];
                                var Subtotal = reader["Subtotal"];
                                var TaxAmount = reader["TaxAmount"];
                                var TotalAmount = reader["TotalAmount"];
                                var IssueDate = reader["IssueDate"];
                                var DueDate = reader["DueDate"];
                                var IsPaid = reader["IsPaid"];
                                var PaidDate = reader["PaidDate"];
                                var PaymentMethod = reader["PaymentMethod"];
                                var Notes = reader["Notes"];

                                if (string.IsNullOrEmpty(InvoiceId.ToString()) == false ||
                                    string.IsNullOrEmpty(JobId.ToString()) == false ||
                                    string.IsNullOrEmpty(InvoiceNumber.ToString()) == false ||
                                    string.IsNullOrEmpty(Subtotal.ToString()) == false ||
                                    string.IsNullOrEmpty(TaxAmount.ToString()) == false ||
                                    string.IsNullOrEmpty(TotalAmount.ToString()) == false ||
                                    string.IsNullOrEmpty(IssueDate.ToString()) == false || 
                                    string.IsNullOrEmpty(DueDate.ToString()) == false ||
                                    string.IsNullOrEmpty(IsPaid.ToString()) == false ||
                                    string.IsNullOrEmpty(PaidDate.ToString()) == false ||
                                    string.IsNullOrEmpty(PaymentMethod.ToString()) == false ||
                                    string.IsNullOrEmpty(Notes.ToString()) == false)
                                {
                                    invoicesList.Add(new InvoiceModel
                                    {
                                        InvoiceId = Convert.ToInt32(InvoiceId),
                                        JobId = Convert.ToInt32(JobId),
                                        InvoiceNumber = InvoiceNumber.ToString()!,
                                        Subtotal = Convert.ToDecimal(Subtotal),
                                        TaxAmount = Convert.ToDecimal(TaxAmount),
                                        TotalAmount = Convert.ToDecimal(TotalAmount),
                                        IssueDate = Convert.ToDateTime(IssueDate),
                                        DueDate = Convert.ToDateTime(DueDate),
                                        IsPaid = Convert.ToBoolean(IsPaid),
                                        PaidDate = Convert.ToDateTime(PaidDate),
                                        PaymentMethod = PaymentMethod.ToString()!,
                                        Notes = Notes.ToString()!,
                                    });
                                }
                                // Debug.WriteLine(clientId + " " + clientName + " " + clientEmail + " " + clientPhone);
                            }
                        }
                    }
                }
                return invoicesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while fetching 'Invoice' table. Please try again.\n\n{ex.Message}",
                    "Fetch Invoice Tables",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Debug.WriteLine($"Fetch Jobs error: {ex.Message}");
                System.Environment.Exit(1); // kill process
                throw; // necessary otherwsie cries about no return value
            }
        }
        #endregion
    }
}