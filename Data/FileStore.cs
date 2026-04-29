using System.Diagnostics;
using System.IO;
using System.Windows;
using Traker.Models.Database;
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
                string clientFolderName = $"{clientId}_{safeName}";
                string clientFolder = Path.Combine(clientsFolder, clientFolderName);
                Directory.CreateDirectory(clientFolder);

                // Documents/Traker/Clients/ID_clientName/Jobs
                string jobsFolder = Path.Combine(clientFolder, "Jobs");
                Directory.CreateDirectory(jobsFolder);

                // Documents/Traker/Clients/ID_clientName/Jobs/ID_jobTitle
                foreach (var job in jobs)
                {
                    string safeJobTitle = MakeSafeFolderName(job.Title);
                    string jobFolderName = $"{job.JobId}_{safeJobTitle}";
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

        public static Task CreateFolder(int clientId, string fullname)
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
                string clientFolderName = $"{clientId}_{safeName}";
                string clientFolder = Path.Combine(clientsFolder, clientFolderName);
                Directory.CreateDirectory(clientFolder);

                // Documents/Traker/Clients/ID_clientName/Jobs
                string jobsFolder = Path.Combine(clientFolder, "Jobs");
                Directory.CreateDirectory(jobsFolder);

                // Documents/Traker/Clients/ID_clientName/Invoices
                string invoicesFolder = Path.Combine(clientFolder, "Invoices");
                Directory.CreateDirectory(invoicesFolder);

                // Documents/Traker/Clients/ID_clientName/Notes
                string notesFolder = Path.Combine(clientFolder, "Notes");
                Directory.CreateDirectory(notesFolder);

                Logger.LogActivity(Logger.INFO, $"FileStore: CreateJobFolder() OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while creating the folder. Please try again.\n\n{ex.Message}",
                    "Create Folder",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"FileStore: CreateJobFolder() FAIL");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Makes a string safe for use as a folder name by replacing invalid characters with underscores and trimming whitespace.
        /// </summary>
        public static string MakeSafeFolderName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name.Trim();
        }

        public static Task<string> SaveInvoicePdf(int clientId, string clientName, string invoiceName)
        {
            // Documents/
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Documents/Traker
            string appRoot = Path.Combine(documents, "Traker");
            Directory.CreateDirectory(appRoot);

            // Documents/Traker/Clients
            string clientsFolder = Path.Combine(appRoot, "Clients");
            Directory.CreateDirectory(clientsFolder);

            // 1️⃣ Find existing client folder (by ID)
            var clientFolder = Directory.GetDirectories(clientsFolder)
                .FirstOrDefault(d =>
                {
                    var name = Path.GetFileName(d);
                    return name != null &&
                           name.StartsWith($"{clientId}_", StringComparison.OrdinalIgnoreCase);
                });

            // 2️⃣ If not found → create it
            if (clientFolder == null)
            {
                string safeClientName = MakeSafeFolderName(clientName);
                clientFolder = Path.Combine(clientsFolder, $"{clientId}_{safeClientName}");
                Directory.CreateDirectory(clientFolder);
            }

            // 3️⃣ Ensure Invoices folder exists
            string invoicesFolder = Path.Combine(clientFolder, "Invoices");
            Directory.CreateDirectory(invoicesFolder);

            // 4️⃣ Clean client name for file
            string safeName = MakeSafeFolderName(clientName);

            // 5️⃣ Build file name
            string fullPath = Path.Combine(invoicesFolder, invoiceName);

            Debug.WriteLine("Full name:" + invoiceName);
            Debug.WriteLine("Full path:" + fullPath);
            Debug.WriteLine("invoice path:" + invoicesFolder);

            return Task.FromResult(fullPath);
        }

        public static Task<string> GetInvoicePdfPath(int jobId, string jobTitle, int invoiceId, int clientId, string clientName, DateTime date)
        {
            // 1️⃣ Base path
            string basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Traker",
                "Clients");

            string clientFolder = Path.Combine(basePath, $"{clientId}_{MakeSafeFolderName(clientName)}");
            if (!Directory.Exists(clientFolder))
                return null;


            string invoicesFolder = Path.Combine(clientFolder, "Invoices");
            if (!Directory.Exists(invoicesFolder))
                return null;

            //// 2️⃣ Find job folder: [jobId]_[safeJobTitle]
            //var jobFolder = Directory.GetDirectories(basePath)
            //    .FirstOrDefault(d =>
            //    {
            //        var name = Path.GetFileName(d);
            //        return name != null &&
            //               name.StartsWith($"{jobId}_{MakeSafeFolderName(jobTitle)}", StringComparison.OrdinalIgnoreCase);
            //    });

            //if (jobFolder == null)
            //    throw new Exception("Job folder not found");

            // 3️⃣ Go into Invoices folder
            //string invoicesFolder = Path.Combine(jobFolder, "Invoices");
            //if (!Directory.Exists(invoicesFolder))
            //    return null;

            //if (!Directory.Exists(invoicesFolder))
            //    throw new Exception("Invoices folder not found");

            // 5️⃣ Build expected filename prefix
            string expectedPrefix = $"INV-{invoiceId}_{jobId}_{clientId}_{MakeSafeFolderName(clientName)}_{date.ToString("dd-MM-yyyy")}_{date.ToString("HHmmss")}";

            // 6️⃣ Find matching file
            string file = Directory.GetFiles(invoicesFolder, "*.pdf")
                .FirstOrDefault(f =>
                    Path.GetFileName(f)
                        .StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))!;

            if (file == null)
                throw new Exception("Invoice file not found");


            return Task.FromResult(file);
        }
    }
}