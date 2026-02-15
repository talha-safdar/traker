using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models;

namespace Traker.Data
{
    /// <summary>
    /// Handles the relationships between Ui and local folders/files.
    /// </summary>
    public static class FileStore
    {
        public static Task LocateFolder(int clientId, string fullname, List<JobsModel> jobs)
        {
            // ClientId + "_" + SafeName

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


        private static string MakeSafeFolderName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            return name.Trim();
        }

    }
}
