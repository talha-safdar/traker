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
    using System.Windows.Media;
    using Traker.Events;
    using Traker.Helper;

    public class EditInvoiceViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly DataService _dataService;
        #endregion

        #region Private View Variables
        private string _buttonText;
        private Brush _buttonBackground;
        private Brush _buttonHover;
        #endregion

        #region Private Class Field Variables
        private string _invoicePath;
        #endregion

        public DashboardModel SelectedJob; // data passed by DashboardVM

        public EditInvoiceViewModel(DataService dataService, IEventAggregator events)
        {
            _dataService = dataService;
            _events = events;

            _invoicePath = string.Empty;
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
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

            return base.OnInitializedAsync(cancellationToken);
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _invoicePath = await FileStore.GetInvoicePdfPath(
                Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).JobId), 
                SelectedJob.JobTitle, 
                Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).InvoiceId), 
                SelectedJob.ClientId, 
                SelectedJob.ClientName, 
                _dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).IssueDate);


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

        #region Public View Functions
        public async Task TogglePaid()
        {
            /*
             * Invoice status:
             * - Created 
             * - Paid
             */

            if (_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).Status == Names.Invoiced || _dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).Status == Names.Overdue)
            {
                //DateOny today = DateOnly.FromDateTime(DateTime.Now);
                //string formattedDate = today.ToString("dd/MM/yyyy");

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

        public Task DownloadInvoice()
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

                    // Optional: Notify user of success
                    System.Windows.MessageBox.Show("File saved successfully!");
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error saving file: {ex.Message}");
                }
            }

            return Task.CompletedTask;
        }

        public async Task DeleteInvoice()
        {
            await Database.DeleteInvoice(Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).InvoiceId), SelectedJob.JobId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
            await TryCloseAsync();
        }

        public async Task Exit()
        {
            await TryCloseAsync();
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
