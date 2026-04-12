using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using System.IO;
using System.Reflection;
using System.Windows;
using Traker.Models;
using Traker.Services;

namespace Traker.Database
{
    /// <summary>
    /// Handle relationships between the UI and the database.
    /// </summary>
    public static class Database
    {
        #region Private Static Variables
        private static string _connectionString = String.Empty; // location of the database file
        #endregion

        #region Public Static Functions
        /// <summary>
        /// Set up the database by creating the database file and tables if they don't exist. This is done by reading the Schema.sql file and executing the SQL commands within it. If any errors occur during this process, an error message is displayed to the user and the application is closed. If the database is set up successfully, a log entry is made indicating that the database is connected.
        /// </summary>
        public static async Task SetUpDatabase()
        {
            await Task.Run(() =>
            {
                try
                {
                    // set directory and database name
                    var folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Traker");
                    Directory.CreateDirectory(folder);
                    var dbPath = Path.Combine(folder, "traker.db");

                    // creeate database
                    _connectionString = $"Data Source={dbPath}";
                    using var conn = new SqliteConnection(_connectionString);
                    conn.Open();

                    // read from Schema.sql
                    var assembly = Assembly.GetExecutingAssembly();
                    using var stream = assembly.GetManifestResourceStream("Traker.Assets.Database.Schema.sql");
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream); // get Schema.sql

                        string sql = reader.ReadToEnd(); // read Schema.sql

                        var _sqliteCommand = conn.CreateCommand();
                        _sqliteCommand.CommandText = sql; // set Schema.sqlwr

                        _sqliteCommand.ExecuteNonQuery(); // execute Schema.sql
                        Logger.LogActivity(Logger.INFO, "Database: SetUpDatabase() DATABASE CONNECTED");
                    }
                    else
                    {
                        MessageBox.Show(
                            $"An error occurred while setting up the database. Please try again.\n\nUnable to locate database schema.",
                            "Database Setup",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        Logger.LogActivity(Logger.WARNING, "Database: SetUpDatabase() DATABASE NOT CONNECTED");
                        Environment.Exit(1); // kill process
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"An error occurred while setting up the database. Please try again.\n\n{ex.Message}",
                        "Database Setup",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Environment.Exit(1); // kill process
                    Logger.LogActivity(Logger.ERROR, "Database: SetUpDatabase() DATABASE ERROR");
                }
            });
        }

        /// <summary>
        /// Fetch all data from the Clients table and return it as a list of ClientsModel objects. This is done by opening a connection to the database, executing a SQL query to select all rows from the Clients table, and then reading the results using a SqliteDataReader. For each row, a new ClientsModel object is created and added to the list. If any errors occur during this process, an error message is displayed to the user and the application is closed. If the data is fetched successfully, a log entry is made indicating that the operation was successful.
        /// </summary>
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
                                        CreatedDate = DateOnly.FromDateTime(Convert.ToDateTime(CreatedDate))!,
                                        IsActive = Convert.ToBoolean(IsActive)
                                    });
                                }
                            }
                        }
                    }
                }
                Logger.LogActivity(Logger.INFO, "Database: FetchClientsTable() OK");
                return clientsList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while fetching 'Client' table. Please try again.\n\n{ex.Message}",
                    "Fetch Client Tables",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Logger.LogActivity(Logger.ERROR, $"Database: FetchClientsTable() FAIL");
                Environment.Exit(1); // kill process
                throw; // necessary otherwsie cries about no return value
            }
        }

        /// <summary>
        /// Fetch all data from the Jobs table and return it as a list of JobsModel objects. This is done by opening a connection to the database, executing a SQL query to select all rows from the Jobs table, and then reading the results using a SqliteDataReader. For each row, a new JobsModel object is created and added to the list. If any errors occur during this process, an error message is displayed to the user and the application is closed. If the data is fetched successfully, a log entry is made indicating that the operation was successful.
        /// </summary>
        public static List<JobsModel> FetchJobsTable()
        {
            List<JobsModel> jobsList = new List<JobsModel>();

            try
            {
                using (var conn = new SqliteConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Jobs";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
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
                                        CreatedDate = DateOnly.FromDateTime(Convert.ToDateTime(CreatedDate)),
                                        StartDate = DateOnly.FromDateTime(Convert.ToDateTime(StartDate)),
                                        CompletedDate = DateOnly.FromDateTime(Convert.ToDateTime(CompletedDate)),
                                        DueDate = DateOnly.FromDateTime(Convert.ToDateTime(DueDate)),
                                        FolderPath = FolderPath.ToString()!,
                                        Notes = Notes.ToString()!,
                                        IsArchived = Convert.ToBoolean(IsArchived),
                                    });
                                }
                            }
                        }
                    }
                }
                Logger.LogActivity(Logger.INFO, "Database: FetchJobsTable() OK");
                return jobsList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while fetching 'Job' table. Please try again.\n\n{ex.Message}",
                    "Fetch Job Tables",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Logger.LogActivity(Logger.ERROR, $"Database: FetchJobsTable() FAIL");
                Environment.Exit(1); // kill process
                throw; // necessary otherwsie cries about no return value
            }
        }

        /// <summary>
        /// Fetch all data from the Invoices table and return it as a list of InvoicesModel objects. This is done by opening a connection to the database, executing a SQL query to select all rows from the Invoices table, and then reading the results using a SqliteDataReader. For each row, a new InvoicesModel object is created and added to the list. If any errors occur during this process, an error message is displayed to the user and the application is closed. If the data is fetched successfully, a log entry is made indicating that the operation was successful.
        /// </summary>
        public static List<InvoicesModel> FetchInvoiceTable()
        {
            List<InvoicesModel> invoicesList = new List<InvoicesModel>();

            try
            {
                using (var conn = new SqliteConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Invoices";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var InvoiceId = reader["InvoiceId"];
                                var JobId = reader["JobId"];
                                var InvoiceNumber = reader["InvoiceNumber"] == DBNull.Value ? String.Empty : reader["InvoiceNumber"];
                                var Subtotal = reader["Subtotal"] == DBNull.Value ? "0" : reader["Subtotal"];
                                var TaxAmount = reader["TaxAmount"] == DBNull.Value ? "0" : reader["TaxAmount"];
                                var TotalAmount = reader["TotalAmount"] == DBNull.Value ? "0" : reader["TotalAmount"];
                                var IssueDate = reader["IssueDate"] == DBNull.Value ? DateTime.MinValue : reader["IssueDate"];
                                var DueDate = reader["DueDate"] == DBNull.Value ? DateTime.MinValue : reader["DueDate"];
                                var BillingName = reader["BillingName"] == DBNull.Value ? String.Empty : reader["BillingName"];
                                var BillingAddress = reader["BillingAddress"] == DBNull.Value ? String.Empty : reader["BillingAddress"];
                                var BillingCity = reader["BillingCity"] == DBNull.Value ? String.Empty : reader["BillingCity"];
                                var BillingPostcode = reader["BillingPostcode"] == DBNull.Value ? String.Empty : reader["BillingPostcode"];
                                var BillingCountry = reader["BillingCountry"] == DBNull.Value ? String.Empty : reader["BillingCountry"];
                                var Status = reader["Status"] == DBNull.Value ? String.Empty : reader["Status"];
                                var IsDeleted = reader["IsDeleted"] == DBNull.Value ? String.Empty : reader["IsDeleted"];
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
                                    string.IsNullOrEmpty(BillingName.ToString()) == false ||
                                    string.IsNullOrEmpty(BillingAddress.ToString()) == false ||
                                    string.IsNullOrEmpty(BillingCity.ToString()) == false ||
                                    string.IsNullOrEmpty(BillingPostcode.ToString()) == false ||
                                    string.IsNullOrEmpty(BillingCountry.ToString()) == false ||
                                    string.IsNullOrEmpty(Status.ToString()) == false ||
                                    string.IsNullOrEmpty(IsDeleted.ToString()) == false ||
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
                                        IssueDate = DateOnly.FromDateTime(Convert.ToDateTime(IssueDate)),
                                        DueDate = DateOnly.FromDateTime(Convert.ToDateTime(DueDate)),
                                        Status = Status.ToString()!,
                                        BillingName = BillingName.ToString()!,
                                        BillingAddress = BillingAddress.ToString()!,
                                        BillingCity = BillingCity.ToString()!,
                                        BillingPostcode = BillingPostcode.ToString()!,
                                        BillingCountry = BillingCountry.ToString()!,
                                        IsDeleted = Convert.ToBoolean(IsDeleted),
                                        PaidDate = DateOnly.FromDateTime(Convert.ToDateTime(PaidDate)),
                                        PaymentMethod = PaymentMethod.ToString()!,
                                        Notes = Notes.ToString()!,
                                    });
                                }
                            }
                        }
                    }
                }
                Logger.LogActivity(Logger.INFO, "Database: FetchInvoiceTable() OK");
                return invoicesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while fetching 'Invoice' table. Please try again.\n\n{ex.Message}",
                    "Fetch Invoice Tables",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Logger.LogActivity(Logger.ERROR, $"Database: FetchInvoiceTable() FAIL");
                Environment.Exit(1); // kill process
                throw; // necessary otherwsie cries about no return value
            }
        }

        /// <summary>
        /// Add a new row to the Clients and Jobs tables in the database. This is done by opening a connection to the database, starting a transaction, and then executing two SQL commands: one to insert a new row into the Clients table and another to insert a new row into the Jobs table with a foreign key reference to the newly inserted client. If any errors occur during this process, an error message is displayed to the user and the transaction is rolled back. If the rows are added successfully, a log entry is made indicating that the operation was successful along with the new client and job IDs.
        /// </summary>
        public static Task AddRow(string clientName, string jobTitle, string jobDescription, decimal finalPrice, DateTime dueDate)
        {
            try
            {
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
                    clienstCmd.Parameters.AddWithValue("@createdDate", DateOnly.FromDateTime(DateTime.Now));
                    clienstCmd.ExecuteNonQuery();
                }


                // get last inserted client id
                using (var clientIdCmd = conn.CreateCommand())
                {
                    clientIdCmd.CommandText = "SELECT last_insert_rowid();";
                    clientId = (long)clientIdCmd.ExecuteScalar()!;
                }

                // insert into jobs table
                using (var jobsCmd = conn.CreateCommand())
                {
                    jobsCmd.CommandText = @"

                    INSERT INTO Jobs
                    (ClientId, Title, Description, Status, FinalPrice, CreatedDate, DueDate)

                    VALUES
                    (@clientId, @title, @description, @status, @finalPrice, @createdDate, @dueDate);

                    ";

                    jobsCmd.Parameters.AddWithValue("@clientId", clientId);
                    jobsCmd.Parameters.AddWithValue("@title", jobTitle);
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

                // Commit both together
                tx.Commit();
                Logger.LogActivity(Logger.INFO, $"Database: AddRow() OK - ClientId: {clientId}, JobId: {jobId}");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while adding a new client and job. Please try again.\n\n{ex.Message}",
                    "Add Client and Job",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"Database: AddRow() FAIL");
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Set the status of a job in the Jobs table. This is done by opening a connection to the database, executing a SQL command to update the Status column of the specified job, and then closing the connection. The function takes three parameters: the new status, the client ID, and the job ID. If any errors occur during this process, an error message is displayed to the user. If the status is updated successfully, a log entry is made indicating that the operation was successful along with the client ID, job ID, and new status.
        /// </summary>
        public static Task SetStatus(string status, int clientId, int jobId)
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
                Logger.LogActivity(Logger.INFO, $"Database: SetStatus() OK - ClientId: {clientId}, JobId: {jobId}, Status: {status}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while updating job status. Please try again.\n\n{ex.Message}",
                    "Update Job Status",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"Database: SetStatus() FAIL - ClientId: {clientId}, JobId: {jobId}, Status: {status}");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete a row from the Clients table and all related rows from the Jobs table. This is done by opening a connection to the database, starting a transaction, and then executing a SQL command to delete the specified client from the Clients table. The command is set up to also delete any related rows from the Jobs table using a foreign key constraint with ON DELETE CASCADE. If any errors occur during this process, an error message is displayed to the user and the transaction is rolled back. If the row is deleted successfully, a log entry is made indicating that the operation was successful along with the client ID.
        /// </summary>
        public static Task DeleteRow(int clientId)
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
                Logger.LogActivity(Logger.INFO, $"Database: DeleteRow() OK - ClientId: {clientId}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while deleting the client and related jobs. Please try again.\n\n{ex.Message}",
                    "Delete Client and Jobs",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"Database: DeleteRow() FAIL - ClientId: {clientId}");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Add a new row to the Jobs table for an existing client. This is done by opening a connection to the database, executing a SQL command to insert a new row into the Jobs table with a foreign key reference to the specified client, and then closing the connection. The function takes four parameters: the client ID, job description, final price, and due date. If any errors occur during this process, an error message is displayed to the user. If the row is added successfully, a log entry is made indicating that the operation was successful along with the client ID and job ID.
        /// </summary>
        public static Task AddNewJobToClient(int clientId, string jobDescription, decimal finalPrice, DateTime dueDate)
        {
            try
            {
                long jobId = 0;

                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

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
                Logger.LogActivity(Logger.INFO, $"Database: AddNewJobToClient() OK - ClientId: {clientId}, JobId: {jobId}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while adding a new job to the client. Please try again.\n\n{ex.Message}",
                    "Add Job to Client",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"Database: AddNewJobToClient() FAIL - ClientId: {clientId}");
            }
            return Task.CompletedTask;
        }

        //public static Task UpdateRow(int clientId, )

        /// <summary>
        /// Create a new invoice for a job by adding a new row to the Invoices table. This is done by opening a connection to the database, starting a transaction, and then executing a SQL command to insert a new row into the Invoices table with the specified details. The function takes several parameters including the job ID, subtotal, tax amount, total amount, due date, billing name, billing address, billing city, billing postcode, and billing country. If any errors occur during this process, an error message is displayed to the user and the transaction is rolled back. If the invoice is created successfully, a log entry is made indicating that the operation was successful along with the job ID.
        /// </summary>
        public static Task CreateInvoice(int jobId, decimal subtotal, int taxAmount, decimal totalAmount, DateTime dueDate, string billingName, string billingAddress, string billingCity, string billingPostcode, string billingCountry)
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

                using var transaction = conn.BeginTransaction();

                long nextNumber;

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "SELECT IFNULL(MAX(InvoiceNumber), 0) FROM Invoices;";
                    nextNumber = (long)cmd.ExecuteScalar()! + 1;
                }

                // insert into Clients table
                using (var invoicesCmd = conn.CreateCommand())
                {
                    invoicesCmd.CommandText = @"

                    INSERT INTO Invoices
                    (JobId, 
                    InvoiceNumber, 
                    Subtotal, 
                    TaxAmount, 
                    TotalAmount, 
                    IssueDate, 
                    DueDate,    
                    BillingName, 
                    BillingAddress,
                    BillingCity,
                    BillingPostcode,
                    BillingCountry,
                    Status)

                    VALUES
                    (@jobId, 
                    @invoiceNumber, 
                    @subtotal,
                    @taxAmount,
                    @totalAmount,
                    @issueDate,
                    @dueDate,
                    @billingName,
                    @billingAddress,
                    @billingCity,
                    @billingPostcode,
                    @billingCountry,
                    @status);

                    ";
                    invoicesCmd.Parameters.AddWithValue("@jobId", jobId);
                    invoicesCmd.Parameters.AddWithValue("@invoiceNumber", nextNumber);
                    invoicesCmd.Parameters.AddWithValue("@subtotal", subtotal);
                    invoicesCmd.Parameters.AddWithValue("@taxAmount", taxAmount);
                    invoicesCmd.Parameters.AddWithValue("@totalAmount", totalAmount);
                    invoicesCmd.Parameters.AddWithValue("@issueDate", DateTime.Now.Date);
                    invoicesCmd.Parameters.AddWithValue("@dueDate", dueDate.ToString("yyyy-MM-dd"));
                    invoicesCmd.Parameters.AddWithValue("@billingName", billingName);
                    invoicesCmd.Parameters.AddWithValue("@billingAddress", billingAddress);
                    invoicesCmd.Parameters.AddWithValue("@billingCity", billingCity);
                    invoicesCmd.Parameters.AddWithValue("@billingPostcode", billingPostcode);
                    invoicesCmd.Parameters.AddWithValue("@billingCountry", billingCountry);
                    invoicesCmd.Parameters.AddWithValue("@status", "Created");

                    invoicesCmd.ExecuteNonQuery();
                }
                transaction.Commit();
                Logger.LogActivity(Logger.INFO, $"Database: CreateInvoice() OK - JobId: {jobId}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while creating the invoice. Please try again.\n\n{ex.Message}",
                    "Create Invoice",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"Database: CreateInvoice() FAIL - JobId: {jobId}");
            }
            return Task.CompletedTask;
        }

        public static Task EditClient(int clientId, string type, string fullName, string email, string companyName, string phoneNumber, string billingAddress, string city, string postcode, string country, bool isActive)
        {
            // in the future replace the long ass arguments with a variable list :)

            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();

                cmd.CommandText = @"
                    UPDATE Clients
                    SET Type = @type,
                        FullName = @fullname,
                        Email = @email,
                        CompanyName = @companyName,
                        PhoneNumber = @phoneNumber,
                        BillingAddress = @billingAddress,
                        City = @city,
                        Postcode = @postcode,
                        Country = @country,
                        IsActive = @isActive
                    WHERE ClientId = @clientId;
                ";

                cmd.Parameters.AddWithValue("@clientId", clientId);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@fullname", fullName);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@companyName", companyName);
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@billingAddress", billingAddress);
                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@postcode", postcode);
                cmd.Parameters.AddWithValue("@country", country);
                cmd.Parameters.AddWithValue("@isActive", isActive);

                cmd.ExecuteNonQuery();

                Logger.LogActivity(Logger.INFO, $"Database: AddNewJobToClient() OK - ClientId: {clientId}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while editing the client. Please try again.\n\n{ex.Message}",
                    "Edit Client",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"Database: EditClient() FAIL - ClientId: {clientId}");
            }
            return Task.CompletedTask;
        }
        #endregion
    }
}