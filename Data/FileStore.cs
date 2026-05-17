using Caliburn.Micro;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Traker.Helper;
using Traker.Models.Database;
using Traker.Services;
using Traker.States;

namespace Traker.Data
{
    /// <summary>
    /// Handles the relationships between UI and local folders/files.
    /// </summary>
    public static class FileStore
    {
        #region Create Functions
        /// <summary>
        /// Create client directory
        /// </summary>
        public static Task<string> CreateClientFolder(int clientId, string fullname)
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

                return Task.FromResult(clientFolderName);
            }
            catch (Exception ex)
            {
                Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Create Client Folder";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"FileStore: CreateClientFolder() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create job directory
        /// </summary>
        public static Task<string> CreateJobFolder(int clientId, int jobId, string fullname, string jobTitle)
        {
            try
            {
                // Documents/
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Documents/Traker
                string appRoot = Path.Combine(documents, "Traker");

                // Documents/Traker/Clients
                string clientsFolder = Path.Combine(appRoot, "Clients");

                // Documents/Traker/Clients/ID_clientName
                string safeName = MakeSafeFolderName(fullname);
                string clientFolderName = $"{clientId}_{safeName}";
                string clientFolder = Path.Combine(clientsFolder, clientFolderName);

                // Documents/Traker/Clients/ID_clientName/Jobs
                string jobsFolder = Path.Combine(clientFolder, "Jobs");

                // Documents/Traker/Clients/ID_clientName/Jobs/ID_jobTitle
                string safeJobTitle = MakeSafeFolderName(jobTitle);
                string jobFolderName = $"{jobId}_{safeJobTitle}";
                string jobFolder = Path.Combine(jobsFolder, jobFolderName);

                // create job folder if it doesn't exist
                Directory.CreateDirectory(jobFolder);

                return Task.FromResult(jobFolderName);
            }
            catch (Exception ex)
            {
                Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Create Job Folder";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"FileStore: CreateJobFolder() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        #endregion

        #region Save Functions
        /// <summary>
        /// Save invoice file
        /// </summary>
        public static Task<string> SaveInvoiceFile(int clientId, string clientName, string invoiceName)
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
            catch (Exception ex)
            {
                Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Save Invoice File";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"FileStore: SaveInvoiceFile() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        #endregion

        #region Update Functions
        /// <summary>
        /// Update client folder name
        /// returns true for all good
        /// </summary>
        public static Task<bool> UpdateClientFolderName(int clientId, string fullName, string newName)
        {
            bool isOkay = false; // true if file was renamed else false
            try
            {
                // Documents/
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Documents/Traker
                string appRoot = Path.Combine(documents, "Traker");

                // Documents/Traker/Clients
                string clientsFolder = Path.Combine(appRoot, "Clients");

                // Documents/Traker/Clients/ID_clientName
                string safeName = MakeSafeFolderName(fullName);
                string clientFolderName = $"{clientId}_{safeName}";
                string clientFolder = Path.Combine(clientsFolder, clientFolderName);

                // new folder name
                string safeNewName = MakeSafeFolderName(newName);
                string clientFolderNewName = $"{clientId}_{safeNewName}";
                string clientNewFolder = Path.Combine(clientsFolder, clientFolderNewName);

                // repalce names
                if (clientFolder != clientNewFolder)
                {
                    try
                    {
                        Directory.Move(clientFolder, clientNewFolder);
                        isOkay = true;
                    }
                    catch(Exception ex)
                    {
                        isOkay = false;
                        AppState state = IoC.Get<AppState>();
                        IWindowManager windowManager = IoC.Get<IWindowManager>();
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                        {
                            state.messageBoxVM.Symbol = 2;
                            state.messageBoxVM.HeadMessage = "Update Client Folder Name";
                            state.messageBoxVM.Message = ex.Message;
                            state.messageBoxVM.ButtonStyle = Names.OK;
                            windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                        Logger.LogActivity(Logger.INFO, $"FileStore: UpdateClientFolderName() FAIL\n\t{ex.Message}");
                    }
                }
                return Task.FromResult(isOkay);
            }
            catch (Exception ex)
            {
                AppState state = IoC.Get<AppState>();
                IWindowManager windowManager = IoC.Get<IWindowManager>();
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                {
                    state.messageBoxVM.Symbol = 2;
                    state.messageBoxVM.HeadMessage = "Update Client Folder Name";
                    state.messageBoxVM.Message = ex.Message;
                    state.messageBoxVM.ButtonStyle = Names.OK;
                    windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"FileStore: UpdateClientFolderName() FAIL\n\t{ex.Message}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Update job folder name
        /// </summary>
        public static Task<bool> UpdateJobFolderName(int clientId, int jobId, string fullName, string jobTitle, string newJobTitle)
        {
            bool isOkay = false; // true if file was renamed else false
            try
            {
                // Documents/
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Documents/Traker
                string appRoot = Path.Combine(documents, "Traker");

                // Documents/Traker/Clients
                string clientsFolder = Path.Combine(appRoot, "Clients");

                // Documents/Traker/Clients/ID_clientName
                string safeName = MakeSafeFolderName(fullName);
                string clientFolderName = $"{clientId}_{safeName}";
                string clientFolder = Path.Combine(clientsFolder, clientFolderName);

                // Documents/Traker/Clients/ID_clientName/Jobs
                string jobsFolder = Path.Combine(clientFolder, "Jobs");

                // Documents/Traker/Clients/ID_clientName/Jobs/ID_jobTitle
                string safeJobTitle = MakeSafeFolderName(jobTitle);
                string jobFolderName = $"{jobId}_{safeJobTitle}";
                string jobFolder = Path.Combine(jobsFolder, jobFolderName);

                // new folder name
                string safeNewName = MakeSafeFolderName(newJobTitle);
                string newName = $"{jobId}_{safeNewName}";
                string jobNewFolder = Path.Combine(jobsFolder, newName);

                // repalce names
                if (jobFolder != jobNewFolder)
                {
                    try 
                    {
                        Directory.Move(jobFolder, jobNewFolder);
                        isOkay = true;
                    }
                    catch(Exception ex)
                    {
                        isOkay = false;
                        AppState state = IoC.Get<AppState>();
                        IWindowManager windowManager = IoC.Get<IWindowManager>();
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                        {
                            state.messageBoxVM.Symbol = 2;
                            state.messageBoxVM.HeadMessage = "Update Job Folder Name";
                            state.messageBoxVM.Message = ex.Message;
                            state.messageBoxVM.ButtonStyle = Names.OK;
                            windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                        Logger.LogActivity(Logger.INFO, $"FileStore: UpdateClientFolderName() FAIL\n\t{ex.Message}");
                    }
                }
                return Task.FromResult(isOkay);
            }
            catch (Exception ex)
            {
                AppState state = IoC.Get<AppState>();
                IWindowManager windowManager = IoC.Get<IWindowManager>();
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                {
                    state.messageBoxVM.Symbol = 2;
                    state.messageBoxVM.HeadMessage = "Update Job Folder Name";
                    state.messageBoxVM.Message = ex.Message;
                    state.messageBoxVM.ButtonStyle = Names.OK;
                    windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"FileStore: UpdateJobFolderName() FAIL\n\t{ex.Message}");
                return Task.FromResult(false);
            }
        }
        #endregion

        #region Delete Functions
        /// <summary>
        /// Deletes entire database
        /// </summary>
        public async static Task <bool> DeleteDatabase()
        {
            bool isSuccessful = false;
            await Task.Run(async() =>
            {
                try
                {
                    // delete database file
                    var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Traker");
                    var dbPath = Path.Combine(folder, "traker.db");
                    if (File.Exists(dbPath))
                    {
                        // database in use problem happens here
                        File.Delete(dbPath);
                        isSuccessful = true;
                    }
                    else
                    {
                        isSuccessful = false;
                        Logger.LogActivity(Logger.WARNING, "Database: CheckUserDatabase() DATABASE FILE NOT FOUND FOR DELETION");
                    }

                }
                catch (Exception ex)
                {
                    await Execute.OnUIThreadAsync(async() =>
                    {
                        AppState state = IoC.Get<AppState>();
                        IWindowManager windowManager = IoC.Get<IWindowManager>();
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                        {
                            state.messageBoxVM.Symbol = 2;
                            state.messageBoxVM.HeadMessage = "Delete Database";
                            state.messageBoxVM.Message = ex.Message;
                            state.messageBoxVM.ButtonStyle = Names.OK;
                            await windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                    });
                    isSuccessful = false;
                    Logger.LogActivity(Logger.ERROR, $"FileStore: DeleteDatabase() FAIL\n\t{ex.Message}");
                }
            });
            return isSuccessful;
        }

        /// <summary>
        /// Delete client folder
        /// </summary>
        public static Task DeleteClientFolder(int clientId, string clientName)
        {
            Task.Run(() =>
            {
                try
                {
                    // Documents/
                    string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    // Documents/Traker
                    string appRoot = Path.Combine(documents, "Traker");

                    // Documents/Traker/Clients
                    string clientsFolder = Path.Combine(appRoot, "Clients");

                    // Documents/Traker/Clients/ID_clientName
                    string safeName = MakeSafeFolderName(clientName);
                    string clientFolderName = $"{clientId}_{safeName}";
                    string clientFolder = Path.Combine(clientsFolder, clientFolderName);

                    // check if folder exists
                    if (Directory.Exists(clientFolder))
                    {
                        Directory.Delete(clientFolder, true); // true means delete everything inside too
                    }
                }
                catch (Exception ex)
                {
                    Execute.OnUIThreadAsync(() =>
                    {
                        AppState state = IoC.Get<AppState>();
                        IWindowManager windowManager = IoC.Get<IWindowManager>();
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                        {
                            state.messageBoxVM.Symbol = 2;
                            state.messageBoxVM.HeadMessage = "Delete Client Directory";
                            state.messageBoxVM.Message = ex.Message;
                            state.messageBoxVM.ButtonStyle = Names.OK;
                            windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                        return Task.CompletedTask;
                    });
                    Logger.LogActivity(Logger.ERROR, $"FileStore: DeleteClientFolder() FAIL\n\t{ex.Message}");
                    throw;
                }
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete job folder
        /// </summary>
        public static Task<bool> DeleteJobFolder(int clientId, int jobId, string fullname, string jobTitle)
        {
            try
            {
                // if only one job left then delete client too
                bool isLastJob = false;

                // Documents/
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Documents/Traker
                string appRoot = Path.Combine(documents, "Traker");

                // Documents/Traker/Clients
                string clientsFolder = Path.Combine(appRoot, "Clients");

                // Documents/Traker/Clients/ID_clientName
                string safeName = MakeSafeFolderName(fullname);
                string clientFolderName = $"{clientId}_{safeName}";
                string clientFolder = Path.Combine(clientsFolder, clientFolderName);

                // Documents/Traker/Clients/ID_clientName/Jobs
                string jobsFolder = Path.Combine(clientFolder, "Jobs");

                // Documents/Traker/Clients/ID_clientName/Jobs/ID_jobTitle
                string safeJobTitle = MakeSafeFolderName(jobTitle);
                string jobFolderName = $"{jobId}_{safeJobTitle}";
                string jobFolder = Path.Combine(jobsFolder, jobFolderName);

                // check if folder exists
                if (Directory.Exists(jobFolder))
                {
                    Directory.Delete(jobFolder, true); // true means delete everything inside too

                    // 1. Get the count of all subfolders inside Clients
                    string[] subFolders = Directory.GetDirectories(jobsFolder);
                    int folderCount = subFolders.Length;
                    Debug.WriteLine("Delete folder job: " + folderCount);

                    if (folderCount == 0)
                    {
                        isLastJob = true;
                    }
                    else if (folderCount > 0)
                    {
                        isLastJob = false;
                    }
                }
                return Task.FromResult(isLastJob);
            }
            catch(Exception ex)
            {
                Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Delete Job Directory";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"FileStore: DeleteJobFolder() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        #endregion

        #region File/Folder Access Functions
        /// <summary>
        /// Locate job directory
        /// </summary>

        public static Task LocateJobFolder(int clientId, int jobId, string fullName, string jobTitle)
        {
            Task.Run(() =>
            {
                try
                {
                    // Documents/
                    string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    // Documents/Traker
                    string appRoot = Path.Combine(documents, "Traker");

                    // Documents/Traker/Clients
                    string clientsFolder = Path.Combine(appRoot, "Clients");

                    // Documents/Traker/Clients/ID_clientName
                    string safeName = MakeSafeFolderName(fullName);
                    string clientFolderName = $"{clientId}_{safeName}";
                    string clientFolder = Path.Combine(clientsFolder, clientFolderName);

                    // Documents/Traker/Clients/ID_clientName/Jobs
                    string jobsFolder = Path.Combine(clientFolder, "Jobs");

                    string safeJobTitle = MakeSafeFolderName(jobTitle);
                    string jobFolderName = $"{jobId}_{safeJobTitle}";
                    string jobFolder = Path.Combine(jobsFolder, jobFolderName);

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = jobFolder,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Execute.OnUIThreadAsync(() =>
                    {
                        AppState state = IoC.Get<AppState>();
                        IWindowManager windowManager = IoC.Get<IWindowManager>();
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                        {
                            state.messageBoxVM.Symbol = 2;
                            state.messageBoxVM.HeadMessage = "Locate Job Folder";
                            state.messageBoxVM.Message = ex.Message;
                            state.messageBoxVM.ButtonStyle = Names.OK;
                            windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                        return Task.CompletedTask;
                    });
                    Logger.LogActivity(Logger.ERROR, $"FileStore: LocateJobFolder() FAIL\n\t{ex.Message}");
                    throw;
                }
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// Get invoice file
        /// </summary>
        public static Task<string> GetInvoiceFilePath(int invoiceId, int clientId, int jobId, DateTime dateIssued, string clientName)
        {
            try
            {
                // Base path
                string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Traker", "Clients");

                string clientFolder = Path.Combine(basePath, $"{clientId}_{MakeSafeFolderName(clientName)}");
                if (!Directory.Exists(clientFolder))
                {
                    return null;
                }


                string invoicesFolder = Path.Combine(clientFolder, "Invoices");
                if (!Directory.Exists(invoicesFolder))
                {
                    return null;
                }

                // 5️⃣ Build expected filename prefix
                string expectedPrefix = $"INV-{invoiceId}_{clientId}_{jobId}_{dateIssued.ToString("dd-MM-yyyy")}_{dateIssued.ToString("HHmmss")}";

                // 6️⃣ Find matching file
                string file = Directory.GetFiles(invoicesFolder, "*.pdf").FirstOrDefault(f => Path.GetFileName(f).StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))!;

                return Task.FromResult(file);
            }
            catch (Exception ex)
            {
                Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get invoice Directory";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"FileStore: GetInvoiceFilePath() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        #endregion

        #region Sanitise Functions
        /// <summary>
        /// Sanitise folder name
        /// </summary>
        public static string MakeSafeFolderName(string name)
        {
            try
            {
                foreach (var c in Path.GetInvalidFileNameChars())
                {
                    name = name.Replace(c, '_');
                }
                return name.Trim();
            }
            catch (Exception ex)
            {
                Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Sanitise Folder Name";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"FileStore: MakeSafeFolderName() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        #endregion
    }
}