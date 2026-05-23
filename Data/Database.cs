using Caliburn.Micro;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Reflection;
using System.Windows;
using Traker.Helper;
using Traker.Models.Database;
using Traker.Services;
using Traker.States;

namespace Traker.Database
{
    using Dapper;
    using QuestPDF.Helpers;
    using System.Globalization;
    using System.Windows.Controls;
    using Traker.Models;

    /// <summary>
    /// Handle relationships between the UI and the database.
    /// </summary>
    public static class Database
    {
        #region Private Static Variables
        private static string _connectionString = String.Empty; // location of the database file
        #endregion

        #region Public Static Functions

        #region Set Up Database
        /// <summary>
        /// Set up the database by creating the database file if it doesn't exist
        /// </summary>
        public static async Task SetUpDatabaseBG()
        {
            await Task.Run(async () =>
            {
                try
                {
                    // set directory and database name
                    var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Traker");
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
                        _sqliteCommand.CommandText = sql; // set Schema.sql
                        _sqliteCommand.ExecuteNonQuery(); // execute Schema.sql
                        Logger.LogActivity(Logger.INFO, "Database: SetUpDatabase() DATABASE CONNECTED");
                    }
                    else
                    {
                        await Execute.OnUIThreadAsync(() =>
                        {
                            AppState state = IoC.Get<AppState>();
                            IWindowManager windowManager = IoC.Get<IWindowManager>();
                            if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                            {
                                state.messageBoxVM.Symbol = 2;
                                state.messageBoxVM.HeadMessage = "Database Setup";
                                state.messageBoxVM.Message = "Could not locate the database";
                                state.messageBoxVM.ButtonStyle = Names.OK;
                                state.messageBoxVM.Action = Names.Close;
                                windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                            }
                            return Task.CompletedTask;
                        });
                        Logger.LogActivity(Logger.WARNING, "Database: SetUpDatabase() DATABASE NOT CONNECTED");
                    }
                }
                catch (Exception ex)
                {
                    await Execute.OnUIThreadAsync(() =>
                    {
                        AppState state = IoC.Get<AppState>();
                        IWindowManager windowManager = IoC.Get<IWindowManager>();
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                        {
                            state.messageBoxVM.Symbol = 2;
                            state.messageBoxVM.HeadMessage = "Database Setup";
                            state.messageBoxVM.Message = ex.Message;
                            state.messageBoxVM.ButtonStyle = Names.OK;
                            state.messageBoxVM.Action = Names.Close;
                            windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                        return Task.CompletedTask;
                    });
                    Logger.LogActivity(Logger.ERROR, $"Database: SetUpDatabase() FAIL\n\t{ex.Message}");
                }
            });
        }
        #endregion

        #region Fetch Functions
        /// <summary>
        /// Fetch Business Table
        /// </summary>
        public async static Task<BusinessModel> FetchBusiness()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.QueryFirstAsync<BusinessModel>("SELECT * FROM Business WHERE UserId = @userId LIMIT 1;",
                    new { userId = await GetUserId() });
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Fetch Business Details";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: FetchBusiness() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Fetch Bank Table
        /// </summary>
        public async static Task<BankModel> FetchBank()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.QueryFirstAsync<BankModel>("SELECT * FROM Bank WHERE UserId = @userId LIMIT 1;",
                    new { userId = await GetUserId() });
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Fetch Bank Details";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: FetchBank() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Fetch User Table
        /// </summary>
        public async static Task<UserModel> FetchUser()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.QueryFirstAsync<UserModel>("SELECT * FROM User LIMIT 1;");
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Fetch User Details";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: FetchUser() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Fetch money information for dashboard
        /// </summary>
        public async static Task<MoneyInfoModel> FetchMoneyInformation()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                string sql = @"
                    SELECT
                        COUNT(CASE WHEN j.Status = @New THEN 1 END) AS NewJobsCount,
                        COUNT(CASE WHEN j.Status = @Done THEN 1 END) AS DoneJobsCount,
                        COUNT(CASE WHEN j.Status = @Active THEN 1 END) AS ActiveJobsCount,

                        COUNT(CASE 
                            WHEN j.Status = @Invoiced
                            AND EXISTS (
                                SELECT 1
                                FROM Invoices i
                                WHERE i.JobId = j.JobId
                                  AND i.Status <> @Paid
                            )
                            THEN 1 
                        END) AS InvoicedJobsCount,

                        COALESCE(SUM(j.FinalPrice), 0) AS GrossAmount,

                        COALESCE(SUM(CASE 
                            WHEN EXISTS (
                                SELECT 1
                                FROM Invoices i
                                WHERE i.JobId = j.JobId
                                  AND i.Status = @Paid
                            )
                            THEN j.FinalPrice ELSE 0 
                        END), 0) AS ReceivedAmount,

                        COALESCE(SUM(CASE 
                            WHEN j.Status = @Done
                              OR (
                                  j.Status = @Invoiced
                                  AND EXISTS (
                                      SELECT 1
                                      FROM Invoices i
                                      WHERE i.JobId = j.JobId
                                        AND i.Status <> @Paid
                                  )
                              )
                              OR EXISTS (
                                  SELECT 1
                                  FROM Invoices i
                                  WHERE i.JobId = j.JobId
                                    AND i.DueDate < @Today
                                    AND i.Status = @Overdue
                              )
                            THEN j.FinalPrice ELSE 0 
                        END), 0) AS OutstandingAmount,

                        COALESCE(SUM(CASE 
                            WHEN j.Status = @Invoiced
                            AND EXISTS (
                                SELECT 1
                                FROM Invoices i
                                WHERE i.JobId = j.JobId
                                  AND i.DueDate < @Today
                                  AND i.Status <> @Paid
                            )
                            THEN j.FinalPrice ELSE 0 
                        END), 0) AS OverdueAmount

                    FROM Jobs j;
                ";

                var moneyInfo = await conn.QueryFirstAsync<MoneyInfoModel>(
                    sql,
                    new
                    {
                        New = "New",
                        Done = "Done",
                        Active = "Active",
                        Invoiced = "Invoiced",
                        Paid = "Paid",
                        Overdue = "Overdue",
                        Today = DateTime.Today.ToString("yyyy-MM-dd")
                    });

                return moneyInfo;
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Fetch Money Information";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: FetchMoneyInformation() FAIL\n\t{ex.Message}");
                throw;
            }
        }
               
        /// <summary>
        /// Get jobs by client id
        /// </summary>
        /// <param name="clientId"></param>
        public async static Task<List<JobsModel>> FetchJobsByClientId(int clientId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return (await conn.QueryAsync<JobsModel>("SELECT * FROM Jobs WHERE ClientId = @clientId;",
                    new { clientId = clientId })).ToList();
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get Jobs By Client Id";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetJobsByClientId() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Dashboard data query
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="statusFilter"></param>
        /// <param name="clientTypeFilter"></param>
        public async static Task<List<DashboardModel>> FetchDashboardRows(int currentPage, int pageSize, string? sortBy, string? sortDirection, string? statusFilter, string? clientTypeFilter)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);

                int offset = (currentPage - 1) * pageSize;

                string orderBy;

                if (string.IsNullOrWhiteSpace(sortBy))
                {
                    orderBy = "j.JobId DESC";
                }
                else
                {
                    string direction = sortDirection == "ASC" ? "ASC" : "DESC";

                    orderBy = sortBy switch
                    {
                        "ClientName" =>
                            $"c.FullName COLLATE NOCASE {direction}, j.JobId DESC",

                        "ClientType" =>
                            $"c.Type COLLATE NOCASE {direction}, j.JobId DESC",

                        "JobTitle" =>
                            $"j.Title COLLATE NOCASE {direction}, j.JobId DESC",

                        "JobStatus" =>
                            $"j.Status {direction}, j.JobId DESC",

                        "JobPrice" =>
                            $"j.FinalPrice {direction}, j.JobId DESC",

                        "DueDate" =>
                            $"j.DueDate {direction}, j.JobId DESC",

                        "CreatedDate" =>
                            $"j.CreatedDate {direction}, j.JobId DESC",

                        "BusinessType" =>
                            $"c.Type COLLATE NOCASE {direction}, j.JobId DESC",

                        "StatusFlow" =>
                            $@"
                            CASE LOWER(COALESCE(NULLIF(i.Status, ''), j.Status))
                                WHEN 'new' THEN 1
                                WHEN 'active' THEN 2
                                WHEN 'done' THEN 3
                                WHEN 'invoiced' THEN 4
                                WHEN 'overdue' THEN 5
                                WHEN 'paid' THEN 6
                                ELSE 99
                            END {direction},
                            j.JobId DESC",

                        _ =>
                            "j.JobId DESC"
                    };
                }

                string sql = $@"
                    SELECT 
                        c.ClientId,
                        c.Type AS ClientType,
                        c.FullName AS ClientName,
                        c.CompanyName,
                        c.Email AS ClientEmail,
                        c.PhoneNumber AS ClientPhone,
                        c.BillingAddress AS Address,
                        c.City,
                        c.Postcode,
                        c.Country,
                        c.IsActive,

                        j.JobId,
                        j.Title AS JobTitle,
                        j.Description AS JobDescription,
                        j.FinalPrice AS Price,
                        j.Status AS JobStatus,
                        j.StartDate,
                        j.DueDate,
                        j.AmountReceived,
                        j.CreatedDate,

                        i.PaidDate,

                        CASE 
                            WHEN NULLIF(i.Status, '') IS NOT NULL THEN i.DueDate
                            ELSE NULL
                        END AS InvoiceDueDate,


                        CASE 
                            WHEN i.InvoiceId IS NOT NULL AND i.IsDeleted = 0 THEN 1 
                            ELSE 0 
                        END AS HasInvoice,

                        COALESCE(NULLIF(i.Status, ''), 'Not invoiced') AS InvoiceStatus

                    FROM Clients c

                    LEFT JOIN Jobs j
                        ON c.ClientId = j.ClientId

                    LEFT JOIN Invoices i
                        ON j.JobId = i.JobId
                        AND i.IsDeleted = 0

                    WHERE
                        (NULLIF(@ClientTypeFilter, '') IS NULL OR c.Type = @ClientTypeFilter)

                        AND
                        (
                            NULLIF(@StatusFilter, '') IS NULL

                            OR
                            (
                                @StatusFilter IN ('New', 'Active', 'Done', 'Invoiced')
                                AND j.Status = @StatusFilter
                            )

                            OR
                            (
                                @StatusFilter IN ('Overdue', 'Paid')
                                AND i.Status = @StatusFilter
                            )
                        )

                    ORDER BY {orderBy}

                    LIMIT @PageSize OFFSET @Offset;
                ";

                statusFilter = string.IsNullOrWhiteSpace(statusFilter) ? null : statusFilter;
                clientTypeFilter = string.IsNullOrWhiteSpace(clientTypeFilter) ? null : clientTypeFilter;

                var rows = await conn.QueryAsync<DashboardModel>(
                    sql,
                    new
                    {
                        PageSize = pageSize,
                        Offset = offset,
                        StatusFilter = statusFilter,
                        ClientTypeFilter = clientTypeFilter
                    });

                return rows.ToList();
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Database: SortList()";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: SortList() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        #endregion

        #region Get Functions
        /// <summary>
        /// Grabs the User Id
        /// </summary>
        public async static Task<int> GetUserId()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.ExecuteScalarAsync<int>("SELECT UserId FROM User LIMIT 1;");
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get User Id";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetUserId() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Get User name
        /// </summary>
        public async static Task<string> GetUserName()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.ExecuteScalarAsync<string>("SELECT FullName FROM User LIMIT 1;") ?? "Admin";
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get User Name";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetUserName() FAIL\n\t{ex.Message}");
                throw;
            }
        }
               
        /// <summary>
        /// Get indiviudal client
        /// </summary>
        /// <param name="clientId"></param>
        public async static Task<ClientsModel> GetClient(int clientId)
        {
            try 
            {                
                using var conn = new SqliteConnection(_connectionString);
                return await conn.QueryFirstAsync<ClientsModel>("SELECT * FROM Clients WHERE ClientId = @clientId LIMIT 1;",
                    new { clientId = clientId });
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get Client Details";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetClient() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get individual ivoice
        /// </summary>
        /// <param name="jobId"></param>
        public async static Task<InvoicesModel> GetInvoice(int jobId)
        {
            try
            {
                //return conn.QueryFirstAsync<InvoicesModel>("SELECT * FROM Invoices WHERE JobId IN (SELECT JobId FROM Jobs WHERE ClientId IN (SELECT ClientId FROM Clients)) LIMIT 1;");
                using var conn = new SqliteConnection(_connectionString);
                return await conn.QueryFirstAsync<InvoicesModel>("SELECT * FROM Invoices WHERE JobId = @jobId LIMIT 1;", 
                    new { jobId = jobId });
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Fetch Invoice Details";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: FetchInvoices() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get last client row number for clientId
        /// </summary>
        /// <returns></returns>
        public async static Task<int> GetLastClientlastRowId()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.ExecuteScalarAsync<int>("SELECT ClientId FROM Clients ORDER BY ClientId DESC LIMIT 1;");
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get Last Client Row Number";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetLastClientRowNumber() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get number of jobs
        /// </summary>
        /// <returns></returns>
        public async static Task<int> GetJobsCount()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Jobs;");
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get Jobs Count";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetJobsCount() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Get invoice id by job id
        /// </summary>
        /// <param name="jobId"></param>
        public async static Task<int> GetInvoiceIdByJobId(int jobId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.ExecuteScalarAsync<int>("SELECT InvoiceId FROM Invoices WHERE JobId = @jobId AND IsDeleted = 0;",
                    new { jobId = jobId });
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get Invoice Id By Job Id";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetInvoiceIdByJobId() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get invoice status by job id
        /// </summary>
        /// <param name="jobId"></param>
        public async static Task<string> GetInvoiceStatusByJobId(int jobId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.ExecuteScalarAsync<string>("SELECT Status FROM Invoices WHERE JobId = @jobId AND IsDeleted = 0;",
                    new { jobId = jobId });
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get Invoice Status By Job Id";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetInvoiceStatusByJobId() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get invoice issue date by job id
        /// </summary>
        /// <param name="jobId"></param>
        public async static Task<DateTime> GetInvoiceIssueDateByJobId(int jobId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.ExecuteScalarAsync<DateTime>("SELECT IssueDate FROM Invoices WHERE JobId = @jobId AND IsDeleted = 0;",
                    new { jobId = jobId });
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get Invoice Issue Date By Job Id";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetInvoiceIssueDateByJobId() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get invoice name by job id
        /// </summary>
        /// <param name="jobId"></param>
        public async static Task<string> GetInvoiceNameByJobId(int jobId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return (await conn.ExecuteScalarAsync<string>("SELECT InvoiceName FROM Invoices WHERE JobId = @jobId AND IsDeleted = 0;",
                    new { jobId = jobId }))?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Get Invoice Name By Job Id";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: GetInvoiceNameByJobId() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        #endregion

        #region Check Functions
        /// <summary>
        /// Checks user database on startup if empty then
        /// prompt setup window (after deleting the databse file)
        /// </summary>
        public async static Task<bool> CheckUserDatabase()
        {
            bool isSuccessful = false;

            try
            {
                await Task.Run(async() =>
                {
                    // set directory and database name
                    var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Traker");
                    if (Directory.Exists(folder) == false)
                    {
                        // to test
                        isSuccessful = false;
                        return;
                    }

                    var dbPath = Path.Combine(folder, "traker.db");
                     _connectionString = $"Data Source={dbPath}";


                    using (var conn = new SqliteConnection(_connectionString))
                    {
                        // query database
                        int countUser = await conn.ExecuteScalarAsync<int>(
                        @"SELECT COUNT(*) FROM 'User'"
                        );

                        int countBusiness = await conn.ExecuteScalarAsync<int>(
                        @"SELECT COUNT(*) FROM 'Business'"
                        );

                        int countBank = await conn.ExecuteScalarAsync<int>(
                        @"SELECT COUNT(*) FROM 'bank'"
                        );

                        // if any table is empty delete database file to trigger setup window
                        if (countUser == 0 || countBusiness == 0 || countBank == 0)
                        {
                            SqliteConnection.ClearAllPools();

                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            //File.Delete(dbPath);

                            isSuccessful = false;
                        }
                        else
                        {
                            isSuccessful = true;
                        }
                    }
                });                
            }
            catch (Exception ex)
            {
                AppState state = IoC.Get<AppState>();
                IWindowManager windowManager = IoC.Get<IWindowManager>();
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                {
                    state.messageBoxVM.Symbol = 2;
                    state.messageBoxVM.HeadMessage = "Fetch Clients Tables";
                    state.messageBoxVM.Message = ex.Message;
                    state.messageBoxVM.ButtonStyle = Names.OK;
                    state.messageBoxVM.Action = Names.Close;
                    await windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                isSuccessful = false;
                Logger.LogActivity(Logger.ERROR, $"Database: CheckUserDatabase() FAIL\n\t{ex.Message}");
            }
            return isSuccessful;
        }
       
        /// <summary>  
        /// Check if user exists
        /// </summary>
        public async static Task<bool> CheckUserExists()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM User);");
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Check User Exists";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: CheckUserExists() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Check if job has invoice
        /// </summary>
        /// <param name="jobId"></param>
        public async static Task<bool> CheckIfJobHasInvoice(int jobId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                return await conn.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM Invoices WHERE JobId = @jobId AND IsDeleted = 0);",
                    new { jobId = jobId });
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Check If Job Has Invoice";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: CheckIfJobHasInvoice() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        #endregion

        #region Create Functions
        /// <summary>
        /// Create invoice
        /// </summary>
        public static Task CreateInvoice(int clientId, int jobId, decimal subtotal, int taxAmount, decimal totalAmount, DateOnly dueDate, string billingName, string billingAddress, string billingCity, string billingPostcode, string billingCountry, DateTime dateIssued)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                conn.Execute("PRAGMA foreign_keys = ON;");

                using var transaction = conn.BeginTransaction();

                var nextNumber =
                    conn.ExecuteScalar<long>(
                        "SELECT IFNULL(MAX(InvoiceNumber), 0) FROM Invoices;",
                        transaction: transaction) + 1;

                const string sql = @"
                    INSERT INTO Invoices
                    (
                        JobId,
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
                        Status
                    )
                    VALUES
                    (
                        @JobId,
                        @InvoiceNumber,
                        @Subtotal,
                        @TaxAmount,
                        @TotalAmount,
                        @IssueDate,
                        @DueDate,
                        @BillingName,
                        @BillingAddress,
                        @BillingCity,
                        @BillingPostcode,
                        @BillingCountry,
                        @Status
                    );";

                conn.Execute(sql, new
                {
                    JobId = jobId,
                    InvoiceNumber = nextNumber,
                    Subtotal = subtotal,
                    TaxAmount = taxAmount,
                    TotalAmount = totalAmount,
                    IssueDate = dateIssued,
                    DueDate = dueDate.ToString("dd-MM-yyyy"),
                    BillingName = billingName,
                    BillingAddress = billingAddress,
                    BillingCity = billingCity,
                    BillingPostcode = billingPostcode,
                    BillingCountry = billingCountry,
                    Status = "Invoiced"
                }, transaction);

                transaction.Commit();

                // also update the status in Jobs table to "Invoiced"
                SetJobStatus(Names.Invoiced, clientId, jobId);
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
                        state.messageBoxVM.HeadMessage = "Create Invoice";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: CreateInvoice() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Create user
        /// </summary>
        public static Task CreateUser(string fullName, string email, string phone)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    INSERT INTO User
                    (
                        FullName,
                        Email,
                        Phone
                    )
                    VALUES
                    (
                        @FullName,
                        @Email,
                        @Phone
                    );";

                conn.Execute(sql, new
                {
                    FullName = fullName,
                    Email = email,
                    Phone = phone
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
                        state.messageBoxVM.HeadMessage = "Create User";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: Createuser() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Create business
        /// </summary>
        public static Task CreateBusiness(int userId, string businessName, string businessType, string country, string city, string address, string postcode, string vatNumber, string registrationNumber)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    INSERT INTO Business
                    (
                        UserId,
                        BusinessName,
                        BusinessType,
                        Country,
                        City,
                        Address,
                        Postcode,
                        VatNumber,
                        RegistrationNumber
                    )
                    VALUES
                    (
                        @UserId,
                        @BusinessName,
                        @BusinessType,
                        @Country,
                        @City,
                        @Address,
                        @Postcode,
                        @VatNumber,
                        @RegistrationNumber
                    );";

                conn.Execute(sql, new
                {
                    UserId = userId,
                    BusinessName = businessName,
                    BusinessType = businessType,
                    Country = country,
                    City = city,
                    Address = address,
                    Postcode = postcode,
                    VatNumber = vatNumber,
                    RegistrationNumber = registrationNumber
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
                        state.messageBoxVM.HeadMessage = "Create Business";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: CreateBusiness() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Create bank
        /// </summary>
        public static Task CreateBank(int userId, string bankName, string accountName, string accountNumber, string sortcode, string IBAN, string BIC)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    INSERT INTO Bank
                    (
                        UserId,
                        BankName,
                        AccountName,
                        AccountNumber,
                        SortCode,
                        IBAN,
                        BIC
                    )
                    VALUES
                    (
                        @UserId,
                        @BankName,
                        @AccountName,
                        @AccountNumber,
                        @SortCode,
                        @IBAN,
                        @BIC
                    );";

                conn.Execute(sql, new
                {
                    UserId = userId,
                    BankName = bankName,
                    AccountName = accountName,
                    AccountNumber = accountNumber,
                    SortCode = sortcode,
                    IBAN,
                    BIC
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
                        state.messageBoxVM.HeadMessage = "Create Bank";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        state.messageBoxVM.Action = Names.Close;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: CreateBank() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Add Functions
        /// <summary>
        /// Add Client as individual type
        /// </summary>
        public static Task<List<int>> AddIndividualClient(string clientName, string clientType, string jobTitle, decimal finalPrice, DateOnly dueDate)
        {
            try
            {
                // list to be passed on add client to get hold of clientId and jobid
                List<int> clientJobIds = new();

                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                conn.Execute("PRAGMA foreign_keys = ON;");

                // work all at once, if one query fails it rolls back
                // if you don't care about failures you can avoid transaction
                using var tx = conn.BeginTransaction();

                try
                {
                    // insert into Clients table
                    const string insertClientSql = @"
                        INSERT INTO Clients
                        (
                            FullName,
                            Type,
                            CreatedDate
                        )
                        VALUES
                        (
                            @FullName,
                            @Type,
                            @CreatedDate
                        );

                        SELECT last_insert_rowid();";

                    // get last inserted client id
                    var clientId = conn.ExecuteScalar<long>(insertClientSql, new
                    {
                        FullName = clientName,
                        Type = clientType,
                        CreatedDate = DateTime.Now.ToString("yyyy-MM-dd")
                    }, tx);

                    clientJobIds.Add((int)clientId);

                    // insert into jobs table
                    const string insertJobSql = @"
                        INSERT INTO Jobs
                        (
                            ClientId,
                            Title,
                            Status,
                            FinalPrice,
                            CreatedDate,
                            DueDate
                        )
                        VALUES
                        (
                            @ClientId,
                            @Title,
                            @Status,
                            @FinalPrice,
                            @CreatedDate,
                            @DueDate
                        );

                        SELECT last_insert_rowid();";

                    // get last inserted job id based on current clientId
                    var jobId = conn.ExecuteScalar<long>(insertJobSql, new
                    {
                        ClientId = clientId,
                        Title = jobTitle,
                        Status = "New",
                        FinalPrice = finalPrice,
                        CreatedDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        DueDate = dueDate.ToString("yyyy-MM-dd")
                    }, tx);

                    clientJobIds.Add((int)jobId); // add it to the list that will be passed to addclientVM

                    // Commit both together
                    tx.Commit();

                    return Task.FromResult(clientJobIds);
                }
                catch
                {
                    tx.Rollback();
                    throw;
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
                        state.messageBoxVM.HeadMessage = "Add Client";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: AddIndividualClient() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add Client as company type
        /// </summary>
        public static Task<List<int>> AddCompanyClient(string companyName, string clientType, string jobTitle, decimal finalPrice, DateOnly dueDate)
        {
            try
            {
                List<int> clientJobIds = new();

                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                conn.Execute("PRAGMA foreign_keys = ON;");

                using var tx = conn.BeginTransaction();

                try
                {
                    const string insertClientSql = @"
                        INSERT INTO Clients
                        (
                            CompanyName,
                            Type,
                            CreatedDate
                        )
                        VALUES
                        (
                            @CompanyName,
                            @Type,
                            @CreatedDate
                        );

                        SELECT last_insert_rowid();";

                    var clientId = conn.ExecuteScalar<long>(insertClientSql, new
                    {
                        CompanyName = companyName,
                        Type = clientType,
                        CreatedDate = DateTime.Now.ToString("yyyy-MM-dd")
                    }, tx);

                    clientJobIds.Add((int)clientId);

                    const string insertJobSql = @"
                        INSERT INTO Jobs
                        (
                            ClientId,
                            Title,
                            Status,
                            FinalPrice,
                            CreatedDate,
                            DueDate
                        )
                        VALUES
                        (
                            @ClientId,
                            @Title,
                            @Status,
                            @FinalPrice,
                            @CreatedDate,
                            @DueDate
                        );

                        SELECT last_insert_rowid();";

                    var jobId = conn.ExecuteScalar<long>(insertJobSql, new
                    {
                        ClientId = clientId,
                        Title = jobTitle,
                        Status = "New",
                        FinalPrice = finalPrice,
                        CreatedDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        DueDate = dueDate.ToString("yyyy-MM-dd")
                    }, tx);

                    clientJobIds.Add((int)jobId);

                    tx.Commit();

                    return Task.FromResult(clientJobIds);
                }
                catch
                {
                    tx.Rollback();
                    throw;
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
                        state.messageBoxVM.HeadMessage = "Add Client";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: AddCompanyClient() FAIL\n\t{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add a job to a client
        /// </summary>
        public static Task<int> AddNewJobToClient(int clientId, string JobTitle, decimal finalPrice, DateOnly dueDate)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    INSERT INTO Jobs
                    (
                        ClientId,
                        Title,
                        Status,
                        FinalPrice,
                        CreatedDate,
                        DueDate
                    )
                    VALUES
                    (
                        @ClientId,
                        @Title,
                        @Status,
                        @FinalPrice,
                        @CreatedDate,
                        @DueDate
                    );

                    SELECT last_insert_rowid();";

                var jobId = conn.ExecuteScalar<long>(sql, new
                {
                    ClientId = clientId,
                    Title = JobTitle,
                    Status = "New",
                    FinalPrice = finalPrice,
                    CreatedDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    DueDate = dueDate.ToString("yyyy-MM-dd")
                });

                return Task.FromResult((int)jobId);
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
                        state.messageBoxVM.HeadMessage = "Add Job";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: AddNewJobToClient() FAIL\n\t{ex.Message}");
                throw;
            }
        }
        #endregion

        #region Edit Functions
        /// <summary>
        /// Edit client
        /// </summary>
        public static Task EditClient(int clientId, string type, string fullName, string email, string companyName, string phoneNumber, string billingAddress, string city, string postcode, string country, bool isActive)
        {
            // in the future replace the long ass arguments with a variable list :)

            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE Clients
                    SET Type = @Type,
                        FullName = @FullName,
                        Email = @Email,
                        CompanyName = @CompanyName,
                        PhoneNumber = @PhoneNumber,
                        BillingAddress = @BillingAddress,
                        City = @City,
                        Postcode = @Postcode,
                        Country = @Country,
                        IsActive = @IsActive
                    WHERE ClientId = @ClientId;";

                conn.Execute(sql, new
                {
                    ClientId = clientId,
                    Type = type,
                    FullName = fullName,
                    Email = email,
                    CompanyName = companyName,
                    PhoneNumber = phoneNumber,
                    BillingAddress = billingAddress,
                    City = city,
                    Postcode = postcode,
                    Country = country,
                    IsActive = isActive
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
                        state.messageBoxVM.HeadMessage = "Edit Client";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: EditClient() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Edit job
        /// </summary>
        public static Task EditJob(int jobId, string jobTitle, string jobDescription, string status, string price, string amountReceived, DateOnly startDate, DateOnly dueDate)
        {
            // in the future replace the long ass arguments with a variable list :)

            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE Jobs
                    SET Title = @Title,
                        Description = @Description,
                        Status = @Status,
                        FinalPrice = @FinalPrice,
                        AmountReceived = @AmountReceived,
                        StartDate = @StartDate,
                        DueDate = @DueDate
                    WHERE JobId = @JobId;";

                conn.Execute(sql, new
                {
                    JobId = jobId,
                    Title = jobTitle,
                    Description = jobDescription,
                    Status = status,
                    FinalPrice = price,
                    AmountReceived = amountReceived,
                    StartDate = startDate,
                    DueDate = dueDate
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
                        state.messageBoxVM.HeadMessage = "Edit Job";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: EditJob() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Edit user
        /// </summary>
        public static Task EditUser(int userId, string fullName, string email, string phone, string businessType)
        {
            // in the future replace the long ass arguments with a variable list :)
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string updateUserSql = @"
                    UPDATE User
                    SET FullName = @FullName,
                        Email = @Email,
                        Phone = @Phone
                    WHERE UserId = @UserId;";

                conn.Execute(updateUserSql, new
                {
                    UserId = userId,
                    FullName = fullName,
                    Email = email,
                    Phone = phone
                });

                const string updateBusinessSql = @"
                    UPDATE Business
                    SET BusinessType = @BusinessType
                    WHERE UserId = @UserId;";

                conn.Execute(updateBusinessSql, new
                {
                    UserId = userId,
                    BusinessType = businessType
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
                        state.messageBoxVM.HeadMessage = "Edit User";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: EditUser() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Edit business
        /// </summary>
        public static Task EditBusiness(int userId, string businessName, string country, string city, string address, string postcode, string vatNumber, string registrationNumber)
        {
            // in the future replace the long ass arguments with a variable list :)

            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE Business
                    SET BusinessName = @BusinessName,
                        Country = @Country,
                        City = @City,
                        Address = @Address,
                        Postcode = @Postcode,
                        VatNumber = @VatNumber,
                        RegistrationNumber = @RegistrationNumber
                    WHERE UserId = @UserId;";

                conn.Execute(sql, new
                {
                    UserId = userId,
                    BusinessName = businessName,
                    Country = country,
                    City = city,
                    Address = address,
                    Postcode = postcode,
                    VatNumber = vatNumber,
                    RegistrationNumber = registrationNumber
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
                        state.messageBoxVM.HeadMessage = "Edit Business";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: EditBusiness() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Edit bank
        /// </summary>
        public static Task EditBank(int userId, string bankName, string accountName, string accountNumber, string sortcode, string IBAN, string BIC)
        {
            // in the future replace the long ass arguments with a variable list :)

            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE Bank
                    SET BankName = @BankName,
                        AccountName = @AccountName,
                        AccountNumber = @AccountNumber,
                        SortCode = @SortCode,
                        IBAN = @IBAN,
                        BIC = @BIC
                    WHERE UserId = @UserId;";

                conn.Execute(sql, new
                {
                    UserId = userId,
                    BankName = bankName,
                    AccountName = accountName,
                    AccountNumber = accountNumber,
                    SortCode = sortcode,
                    IBAN,
                    BIC
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
                        state.messageBoxVM.HeadMessage = "Edit Bank";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: EditBank() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Set Functions
        /// <summary>
        /// Set the status of a job
        /// </summary>
        public static Task SetJobStatus(string status, int clientId, int jobId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                conn.Execute("PRAGMA foreign_keys = ON;");

                if (status == Names.New)
                {
                    const string sql = @"
                        UPDATE Jobs
                        SET Status = @Status,
                            StartDate = @StartDate,
                            CompletedDate = @CompletedDate
                        WHERE JobId = @JobId;";

                    conn.Execute(sql, new
                    {
                        JobId = jobId,
                        Status = status,
                        StartDate = DateTime.MinValue,
                        CompletedDate = DateTime.MinValue
                    });
                }
                else if (status == Names.Active)
                {
                    const string sql = @"
                        UPDATE Jobs
                        SET Status = @Status,
                            StartDate = @StartDate
                        WHERE JobId = @JobId;";

                    conn.Execute(sql, new
                    {
                        JobId = jobId,
                        Status = status,
                        StartDate = DateTime.Now.ToString("yyyy-MM-dd")
                    });
                }
                else if (status == Names.Done)
                {
                    const string sql = @"
                        UPDATE Jobs
                        SET Status = @Status,
                            CompletedDate = @CompletedDate
                        WHERE JobId = @JobId;";

                    conn.Execute(sql, new
                    {
                        JobId = jobId,
                        Status = status,
                        CompletedDate = DateTime.Now.ToString("yyyy-MM-dd")
                    });
                }
                else if (status == Names.Invoiced)
                {
                    const string sql = @"
                        UPDATE Jobs
                        SET Status = @Status
                        WHERE JobId = @JobId;";

                    conn.Execute(sql, new
                    {
                        JobId = jobId,
                        Status = status
                    });
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
                        state.messageBoxVM.HeadMessage = "Set Job Status";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: SetJobStatus() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// save invoice file name
        /// </summary>
        public static Task SetInvoiceName(int invoiceId, string invoiceName)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE Invoices
                    SET InvoiceName = @InvoiceName
                    WHERE InvoiceId = @InvoiceId;";

                conn.Execute(sql, new
                {
                    InvoiceName = invoiceName,
                    InvoiceId = invoiceId
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
                        state.messageBoxVM.HeadMessage = "Set Invoice Name";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: SetInvoiceName() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// set invoice status
        /// </summary>
        public static Task SetInvoiceStatus(int invoiceId, string status, DateOnly? paidDate)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE Invoices
                    SET Status = @Status,
                        PaidDate = @PaidDate
                    WHERE InvoiceId = @InvoiceId;";

                conn.Execute(sql, new
                {
                    Status = status,
                    PaidDate = paidDate,
                    InvoiceId = invoiceId
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
                        state.messageBoxVM.HeadMessage = "Set Invoice Status";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: SetInvoiceStatus() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// save client folder name
        /// </summary>
        public static Task SetClientFolderName(int clientId, string folderName)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE Clients
                    SET FolderName = @FolderName
                    WHERE ClientId = @ClientId;";

                conn.Execute(sql, new
                {
                    ClientId = clientId,
                    FolderName = folderName
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
                        state.messageBoxVM.HeadMessage = "Set Client Folder";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: SetClientFolderName() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// save job folder name
        /// </summary>
        public static Task SetJobFolderName(int jobId, string folderName)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE Jobs
                    SET FolderName = @FolderName
                    WHERE JobId = @JobId;";

                conn.Execute(sql, new
                {
                    JobId = jobId,
                    FolderName = folderName
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
                        state.messageBoxVM.HeadMessage = "Set Job Folder";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: SetJobFolderName() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Delete Functions
        /// <summary>
        /// Delete a client
        /// </summary>
        public static Task DeleteClient(int clientId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                conn.Execute("PRAGMA foreign_keys = ON;");

                using var tx = conn.BeginTransaction();

                try
                {
                    const string sql = @"
                        DELETE FROM Clients
                        WHERE ClientId = @ClientId;";

                    conn.Execute(sql, new
                    {
                        ClientId = clientId
                    }, tx);

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
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
                        state.messageBoxVM.HeadMessage = "Delete Client";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: DeleteClient() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete a job
        /// </summary>
        public static Task DeleteJob(int jobId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                conn.Execute("PRAGMA foreign_keys = ON;");

                using var tx = conn.BeginTransaction();

                try
                {
                    const string sql = @"
                        DELETE FROM Jobs
                        WHERE JobId = @JobId;";

                    conn.Execute(sql, new
                    {
                        JobId = jobId
                    }, tx);

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
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
                        state.messageBoxVM.HeadMessage = "Delete Job";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: DeleteJob() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete invoice
        /// </summary>
        public static Task DeleteInvoice(int invoiceId, int jobId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                conn.Execute("PRAGMA foreign_keys = ON;");

                using var tx = conn.BeginTransaction();

                try
                {
                    const string deleteInvoiceSql = @"
                        DELETE FROM Invoices
                        WHERE InvoiceId = @InvoiceId;";

                    conn.Execute(deleteInvoiceSql, new
                    {
                        InvoiceId = invoiceId
                    }, tx);

                    const string updateJobStatusSql = @"
                        UPDATE Jobs
                        SET Status = @Status
                        WHERE JobId = @JobId;";

                    conn.Execute(updateJobStatusSql, new
                    {
                        JobId = jobId,
                        Status = "Active"
                    }, tx);

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
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
                        state.messageBoxVM.HeadMessage = "Delete Invoice";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"Database: DeleteInvoice() FAIL\n\t{ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }
        #endregion

        #endregion
    }
}