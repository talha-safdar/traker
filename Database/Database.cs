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
        private SqliteCommand _sqliteCommand;
        private string _connectionString;

        public Database()
        {
            _sqliteCommand = new SqliteCommand();
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
                    _sqliteCommand = conn.CreateCommand();
                    _sqliteCommand.CommandText = @"
                        DROP TABLE IF EXISTS Invoices;
                        DROP TABLE IF EXISTS Jobs;
                        DROP TABLE IF EXISTS Clients;

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

                        INSERT INTO Clients (ClientId, Name, Email, Phone) VALUES
                        (1, 'John Doe', 'john.doe@example.com', '123-456-7890'),
                        (2, 'Jane Smith', 'jane.smith@example.com', '234-567-8901'),
                        (3, 'Alice Johnson', 'alice.johnson@example.com', '345-678-9012'),
                        (4, 'Bob Brown', 'bob.brown@example.com', '456-789-0123'),
                        (5, 'Charlie Davis', 'charlie.davis@example.com', '567-890-1234'),
                        (6, 'Diana Evans', 'diana.evans@example.com', '678-901-2345'),
                        (7, 'Ethan Foster', 'ethan.foster@example.com', '789-012-3456'),
                        (8, 'Fiona Green', 'fiona.green@example.com', '890-123-4567'),
                        (9, 'George Harris', 'george.harris@example.com', '901-234-5678'),
                        (10, 'Hannah Ivers', 'hannah.ivers@example.com', '012-345-6789'),
                        (11, 'Ian Johnson', 'ian.johnson@example.com', '123-456-7891'),
                        (12, 'Julia King', 'julia.king@example.com', '234-567-8902'),
                        (13, 'Kevin Lee', 'kevin.lee@example.com', '345-678-9013'),
                        (14, 'Laura Miller', 'laura.miller@example.com', '456-789-0124'),
                        (15, 'Mike Nelson', 'mike.nelson@example.com', '567-890-1235'),
                        (16, 'Nina OConnor', 'nina.oconnor@example.com', '678-901-2346'),
                        (17, 'Oscar Parker', 'oscar.parker@example.com', '789-012-3457'),
                        (18, 'Paula Quinn', 'paula.quinn@example.com', '890-123-4568'),
                        (19, 'Quentin Roberts', 'quentin.roberts@example.com', '901-234-5679'),
                        (20, 'Rachel Smith', 'rachel.smith@example.com', '012-345-6790'),
                        (21, 'Sam Thompson', 'sam.thompson@example.com', '123-456-7892'),
                        (22, 'Tina Underwood', 'tina.underwood@example.com', '234-567-8903'),
                        (23, 'Ursula Vance', 'ursula.vance@example.com', '345-678-9014'),
                        (24, 'Victor White', 'victor.white@example.com', '456-789-0125'),
                        (25, 'Wendy Xu', 'wendy.xu@example.com', '567-890-1236'),
                        (26, 'Xander Young', 'xander.young@example.com', '678-901-2347'),
                        (27, 'Yvonne Zane', 'yvonne.zane@example.com', '789-012-3458'),
                        (28, 'Zachary Allen', 'zachary.allen@example.com', '890-123-4569'),
                        (29, 'Amy Brown', 'amy.brown@example.com', '901-234-5680'),
                        (30, 'Brian Clark', 'brian.clark@example.com', '012-345-6791');

                        INSERT INTO Jobs (JobId, ClientId, Description, Status, Price) VALUES
                        (1, 1, 'Website Development', 'Completed', 1500.00),
                        (2, 2, 'SEO Optimization', 'In Progress', 800.00),
                        (3, 3, 'Graphic Design', 'Pending', 600.00),
                        (4, 4, 'Content Writing', 'Completed', 300.00),
                        (5, 5, 'Social Media Management', 'In Progress', 1200.00),
                        (6, 6, 'Email Marketing', 'Pending', 400.00),
                        (7, 7, 'App Development', 'Completed', 2500.00),
                        (8, 8, 'Market Research', 'In Progress', 700.00),
                        (9, 9, 'Brand Strategy', 'Pending', 900.00),
                        (10, 10, 'Video Production', 'Completed', 1800.00),
                        (11, 11, 'Consulting', 'In Progress', 1000.00),
                        (12, 12, 'Data Analysis', 'Pending', 500.00),
                        (13, 13, 'Event Planning', 'Completed', 2200.00),
                        (14, 14, 'Public Relations', 'In Progress', 950.00),
                        (15, 15, 'Photography', 'Pending', 750.00),
                        (16, 16, 'Copywriting', 'Completed', 350.00),
                        (17, 17, 'Web Hosting', 'In Progress', 450.00),
                        (18, 18, 'E-commerce Setup', 'Pending', 1300.00),
                        (19, 19, 'Mobile Marketing', 'Completed', 1100.00),
                        (20, 20, 'Branding', 'In Progress', 1400.00),
                        (21, 21, 'User Experience Design', 'Pending', 800.00),
                        (22, 22, 'Software Development', 'Completed', 3000.00),
                        (23, 23, 'IT Support', 'In Progress', 600.00),
                        (24, 24, 'Network Setup', 'Pending', 900.00),
                        (25, 25, 'Cloud Services', 'Completed', 2000.00),
                        (26, 26, 'Cybersecurity', 'In Progress', 1700.00),
                        (27, 27, 'Blockchain Development', 'Pending', 2500.00),
                        (28, 28, 'AI Solutions', 'Completed', 3000.00),
                        (29, 29, 'Virtual Assistant', 'In Progress', 500.00),
                        (30, 30, 'Translation Services', 'Pending', 400.00);

                        INSERT INTO Invoices (InvoiceId, JobId, Amount, IssueDate, IsPaid) VALUES
                        (1, 1, 1500.00, '2023-10-01 12:00:00', 1),
                        (2, 2, 800.00, '2023-10-01 12:00:00', 0),
                        (3, 3, 600.00, '2023-10-01 12:00:00', 0),
                        (4, 4, 300.00, '2023-10-01 12:00:00', 1),
                        (5, 5, 1200.00, '2023-10-01 12:00:00', 0),
                        (6, 6, 400.00, '2023-10-01 12:00:00', 0),
                        (7, 7, 2500.00, '2023-10-01 12:00:00', 1),
                        (8, 8, 700.00, '2023-10-01 12:00:00', 0),
                        (9, 9, 900.00, '2023-10-01 12:00:00', 0),
                        (10, 10, 1800.00, '2023-10-01 12:00:00', 1),
                        (11, 11, 1000.00, '2023-10-01 12:00:00', 0),
                        (12, 12, 500.00, '2023-10-01 12:00:00', 0),
                        (13, 13, 2200.00, '2023-10-01 12:00:00', 1),
                        (14, 14, 950.00, '2023-10-01 12:00:00', 0),
                        (15, 15, 750.00, '2023-10-01 12:00:00', 0),
                        (16, 16, 350.00, '2023-10-01 12:00:00', 1),
                        (17, 17, 450.00, '2023-10-01 12:00:00', 0),
                        (18, 18, 1300.00, '2023-10-01 12:00:00', 0),
                        (19, 19, 1100.00, '2023-10-01 12:00:00', 1),
                        (20, 20, 1400.00, '2023-10-01 12:00:00', 0),
                        (21, 21, 800.00, '2023-10-01 12:00:00', 1),
                        (22, 22, 3000.00, '2023-10-01 12:00:00', 0),
                        (23, 23, 600.00, '2023-10-01 12:00:00', 0),
                        (24, 24, 900.00, '2023-10-01 12:00:00', 1),
                        (25, 25, 2000.00, '2023-10-01 12:00:00', 0),
                        (26, 26, 1700.00, '2023-10-01 12:00:00', 1),
                        (27, 27, 2500.00, '2023-10-01 12:00:00', 0),
                        (28, 28, 3000.00, '2023-10-01 12:00:00', 1),
                        (29, 29, 500.00, '2023-10-01 12:00:00', 0),
                        (30, 30, 400.00, '2023-10-01 12:00:00', 1);
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

        public List<string> FetchClientNames()
        {
            /*
             * pass whole row to the variable then show the selected one on the table so when search the databased
             * already have id and other values linked to the selection
             * 
             * make variables like reels to collect each row e.g. 
             * List:
             * - Row 1:
             *      [name, email, phone..]
             *- Row 2:
             *      [name, email, phone..]
             * and so on...
             */


            List<string> clientNames = new List<string>();
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                // 2. Create the command locally so it can't be "modified" by other parts of the app
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Clients";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clientNames.Add(reader.GetString(0));
                            // Debug.WriteLine(reader.GetString(0));
                        }
                    }
                }
            }
            return clientNames;
        }

        public List<Clients> FetchClientsTable()
        {
            try
            {
                List<Clients> clientList = new List<Clients>();

                using (var conn = new SqliteConnection(_connectionString))
                {
                    conn.Open();

                    // 2. Create the command locally so it can't be "modified" by other parts of the app
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Clients";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var clientId = reader["ClientId"];
                                var clientName = reader["Name"];
                                var clientEmail = reader["Email"];
                                var clientPhone = reader["Phone"];

                                if (string.IsNullOrEmpty(clientId.ToString()) == false ||
                                    string.IsNullOrEmpty(clientName.ToString()) == false ||
                                    string.IsNullOrEmpty(clientEmail.ToString()) == false ||
                                    string.IsNullOrEmpty(clientPhone.ToString()) == false)
                                {
                                    clientList.Add(new Clients
                                    {
                                        ClientId = Convert.ToInt32(clientId),
                                        Name = clientName.ToString()!,
                                        Email = clientEmail.ToString()!,
                                        Phone = clientPhone.ToString()!
                                    });
                                }

                                // Debug.WriteLine(clientId + " " + clientName + " " + clientEmail + " " + clientPhone);
                            }
                        }
                    }
                }
                return clientList;
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while fetching 'Clients' table. Please try again.\n\n{ex.Message}",
                    "Fetch Clients Tables",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Debug.WriteLine($"Fetch Clients error: {ex.Message}");
                // System.Windows.Application.Current.Shutdown(); // close application
                System.Environment.Exit(1); // kill process
                throw;
            }            
        }
        #endregion
    }
}