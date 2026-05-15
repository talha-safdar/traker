using Caliburn.Micro;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Data;
using Traker.Models;
using Traker.Services;

namespace Traker.ViewModels.Edit
{
    using Database;
    using Microsoft.VisualBasic;
    using Microsoft.Win32;
    using System.Globalization;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Traker.Events;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.States;

    public class EditInvoiceViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        #region Public Variables
        public DashboardModel SelectedJob;
        #endregion

        #region Private View Variables
        private string _buttonText;
        private Brush _buttonBackground;
        private Brush _buttonHover;
        #endregion

        #region Private Class Field Variables
        private string _invoicePath;
        #endregion

        public EditInvoiceViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            SelectedJob = new DashboardModel();

            _buttonText = string.Empty;
            _buttonBackground = (Brush)new BrushConverter().ConvertFrom("#FFFFFF")!;
            _buttonHover = (Brush)new BrushConverter().ConvertFrom("#FFFFFF")!;

            _invoicePath = string.Empty;
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                // set button text
                if (_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).Status == Names.Invoiced || _dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).Status == Names.Overdue)
                {
                    ButtonText = "✔ Mark as Paid";
                    ButtonBackground = (Brush)new BrushConverter().ConvertFrom("#16A34A")!;
                    ButtonHover = (Brush)new BrushConverter().ConvertFrom("#15803D")!;
                }
                else if (_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).Status == Names.Paid)
                {
                    ButtonText = "⏳ Mark as Not Paid";
                    ButtonBackground = (Brush)new BrushConverter().ConvertFrom("#F59E0B")!;
                    ButtonHover = (Brush)new BrushConverter().ConvertFrom("#D97706")!;
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Initialise Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditInvoiceViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            return base.OnInitializedAsync(cancellationToken);
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            try
            {
                _invoicePath = await FileStore.GetInvoiceFilePath(
                Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).InvoiceId),
                SelectedJob.ClientId,
                Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).JobId),
                _dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).IssueDate,
                SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName);

                // if no file found
                if (string.IsNullOrEmpty(_invoicePath))
                {
                    await TryCloseAsync();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                    {
                        _state.messageBoxVM.Symbol = 1;
                        _state.messageBoxVM.HeadMessage = "View Form";
                        _state.messageBoxVM.Message = $"Could not locate the invoice file\n\n{_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).InvoiceName}";
                        _state.messageBoxVM.ButtonStyle = Names.OK;
                        _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    Logger.LogActivity(Logger.WARNING, $"EditInvoiceViewModel: OnViewLoaded() FAIL file not found");
                    return;
                }


                var v = (Traker.Views.Edit.EditInvoiceView)view;
                var browser = v.PdfViewer;

                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string pdfPath = _invoicePath;
                Debug.WriteLine(pdfPath);
                await browser.EnsureCoreWebView2Async();
                browser.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
                browser.CoreWebView2.Settings.IsStatusBarEnabled = false;
                browser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                browser.CoreWebView2.Settings.HiddenPdfToolbarItems =
                          CoreWebView2PdfToolbarItems.Bookmarks
                        | CoreWebView2PdfToolbarItems.FitPage
                        | CoreWebView2PdfToolbarItems.PageLayout
                        | CoreWebView2PdfToolbarItems.PageSelector
                        | CoreWebView2PdfToolbarItems.Print
                        | CoreWebView2PdfToolbarItems.Rotate
                        | CoreWebView2PdfToolbarItems.Save
                        | CoreWebView2PdfToolbarItems.SaveAs
                        | CoreWebView2PdfToolbarItems.Search
                        | CoreWebView2PdfToolbarItems.ZoomIn
                        | CoreWebView2PdfToolbarItems.FullScreen
                        | CoreWebView2PdfToolbarItems.MoreSettings
                        | CoreWebView2PdfToolbarItems.ZoomOut;

                browser.Source = new Uri($"file:///{pdfPath.Replace("\\", "/")}");
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "View Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditInvoiceViewModel: OnViewLoaded() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Public View Functions
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    await TryCloseAsync();
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Exit Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditInvoiceViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task TogglePaid()
        {
            try
            {
                // Invoice status: Created, Paid
                if (_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).Status == Names.Invoiced || _dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).Status == Names.Overdue)
                {
                    ButtonText = "✔ Paid";
                    ButtonBackground = (Brush)new BrushConverter().ConvertFrom("#16A34A")!;
                    ButtonHover = (Brush)new BrushConverter().ConvertFrom("#15803D")!;

                    await Database.SetInvoiceStatus(Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).InvoiceId), "Paid", DateOnly.FromDateTime(DateTime.Now));
                    await _events.PublishOnUIThreadAsync(new RefreshDatabase());

                    ButtonText = "⏳ Marks as Not Paid";
                    ButtonBackground = (Brush)new BrushConverter().ConvertFrom("#F59E0B")!;
                    ButtonHover = (Brush)new BrushConverter().ConvertFrom("#D97706")!;
                }
                else if (_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).Status == Names.Paid)
                {
                    ButtonText = "⏳ Not Paid";
                    ButtonBackground = (Brush)new BrushConverter().ConvertFrom("#F59E0B")!;
                    ButtonHover = (Brush)new BrushConverter().ConvertFrom("#D97706")!;

                    await Database.SetInvoiceStatus(Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).InvoiceId), "Invoiced", null);
                    await _events.PublishOnUIThreadAsync(new RefreshDatabase());

                    ButtonText = "✔ Mark as Paid";
                    ButtonBackground = (Brush)new BrushConverter().ConvertFrom("#16A34A")!;
                    ButtonHover = (Brush)new BrushConverter().ConvertFrom("#15803D")!;
                }
                // add animation?
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Set Payment Status";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditClientViewModel: TogglePaid() FAIL\n\t{ex.Message}");
            }
        }

        public Task DownloadInvoice()
        {
            try
            {
                // 1. Initialize the SaveFileDialog
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FileName = $"Invoice_{FileStore.MakeSafeFolderName(SelectedJob.ClientName)}_{_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).IssueDate.ToString("dd-MM-yyyy")}_{_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).IssueDate.ToString("HHmmss")}.pdf", // Default name
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };


                // 2. Show the explorer dialog
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        // 3. Copy the file from your known path to the user's chosen path
                        File.Copy(_invoicePath, saveFileDialog.FileName, overwrite: true);

                        // Notify user of success
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                        {
                            _state.messageBoxVM.Symbol = 0;
                            _state.messageBoxVM.HeadMessage = "File Saved";
                            _state.messageBoxVM.Message = "File saved successfully!";
                            _state.messageBoxVM.ButtonStyle = Names.OK;
                            _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                    }
                    catch (Exception ex)
                    {
                        Execute.OnUIThreadAsync(async() =>
                        {
                            if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                            {
                                _state.messageBoxVM.Symbol = 2;
                                _state.messageBoxVM.HeadMessage = "Save File";
                                _state.messageBoxVM.Message = ex.Message;
                                _state.messageBoxVM.ButtonStyle = Names.OK;
                                await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                            }
                        });
                        Logger.LogActivity(Logger.ERROR, $"EditInvoiceViewModel: DownloadInvoice() FAIL inner\n\t{ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Save File";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditInvoiceViewModel: DownloadInvoice() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }

        public async Task DeleteInvoice()
        {
            try
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 0;
                    _state.messageBoxVM.HeadMessage = "Delete Invoice";
                    _state.messageBoxVM.Message = Names.DeleteInvoiceConfirmation;
                    _state.messageBoxVM.ButtonStyle = Names.NoYes;
                    await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }

                // if clicked yes
                if (_state.messageBoxVM.Output == true)
                {
                    await Task.Run(async() =>
                    {
                        await Database.DeleteInvoice(Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).InvoiceId), SelectedJob.JobId);
                        await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                        await TryCloseAsync();
                    });
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Delete Invoice";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditInvoiceViewModel: DeleteInvoice() FAIL\n\t{ex.Message}");
            }
        }

        public async Task Exit()
        {
            try
            {
                await TryCloseAsync();
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Exit";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditInvoiceViewModel: Exit() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Public View Variables
        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                _buttonText = value;
                NotifyOfPropertyChange(() => ButtonText);
            }
        }

        public Brush ButtonBackground
        {
            get { return _buttonBackground; }
            set
            {
                _buttonBackground = value;
                NotifyOfPropertyChange(() => ButtonBackground);
            }
        }

        public Brush ButtonHover
        {
            get { return _buttonHover; }
            set
            {
                _buttonHover = value;
                NotifyOfPropertyChange(() => ButtonHover);
            }
        }
        #endregion
    }
}