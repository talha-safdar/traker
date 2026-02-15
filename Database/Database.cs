using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using Traker.Models;
using Traker.States;

namespace Traker.Database
{
    public static class Database
    {
        private static string _connectionString = String.Empty; // location of the database file

        #region Public Functions
        public static async Task SetUpDatabase()
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
    Status VARCHAR(20),
    EstimatedPrice TEXT,
    FinalPrice TEXT,
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
    InvoiceNumber TEXT,
    Subtotal TEXT,
    TaxAmount TEXT,
    TotalAmount TEXT,
    IssueDate DATETIME,
    DueDate DATETIME,
    PaidDate DATETIME,
    PaymentMethod TEXT,
    Status VARCHAR(20),
    Notes TEXT,
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId) ON DELETE CASCADE
);

PRAGMA foreign_keys = ON;


                        ";

                    int rows = _sqliteCommand.ExecuteNonQuery();

                    if (rows >= 0)
                    {
                        Debug.WriteLine($"Database Created");
                    }
                }
                catch (Exception ex)
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

        public static List<ClientsModel> FetchClientsTable()
        {
            List<ClientsModel> clientsList = new List<ClientsModel>();

            try
            {
                using (var conn = new SqliteConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Clients";

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
                                    clientsList.Add(new ClientsModel
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
            catch (Exception ex)
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

        public static List<JobsModel> FetchJobsTable()
        {
            List<JobsModel> jobsList = new List<JobsModel>();

            try
            {

                using (var conn = new SqliteConnection(_connectionString))
                {
                    conn.Open();

                    // 2. Create the command locally so it can't be "modified" by other parts of the app
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Jobs";

                        using (var reader = cmd.ExecuteReader())
                        {




                            while (reader.Read())
                            {
                                //Debug.WriteLine(reader["JobId"] + " "
                                //    + reader["ClientId"] + " "
                                //    + reader["Title"] + " "
                                //    + reader["Description"] + " "
                                //    + reader["Status"] + " "
                                //    + reader["EstimatedPrice"] + " "
                                //    + reader["FinalPrice"] + " "
                                //    + reader["CreatedDate"] + " "
                                //    + reader["StartDate"] + " "
                                //    + reader["CompletedDate"] + " "
                                //    + reader["DueDate"] + " "
                                //    + reader["FolderPath"] + " "
                                //    + reader["Notes"] + " "
                                //    + reader["IsArchived"] + " "

                                //    );


                                var JobId = reader["JobId"];
                                var ClientId = reader["ClientId"];
                                var Title = reader["Title"] == DBNull.Value ? String.Empty : reader["Title"];
                                var Description = reader["Description"] == DBNull.Value ? String.Empty : reader["Description"];
                                var Status = reader["Status"] == DBNull.Value ? String.Empty : reader["Status"];
                                var EstimatedPrice = reader["EstimatedPrice"] == DBNull.Value ? "0" : reader["EstimatedPrice"];
                                var FinalPrice = reader["FinalPrice"] == DBNull.Value ? "0" : reader["FinalPrice"];
                                var CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.MinValue : reader["CreatedDate"];
                                var StartDate = reader["StartDate"] == DBNull.Value ? DateTime.MinValue : reader["StartDate"];
                                var CompletedDate = reader["CompletedDate"] == DBNull.Value ? DateTime.MinValue : reader["CompletedDate"];
                                var DueDate = reader["DueDate"] == DBNull.Value ? DateTime.MinValue : reader["DueDate"]; ;
                                var FolderPath = reader["FolderPath"] == DBNull.Value ? String.Empty : reader["FolderPath"];
                                var Notes = reader["Notes"] == DBNull.Value ? String.Empty : reader["Notes"];
                                var IsArchived = reader["IsArchived"] == DBNull.Value ? false : reader["IsArchived"];

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
                                    jobsList.Add(new JobsModel
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

        public static List<InvoicesModel> FetchInvoiceTable()
        {
            List<InvoicesModel> invoicesList = new List<InvoicesModel>();

            try
            {

                using (var conn = new SqliteConnection(_connectionString))
                {
                    conn.Open();

                    // 2. Create the command locally so it can't be "modified" by other parts of the app
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Invoices";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Debug.WriteLine(reader["InvoiceId"] + " | "
                                //+ reader["JobId"] + " | "
                                //+ reader["InvoiceNumber"] + " | "
                                //+ reader["Subtotal"] + " | "
                                //+ reader["TaxAmount"] + " | "
                                //+ reader["TotalAmount"] + " | "
                                //+ reader["IssueDate"] + " | "
                                //+ reader["DueDate"] + " | "
                                //+ reader["IsPaid"] + " | "
                                //+ reader["PaidDate"] + " | "
                                //+ reader["PaymentMethod"] + " | "
                                //+ reader["Notes"] + " | "

                                //    );

                                var InvoiceId = reader["InvoiceId"];
                                var JobId = reader["JobId"];
                                var InvoiceNumber = reader["InvoiceNumber"] == DBNull.Value ? String.Empty : reader["InvoiceNumber"];
                                var Subtotal = reader["Subtotal"] == DBNull.Value ? "0" : reader["Subtotal"];
                                var TaxAmount = reader["TaxAmount"] == DBNull.Value ? "0" : reader["TaxAmount"];
                                var TotalAmount = reader["TotalAmount"] == DBNull.Value ? "0" : reader["TotalAmount"];
                                var IssueDate = reader["IssueDate"] == DBNull.Value ? DateTime.MinValue : reader["IssueDate"];
                                var DueDate = reader["DueDate"] == DBNull.Value ? DateTime.MinValue : reader["DueDate"];
                                var Status = reader["Status"] == DBNull.Value ? String.Empty : reader["Status"];
                                var PaidDate = reader["PaidDate"] == DBNull.Value ? DateTime.MinValue : reader["PaidDate"];
                                var PaymentMethod = reader["PaymentMethod"] == DBNull.Value ? String.Empty : reader["PaymentMethod"];
                                var Notes = reader["Notes"] == DBNull.Value ? String.Empty : reader["Notes"];

                                if (string.IsNullOrEmpty(InvoiceId.ToString()) == false ||
                                    string.IsNullOrEmpty(JobId.ToString()) == false ||
                                    string.IsNullOrEmpty(InvoiceNumber.ToString()) == false ||
                                    string.IsNullOrEmpty(Subtotal.ToString()) == false ||
                                    string.IsNullOrEmpty(TaxAmount.ToString()) == false ||
                                    string.IsNullOrEmpty(TotalAmount.ToString()) == false ||
                                    string.IsNullOrEmpty(IssueDate.ToString()) == false ||
                                    string.IsNullOrEmpty(DueDate.ToString()) == false ||
                                    string.IsNullOrEmpty(Status.ToString()) == false ||
                                    string.IsNullOrEmpty(PaidDate.ToString()) == false ||
                                    string.IsNullOrEmpty(PaymentMethod.ToString()) == false ||
                                    string.IsNullOrEmpty(Notes.ToString()) == false)
                                {
                                    invoicesList.Add(new InvoicesModel
                                    {
                                        InvoiceId = Convert.ToInt32(InvoiceId),
                                        JobId = Convert.ToInt32(JobId),
                                        InvoiceNumber = InvoiceNumber.ToString()!,
                                        Subtotal = Convert.ToDecimal(Subtotal),
                                        TaxAmount = Convert.ToDecimal(TaxAmount),
                                        TotalAmount = Convert.ToDecimal(TotalAmount),
                                        IssueDate = Convert.ToDateTime(IssueDate),
                                        DueDate = Convert.ToDateTime(DueDate),
                                        Status = Status.ToString()!,
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

        public static Task AddRow(string clientName, string jobDescription, decimal finalPrice, DateTime dueDate)
        {
            // use this to check
            /*
             * if (clientId <= 0)
                throw new Exception("Client not saved / not found");

             */

            long clientId = 0;
            long jobId = 0;

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using (var pragma = conn.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            // work all at once, if one query fails it rolls back
            // if you don't care about failures you can avoid transaction
            using var tx = conn.BeginTransaction();


            // insert into Clients table
            using (var clienstCmd = conn.CreateCommand())
            {
                clienstCmd.CommandText = @"

                INSERT INTO Clients
                (FullName, CreatedDate)

                VALUES
                (@clientName, @createdDate);

                ";
                clienstCmd.Parameters.AddWithValue("@clientName", clientName);
                clienstCmd.Parameters.AddWithValue("@createdDate", DateTime.Now.Date);
                clienstCmd.ExecuteNonQuery();
            }


            // get last inserted client id
            using (var clientIdCmd = conn.CreateCommand())
            {
                clientIdCmd.CommandText = "SELECT last_insert_rowid();";
                clientId = (long)clientIdCmd.ExecuteScalar()!;
            }

            // debug
            //using (var textCmd = conn.CreateCommand())
            //{
            //    // SELECT COUNT(*) FROM Clients WHERE ClientId = 31;
            //    textCmd.CommandText = "SELECT FullName FROM Clients WHERE ClientId = 31";
            //    var exists = textCmd.ExecuteScalar();
            //}


            // insert into jobs table
            using (var jobsCmd = conn.CreateCommand())
            {
                jobsCmd.CommandText = @"

                INSERT INTO Jobs
                (ClientId, Description, Status, FinalPrice, CreatedDate, DueDate)

                VALUES
                (@clientId, @description, @status, @finalPrice, @createdDate, @dueDate);

                ";

                jobsCmd.Parameters.AddWithValue("@clientId", clientId);
                jobsCmd.Parameters.AddWithValue("@description", jobDescription);
                jobsCmd.Parameters.AddWithValue("@status", "New");
                jobsCmd.Parameters.AddWithValue("@finalPrice", finalPrice);
                jobsCmd.Parameters.AddWithValue("@createdDate", DateTime.Now.Date);
                jobsCmd.Parameters.AddWithValue("@dueDate", dueDate.ToString("yyyy-MM-dd"));
                jobsCmd.ExecuteNonQuery();
            }


            // get last inserted job id based on current clientId
            using (var jobIdCmd = conn.CreateCommand())
            {
                jobIdCmd.CommandText = @"
                    SELECT JobId
                    FROM Jobs
                    WHERE ClientId = @clientId
                    ORDER BY JobId DESC
                    LIMIT 1;
                ";
                jobIdCmd.Parameters.AddWithValue("@clientId", clientId);
                jobId = (long)jobIdCmd.ExecuteScalar()!;
            }


            //using (var invoiceCmd = conn.CreateCommand())
            //{
            //    invoiceCmd.CommandText = @"

            //    INSERT INTO Invoices
            //    (JobId, IsPaid)

            //    VALUES
            //    (@jobId, @isPaid);

            //    ";

            //    invoiceCmd.Parameters.AddWithValue("@jobId", jobId);
            //    invoiceCmd.Parameters.AddWithValue("@isPaid", false);
            //    invoiceCmd.ExecuteNonQuery();
            //}

            // Commit both together
            tx.Commit();

            return Task.CompletedTask;
        }

        public static async Task SetStatus(string status, int clientId, int jobId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                if (status == "New")
                {
                    using (var pragma = conn.CreateCommand())
                    {
                        pragma.CommandText = "PRAGMA foreign_keys = ON;";
                        pragma.ExecuteNonQuery();

                        using (var jobStatusCmd = conn.CreateCommand())
                        {
                            jobStatusCmd.CommandText = @"
    
                            UPDATE Jobs
                            SET Status = @status,
                                StartDate = @startDate,
                                CompletedDate = @completedDate
                            WHERE JobId = @jobId;
                        
                            ";

                            jobStatusCmd.Parameters.AddWithValue("@status", status);
                            jobStatusCmd.Parameters.AddWithValue("@startDate", DateTime.MinValue); // then you can check if new if less than today
                            jobStatusCmd.Parameters.AddWithValue("@completedDate", DateTime.MinValue); // then you can check if new if less than today
                            jobStatusCmd.Parameters.AddWithValue("@jobId", jobId);
                            jobStatusCmd.ExecuteNonQuery();
                        }
                    }
                }
                else if (status == "Active")
                {
                    using (var pragma = conn.CreateCommand())
                    {
                        pragma.CommandText = "PRAGMA foreign_keys = ON;";
                        pragma.ExecuteNonQuery();

                        using (var jobStatusCmd = conn.CreateCommand())
                        {
                            jobStatusCmd.CommandText = @"
    
                            UPDATE Jobs
                            SET Status = @status,
                                StartDate = @startDate
                            WHERE JobId = @jobId;
                        
                            ";

                            jobStatusCmd.Parameters.AddWithValue("@status", status);
                            jobStatusCmd.Parameters.AddWithValue("@startDate", DateTime.Now.Date);
                            jobStatusCmd.Parameters.AddWithValue("@jobId", jobId);
                            jobStatusCmd.ExecuteNonQuery();
                        }
                    }
                }
                else if (status == "Done")
                {
                    using (var pragma = conn.CreateCommand())
                    {
                        pragma.CommandText = "PRAGMA foreign_keys = ON;";
                        pragma.ExecuteNonQuery();

                        using (var jobStatusCmd = conn.CreateCommand())
                        {
                            jobStatusCmd.CommandText = @"
    
                            UPDATE Jobs
                            SET Status = @status,
                                CompletedDate = @completedDate
                            WHERE JobId = @jobId;
                        
                            ";

                            jobStatusCmd.Parameters.AddWithValue("@status", status);
                            jobStatusCmd.Parameters.AddWithValue("@completedDate", DateTime.Now.Date);
                            jobStatusCmd.Parameters.AddWithValue("@jobId", jobId);
                            jobStatusCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public static async Task DeleteRow(int clientId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using (var pragma = conn.CreateCommand())
                {
                    pragma.CommandText = "PRAGMA foreign_keys = ON;";
                    pragma.ExecuteNonQuery();
                }

                using var tx = conn.BeginTransaction();

                using (var deleteRowCmd = conn.CreateCommand())
                {
                    deleteRowCmd.CommandText = @"
    
                        DELETE FROM Clients
                        WHERE ClientId = @clientId;
                        ";

                    deleteRowCmd.Parameters.AddWithValue("@clientId", clientId);
                    deleteRowCmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {

            }
        }

        public static Task AddNewJobToClient(int clientId, string jobDescription, decimal finalPrice, DateTime dueDate)
        {
            //long clientId = 0;
            long jobId = 0;

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            //using (var pragma = conn.CreateCommand())
            //{
            //    pragma.CommandText = "PRAGMA foreign_keys = ON;";
            //    pragma.ExecuteNonQuery();
            //}

            //using var tx = conn.BeginTransaction();

            // insert into jobs table
            using (var jobsCmd = conn.CreateCommand())
            {
                jobsCmd.CommandText = @"

                INSERT INTO Jobs
                (ClientId, Description, Status, FinalPrice, CreatedDate, DueDate)

                VALUES
                (@clientId, @description, @status, @finalPrice, @createdDate, @dueDate);

                ";

                jobsCmd.Parameters.AddWithValue("@clientId", clientId);
                jobsCmd.Parameters.AddWithValue("@description", jobDescription);
                jobsCmd.Parameters.AddWithValue("@status", "New");
                jobsCmd.Parameters.AddWithValue("@finalPrice", finalPrice);
                jobsCmd.Parameters.AddWithValue("@createdDate", DateTime.Now.Date);
                jobsCmd.Parameters.AddWithValue("@dueDate", dueDate.ToString("yyyy-MM-dd"));
                jobsCmd.ExecuteNonQuery();
            }

            //// get last inserted job id based on current clientId
            using (var jobIdCmd = conn.CreateCommand())
            {
                jobIdCmd.CommandText = "SELECT last_insert_rowid();";
                jobId = (long)jobIdCmd.ExecuteScalar()!;
            }


            //using (var invoiceCmd = conn.CreateCommand())
            //{
            //    invoiceCmd.CommandText = @"

            //    INSERT INTO Invoices
            //    (JobId, IsPaid)

            //    VALUES
            //    (@jobId, @isPaid);

            //    ";

            //    invoiceCmd.Parameters.AddWithValue("@jobId", jobId);
            //    invoiceCmd.Parameters.AddWithValue("@isPaid", false);
            //    invoiceCmd.ExecuteNonQuery();
            //}

            // Commit both together
            //tx.Commit();

            return Task.CompletedTask;
        }
        #endregion
    }
}