using System.Diagnostics;
using System.IO;
using System.Windows;
using Traker.Models;
using Traker.Services;

namespace Traker.Data
{
    /// <summary>
    /// Handles the relationships between UI and local folders/files.
    /// </summary>
    public static class FileStore
    {
        public static Task LocateFolder(int clientId, string fullname, List<JobsModel> jobs)
        {
            try
            {
                // Documents/
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Documents/Traker
                string appRoot = Path.Combine(documents, "Traker");
                Directory.CreateDirectory(appRoot);

                // Documents/Traker/Clients
                string clientsFolder = Path.Combine(appRoot, "Clients");
                Directory.CreateDirectory(clientsFolder);

                // Documents/Traker/Clients/ID_clientName
                string safeName = MakeSafeFolderName(fullname);
                string clientFolderName = $"{clientId:D4}_{safeName}";
                string clientFolder = Path.Combine(clientsFolder, clientFolderName);
                Directory.CreateDirectory(clientFolder);

                // Documents/Traker/Clients/ID_clientName/Jobs
                string jobsFolder = Path.Combine(clientFolder, "Jobs");
                Directory.CreateDirectory(jobsFolder);

                // Documents/Traker/Clients/ID_clientName/Jobs/ID_jobTitle
                foreach (var job in jobs)
                {
                    string safeJobTitle = MakeSafeFolderName(job.Description); // use Title in the future
                    string jobFolderName = $"{job.JobId:D4}_{safeJobTitle}";
                    string jobFolder = Path.Combine(jobsFolder, jobFolderName);

                    Directory.CreateDirectory(jobFolder);
                }

                // Documents/Traker/Clients/ID_clientName/Invoices
                string invoicesFolder = Path.Combine(clientFolder, "Invoices");
                Directory.CreateDirectory(invoicesFolder);

                // Documents/Traker/Clients/ID_clientName/Notes
                string notesFolder = Path.Combine(clientFolder, "Notes");
                Directory.CreateDirectory(notesFolder);

                // Open folder in Explorer
                Process.Start(new ProcessStartInfo
                {
                    FileName = clientFolder,
                    UseShellExecute = true
                });
                Logger.LogActivity(Logger.INFO, $"FileStore: LocateFolder() OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while locating the folder. Please try again.\n\n{ex.Message}",
                    "Open Folder",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"FileStore: LocateFolder() FAIL");
            }
            return Task.CompletedTask;
        }

        //public string CreateJobFolder(string clientFolderPath, int jobId, string jobTitle)
        //{
        //    string safeTitle = MakeSafeFolderName(jobTitle);
        //    string jobFolderName = $"{jobId:D4}_{safeTitle}";
        //    string jobFolder = Path.Combine(clientFolderPath, "Jobs", jobFolderName);

        //    Directory.CreateDirectory(jobFolder);

        //    return jobFolder;
        //}

        /// <summary>
        /// Makes a string safe for use as a folder name by replacing invalid characters with underscores and trimming whitespace.
        /// </summary>
        private static string MakeSafeFolderName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name.Trim();
        }
    }
}