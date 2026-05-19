using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models;

namespace Traker.ViewModels
{
    using Database;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using Traker.Data;
    using Traker.Events;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.Services;
    using Traker.States;

    public class CreateInvoiceViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        #region Private View Variables
        private string _billingName;
        private string _billingAddress;
        private string _billingCity;
        private string _billingPostcode;
        private string _billingCountry;
        private string _dueDate;
        private string _subtotal; // subTotAmount#2 used for UI with proper currency symbol and decimal
        private string _vatValue;
        private string _totalAmount; // totAmount#1 used for UI with proper currency symbol and decimal
        private List<string> _invoiceStatusEdit = new List<string> { "0%", "5%", "20%" };

        // submit button
        private bool _enableSubmitBtn;
        private double _opacitySubmitBtn;
        #endregion

        #region Public Variables
        public DashboardModel SelectedJob; // data passed by DashboardVM
        #endregion

        #region Private Class Field Variables
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;

        private decimal _subtotalAmountDb; // subTotAmount#1 used for database
        private decimal _totalAmountDb; // totAmount#1 used for database
        #endregion

        public CreateInvoiceViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            SelectedJob = new DashboardModel(); // to be improved to add useful properties

            _billingName = string.Empty;
            _billingAddress = string.Empty;
            _billingCity = string.Empty;
            _billingPostcode = string.Empty;
            _billingCountry = string.Empty;
            _dueDate = string.Empty;
            _subtotal = "0";
            _vatValue = "0%";
            _totalAmount = "0";
            _subtotalAmountDb = 0.0m;
            _totalAmountDb = 0.0m;
            _windowManager = windowManager;
            _invoiceStatusEdit = new List<string> { "0%", "5%", "20%" };
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                CanSubmit(); // check wether to enable/disbale submit button
                BillingName = SelectedJob.ClientName;
                BillingAddress = SelectedJob.Address;
                BillingCity = SelectedJob.City;
                BillingPostcode = SelectedJob.Postcode;
                BillingCountry = SelectedJob.Country;
                DueDate = SelectedJob.DueDate.AddDays(7).ToString();
                _subtotalAmountDb = decimal.Parse(SelectedJob.Price.ToString(), NumberStyles.Currency, CultureInfo.CurrentCulture);
                Subtotal = (decimal.Parse(SelectedJob.Price.ToString(), NumberStyles.Currency, CultureInfo.CurrentCulture)).ToString("C");
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
                Logger.LogActivity(Logger.ERROR, $"CreateInvoiceViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            return base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion

        #region Public View Functions
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    if (
                        (string.IsNullOrEmpty(BillingName) == false) && SelectedJob.ClientName != BillingName ||
                        (string.IsNullOrEmpty(BillingAddress) == false) && SelectedJob.Address != BillingAddress ||
                        (string.IsNullOrEmpty(BillingCity) == false) && SelectedJob.City != BillingCity ||
                        (string.IsNullOrEmpty(BillingCountry) == false) && SelectedJob.Country != BillingCountry ||
                        (string.IsNullOrEmpty(BillingPostcode) == false) && SelectedJob.Postcode != BillingPostcode ||
                        (string.IsNullOrEmpty(DueDate) == false) && SelectedJob.DueDate.AddDays(7).ToString() != DueDate
                        )
                    {
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                        {
                            _state.messageBoxVM.Symbol = 0;
                            _state.messageBoxVM.HeadMessage = "Discard changes?";
                            _state.messageBoxVM.Message = Names.DiscardEsc;
                            _state.messageBoxVM.ButtonStyle = Names.NoYes;
                            await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }

                        // if clicked yes
                        if (_state.messageBoxVM.Output == true)
                        {
                            await TryCloseAsync();
                        }
                    }
                    else
                    {
                        await TryCloseAsync();
                    }
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
                Logger.LogActivity(Logger.ERROR, $"CreateInvoiceViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task Exit()
        {
            try
            {
                if (
                    (string.IsNullOrEmpty(BillingName) == false) && SelectedJob.ClientName != BillingName ||
                    (string.IsNullOrEmpty(BillingAddress) == false) && SelectedJob.Address != BillingAddress ||
                    (string.IsNullOrEmpty(BillingCity) == false) && SelectedJob.City != BillingCity ||
                    (string.IsNullOrEmpty(BillingCountry) == false) && SelectedJob.Country != BillingCountry ||
                    (string.IsNullOrEmpty(BillingPostcode) == false) && SelectedJob.Postcode != BillingPostcode ||
                    (string.IsNullOrEmpty(DueDate) == false) && SelectedJob.DueDate.AddDays(7).ToString() != DueDate
                    )
                {
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                    {
                        _state.messageBoxVM.Symbol = 0;
                        _state.messageBoxVM.HeadMessage = "Discard changes?";
                        _state.messageBoxVM.Message = Names.DiscardEsc;
                        _state.messageBoxVM.ButtonStyle = Names.NoYes;
                        await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }

                    // if clicked yes
                    if (_state.messageBoxVM.Output == true)
                    {
                        await TryCloseAsync();
                    }
                }
                else
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
                Logger.LogActivity(Logger.ERROR, $"CreateInvoiceViewModel: Exit() FAIL\n\t{ex.Message}");
            }
        }

        public async Task CreateInvoice()
        {
            try
            {
                await Task.Run(async () =>
                {
                    await TryCloseAsync();
                    var dueDate = DateOnly.MinValue;
                    if (DueDate != String.Empty)
                    {
                        dueDate = DateOnly.ParseExact(DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }

                    int result = int.Parse(VatValue.TrimEnd('%'));

                    DateTime now = DateTime.Now;
                    DateTime dateTimeIssued = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                    await Database.CreateInvoice(SelectedJob.ClientId, SelectedJob.JobId, _subtotalAmountDb, result, _totalAmountDb, dueDate, BillingName.Trim(), BillingAddress.Trim(), BillingCity.Trim(), BillingPostcode.Trim(), BillingCountry.Trim(), dateTimeIssued);
                    await _dataService.RefreshDatabase();

                    var invoiceId = Convert.ToInt32(_dataService.Invoices.First(i => i.JobId == SelectedJob.JobId).InvoiceId);
                    var invoiceName = $"INV-{invoiceId}_{SelectedJob.ClientId}_{SelectedJob.JobId}_{dateTimeIssued.ToString("dd-MM-yyyy")}_{dateTimeIssued.ToString("HHmmss")}.pdf";
                    await Database.SetInvoiceName(invoiceId, invoiceName.Trim());

                    await GenerateInvoice(invoiceName);

                    await _events.PublishOnUIThreadAsync(new RefreshDatabase() { Command = "Invoice" });
                    await _events.PublishOnUIThreadAsync(new DashboardVMEvents { Command = Names.ShowInvoice });
                });
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Invoice Creation";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"CreateInvoiceViewModel: CreateInvoice() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Private Functions
        private async Task GenerateInvoice(string invoiceName)
        {
            try
            {
                await Task.Run(async () =>
                {
                    string filePath = await FileStore.SaveInvoiceFile(SelectedJob.ClientId, SelectedJob.ClientName, invoiceName);

                    Document.Create(container =>
                    {
                        // container allows multiple page
                        container.Page(page =>
                        {
                            page.Margin(40);

                            // header
                            // horizontal |---|---|
                            page.Header().PaddingBottom(60).Row(row =>
                            {
                                // cell 1
                                row.RelativeItem().Text(SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName).ExtraBold().FontSize(36).FontColor(Colors.Blue.Medium);

                                // cell 2
                                row.RelativeItem().AlignRight().Text("Invoice").SemiBold().FontSize(36).FontColor(Colors.Black);
                            });

                            // content
                            /*
                             * section 1:
                             * - client billing info, invoice information
                             * 
                             * section 2:
                             * - table with jobs
                             * 
                             * section 3:
                             * - subtotal, tax, total
                             * 
                             * section 4:
                             * - notes
                             * 
                             * section 5:
                             * - payment info (client's details and pay date), my company ifo
                             */
                            page.Content().Column(mainContainer =>
                            {
                                // billing info (client's) and invoice info
                                mainContainer.Item().PaddingBottom(60).Row(section1 =>
                                {
                                    // billing info
                                    section1.RelativeItem().AlignLeft().Column(billingInfo =>
                                    {
                                        billingInfo.Item().Text("BILLED TO:").FontSize(14).Bold();
                                        billingInfo.Item().Text(text =>
                                        {
                                            text.Span("Full Name: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(BillingName).FontSize(14);
                                        });
                                        billingInfo.Item().Text(text =>
                                        {
                                            text.Span("Phone Number: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(_dataService.Clients.FirstOrDefault(c => c.ClientId == SelectedJob.ClientId)?.PhoneNumber).FontSize(14);
                                        });
                                        billingInfo.Item().Text(text =>
                                        {
                                            text.Span("Email Address: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(_dataService.Clients.FirstOrDefault(c => c.ClientId == SelectedJob.ClientId)?.Email).FontSize(14);
                                        });
                                        billingInfo.Item().Text(text =>
                                        {
                                            text.Span("Home Address: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(_dataService.Clients.FirstOrDefault(c => c.ClientId == SelectedJob.ClientId)?.BillingAddress).FontSize(14);
                                        });
                                    });

                                    // invoice info
                                    section1.RelativeItem().AlignRight().Column(invoiceInfo =>
                                    {
                                        invoiceInfo.Item().Text(text =>
                                        {
                                            text.Span("Invoice No: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(_dataService.Invoices.Max(c => c.InvoiceNumber + 1).ToString()).FontSize(14);
                                        });
                                        invoiceInfo.Item().Text(text =>
                                        {
                                            text.Span("Created Date: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(DateOnly.FromDateTime(Convert.ToDateTime(DateTime.Now)).ToString()).FontSize(14);
                                        });
                                    });
                                });

                                // section 2 (table jobs)
                                mainContainer.Item().PaddingVertical(5).LineHorizontal(1);
                                mainContainer.Item().AlignCenter().Table(section2 =>
                                {
                                    section2.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); // item/description
                                        columns.RelativeColumn(1); // unit quantity
                                        columns.RelativeColumn(1); // unit price
                                        columns.RelativeColumn(1); // total
                                    });

                                    section2.Cell().AlignCenter().Text("Item").FontSize(14).Bold().FontColor(Colors.Grey.Darken2);
                                    section2.Cell().AlignCenter().Text("Quantity").FontSize(14).Bold().FontColor(Colors.Grey.Darken2);
                                    section2.Cell().AlignCenter().Text("Unit Price").FontSize(14).Bold().FontColor(Colors.Grey.Darken2);
                                    section2.Cell().AlignCenter().Text("Total").FontSize(14).Bold().FontColor(Colors.Grey.Darken2);
                                });
                                mainContainer.Item().PaddingVertical(5).LineHorizontal(1);
                                // add crap here
                                mainContainer.Item().AlignCenter().Table(section2 =>
                                {
                                    section2.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); // item/description
                                        columns.RelativeColumn(1); // unit quantity
                                        columns.RelativeColumn(1); // unit price
                                        columns.RelativeColumn(1); // total
                                    });

                                    section2.Cell().PaddingLeft(10).AlignLeft().Text(SelectedJob.JobTitle).FontSize(14);
                                    section2.Cell().AlignCenter().Text("1").FontSize(14);
                                    section2.Cell().AlignCenter().Text(SelectedJob.Price.ToString()).FontSize(14);
                                });
                                mainContainer.Item().PaddingVertical(5).LineHorizontal(1);


                                // section 3 (totals)
                                mainContainer.Item().PaddingBottom(60).AlignRight().Width(150).Column(section3 =>
                                {
                                    // subtotal
                                    section3.Item().PaddingBottom(10).Row(Subtotal =>
                                    {
                                        // cell 1
                                        Subtotal.RelativeItem().Text("subtotal").ExtraBold().FontSize(14);

                                        // cell 2
                                        Subtotal.RelativeItem().Text(SelectedJob.Price.ToString("C")).FontSize(14);
                                    });

                                    // tax
                                    section3.Item().PaddingBottom(10).Row(Subtotal =>
                                    {
                                        // cell 1
                                        Subtotal.RelativeItem().Text($"Tax (%)").ExtraBold().FontSize(14);

                                        // cell 2
                                        Subtotal.RelativeItem().Text(VatValue).FontSize(14);
                                    });

                                    // line break
                                    section3.Item().PaddingVertical(5).PaddingBottom(10).LineHorizontal(2);

                                    // tax
                                    section3.Item().PaddingBottom(10).Row(Subtotal =>
                                    {
                                        // cell 1
                                        Subtotal.RelativeItem().Text("Total").ExtraBold().FontSize(14);

                                        // cell 2
                                        Subtotal.RelativeItem().Text(_totalAmountDb.ToString("C")).FontSize(14);
                                    });
                                });

                                // section 4 (notes)
                                mainContainer.Item().AlignLeft().PaddingBottom(40).Column(section4 =>
                                {
                                    section4.Item().Text("NOTES").Bold();
                                    section4.Item().Text("I need curry!");
                                });

                                //mainContainer.Item().Height(60);

                                // section 5 (payment info, company info)
                                mainContainer.Item().PaddingBottom(0).Row(section5 =>
                                {
                                    // payment info
                                    section5.RelativeItem().AlignLeft().Column(paymentInfo =>
                                    {
                                        paymentInfo.Item().Text("PAYMENT INFORMATION").FontSize(14).Bold();
                                        paymentInfo.Item().Text(text =>
                                        {
                                            text.Span("Name: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(_dataService.Bank[0]?.BankName).FontSize(14);
                                        });
                                        paymentInfo.Item().Text(text =>
                                        {
                                            text.Span("Account Name: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(_dataService.Bank[0]?.AccountName).FontSize(14);
                                        });
                                        paymentInfo.Item().Text(text =>
                                        {
                                            text.Span("Account Number: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(_dataService.Bank[0]?.AccountNumber).FontSize(14);
                                        });
                                        paymentInfo.Item().Text(text =>
                                        {
                                            text.Span("Sort Code: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(_dataService.Bank[0]?.SortCode).FontSize(14);
                                        });
                                        if (string.IsNullOrEmpty(_dataService.Bank[0]?.IBAN) == false)
                                        {
                                            paymentInfo.Item().Text(text =>
                                            {
                                                text.Span("IBAN: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                                text.Span(_dataService.Bank[0]?.IBAN).FontSize(14);
                                            });
                                        }
                                        if (string.IsNullOrEmpty(_dataService.Bank[0]?.BIC) == false)
                                        {
                                            paymentInfo.Item().Text(text =>
                                            {
                                                text.Span("BIC: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                                text.Span(_dataService.Bank[0]?.BIC).FontSize(14);
                                            });
                                        }
                                        paymentInfo.Item().Text(text =>
                                        {
                                            text.Span("Due Date: ").Bold().FontColor(Colors.Grey.Darken2).FontSize(14);
                                            text.Span(DueDate).FontSize(14);
                                        });
                                    });

                                    section5.RelativeItem().AlignRight().Column(companyInfo =>
                                    {
                                        companyInfo.Item().Text(_dataService.Business[0]?.BusinessName).FontSize(14);
                                        companyInfo.Item().Text(_dataService.Business[0]?.Address).FontSize(14);
                                    });
                                });

                                // spacing
                                mainContainer.Item().PaddingVertical(10);
                            });

                            // footer
                            page.Footer().Text(text =>
                            {
                                //text.Span("Page ");
                                //text.CurrentPageNumber();
                                //text.Span(" of ");
                                //text.TotalPages();
                            });
                        });
                    }).GeneratePdf(filePath);
                });
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Generate Invoice";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"CreateInvoiceViewModel: GenerateInvoice() FAIL\n\t{ex.Message}");
            }
        }

        private bool ValidateDate()
        {
            return DateTime.TryParseExact(
                DueDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate)
                && parsedDate.Date >= DateTime.Today;
        }

        private Task CanSubmit()
        {
            try
            {
                if (string.IsNullOrEmpty(BillingName) == false && string.IsNullOrEmpty(BillingAddress) == false && string.IsNullOrEmpty(BillingCity) == false && string.IsNullOrEmpty(BillingCountry) == false && string.IsNullOrEmpty(BillingPostcode) == false && ValidateDate() == true)
                {

                    EnableSubmitBtn = true;
                    OpacitySubmitBtn = _fullOpacity;
                }
                else
                {
                    EnableSubmitBtn = false;
                    OpacitySubmitBtn = _halfOpacity;
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Validation";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"CreateInvoiceViewModel: CanSubmit() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Public View Variables
        public string BillingName
        {
            get { return _billingName; }
            set
            {
                _billingName = value;
                NotifyOfPropertyChange(() => BillingName);
                CanSubmit();
            }
        }

        public string BillingAddress
        {
            get { return _billingAddress; }
            set
            {
                _billingAddress = value;
                NotifyOfPropertyChange(() => BillingAddress);
                CanSubmit();
            }
        }

        public string BillingCity
        {
            get { return _billingCity; }
            set
            {
                _billingCity = value;
                NotifyOfPropertyChange(() => BillingCity);
                CanSubmit();
            }
        }

        public string BillingPostcode
        {
            get { return _billingPostcode; }
            set
            {
                _billingPostcode = value;
                NotifyOfPropertyChange(() => BillingPostcode);
                CanSubmit();
            }
        }

        public string BillingCountry
        {
            get { return _billingCountry; }
            set
            {
                _billingCountry = value;
                NotifyOfPropertyChange(() => BillingCountry);
                CanSubmit();
            }
        }

        public string DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                NotifyOfPropertyChange(() => DueDate);
                CanSubmit();
            }
        }

        public string Subtotal
        {
            get { return _subtotal; }
            set
            {
                _subtotal = value;
                _totalAmountDb = _subtotalAmountDb + (_subtotalAmountDb * (decimal.Parse(VatValue.TrimEnd('%')) / 100));
                TotalAmount = _totalAmountDb.ToString("C");
                NotifyOfPropertyChange(() => Subtotal);
            }
        }

        public string VatValue
        {
            get { return _vatValue; }
            set
            {
                _vatValue = value;
                _totalAmountDb = _subtotalAmountDb + (_subtotalAmountDb * (decimal.Parse(VatValue.TrimEnd('%')) / 100));
                TotalAmount = _totalAmountDb.ToString("C");
                NotifyOfPropertyChange(() => VatValue);
            }
        }

        public string TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                _totalAmount = value;
                NotifyOfPropertyChange(() => TotalAmount);
            }
        }

        public bool EnableSubmitBtn
        {
            get { return _enableSubmitBtn; }
            set
            {
                _enableSubmitBtn = value;
                NotifyOfPropertyChange(() => EnableSubmitBtn);
            }
        }

        public double OpacitySubmitBtn
        {
            get { return _opacitySubmitBtn; }
            set
            {
                _opacitySubmitBtn = value;
                NotifyOfPropertyChange(() => OpacitySubmitBtn);
            }
        }

        public List<string> InvoiceStatusEdit
        {
            get { return _invoiceStatusEdit; }
            set
            {
                _invoiceStatusEdit = value;
                NotifyOfPropertyChange(() => InvoiceStatusEdit);
            }
        }
        #endregion
    }
}
