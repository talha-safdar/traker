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
    public class EditInvoiceViewModel : Screen
    {
        #region Private Class Field Variables
        private readonly DataService _dataService;
        private WebView2 _pdfView;
        #endregion

        public DashboardModel SelectedJob; // data passed by DashboardVM

        public EditInvoiceViewModel(DataService dataService)
        {
            _dataService = dataService;
            _pdfView = new WebView2();
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);


            //FileStore.GetInvoicePdfPath(Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).InvoiceId), )


            var v = (Traker.Views.Edit.EditInvoiceView)view;
            var browser = v.PdfViewer;

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string pdfPath = Path.Combine(exeDir, "Assets/PDF", "ReplicationHelp.pdf");
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
                    | CoreWebView2PdfToolbarItems.ZoomOut;

            browser.Source = new Uri($"file:///{pdfPath.Replace("\\", "/")}");
        }
    }
}
