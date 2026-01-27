using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Traker.Database
{
    public static class Database
    {
        #region Public Static Functions
        public static void SetUpDatabase()
        {
            Task.Run(() =>
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
                    var connectionString = $"Data Source={dbPath}";
                    using var conn = new SqliteConnection(connectionString);
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
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
                        );";

                    int rows = cmd.ExecuteNonQuery();

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
                        MessageBoxButton.OK);

                    Debug.WriteLine($"Database setup error: {ex.Message}");
                }


                //                cmd.CommandText = @"SELECT name 
                //                    FROM sqlite_master 
                //                    WHERE type='table'
                //AND name NOT LIKE 'sqlite_%';;
                //                    ";

                //                cmd.ExecuteNonQuery();

                //                using var reader = cmd.ExecuteReader();

                //                while (reader.Read())
                //                {
                //                    Debug.WriteLine(reader.GetString(0));
                //                }
            });
        }
        #endregion
    }
}