using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Database
{
    public static class Database
    {
        #region Public Static Functions
        public static void SetUpDatabase()
        {
            Task.Run(() =>
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
                    CREATE TABLE IF NOT EXISTS Clients (
                    ClientId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT
                );

                CREATE TABLE IF NOT EXISTS Jobs (
                    JobId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER NOT NULL,
                    Description TEXT,
                    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId)
                );

                CREATE TABLE IF NOT EXISTS Invoices (
                    InvoiceId INTEGER PRIMARY KEY AUTOINCREMENT,
                    JobId INTEGER NOT NULL,
                    Amount REAL,
                    FOREIGN KEY (JobId) REFERENCES Jobs(JobId)
                );";

                cmd.ExecuteNonQuery();
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