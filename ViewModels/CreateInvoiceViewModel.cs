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
    using Traker.Events;
    using Traker.Services;

    public class CreateInvoiceViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly DataService _dataService;
        #endregion

        #region Private View Variables
        private string _billingName;
        private string _billingAddress;
        private string _billingCity;
        private string _billingPostcode;
        private string _billingCountry;
        private string _dueDate;
        private decimal _subtotal;
        private string _vatValue;
        private decimal _totalAmount;
        #endregion

        #region Public Variables
        public List<string> InvoiceStatusEdit { get; set; } = new List<string> { "0%", "5%", "20%" };

        /*
         * create a new model containing all the important information to pre-fill
         * the invoice form could be using DashboardModel since it's under row selection
         */
        public DashboardModel SelectedJob; // data passed by DashboardVM
        #endregion

        public CreateInvoiceViewModel(IEventAggregator events, DataService dataService)
        {
            _events = events;
            _dataService = dataService;

            SelectedJob = new DashboardModel(); // to be improved to add useful properties

            _billingName = string.Empty;
            _billingAddress = string.Empty;
            _billingCity = string.Empty;
            _billingPostcode = string.Empty;
            _billingCountry = string.Empty;
            _dueDate = string.Empty;
            _subtotal = 0.0m;
            _vatValue = "0%";
            _totalAmount = 0.0m;

        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            BillingName = SelectedJob.ClientName;
            BillingAddress = SelectedJob.BillingAddress;
            BillingCity = SelectedJob.City;
            BillingPostcode = SelectedJob.Postcode;
            BillingCountry = SelectedJob.Country;
            DueDate = DateOnly.FromDateTime(DateTime.Now.Date.Add(TimeSpan.FromDays(7))).ToString();
            Subtotal = decimal.Parse(SelectedJob.Price, NumberStyles.Currency, CultureInfo.CurrentCulture);

            return base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public async Task Exit()
        {
            await TryCloseAsync();
        }

        public Task CreateInvoice()
        {
            //BillingName = "Obama";
            //BillingAddress = "32 Downing Street";
            //BillingCity = "London";
            //BillingPostcode = "SW1A 2AA";
            //BillingCountry = "United Kingdom";
            //DueDate = "23/10/2026";
            //Subtotal = 1000.00m;
            //VatValue = "20%";
            //TotalAmount = 1200.00m;


            /*
             * InvoiceNumber
             * IssueDate
             * Status
             */

            //public static Task CreateInvoice(int jobId, decimal subtotal, decimal taxAmount, decimal totalAmount, DateTime dueDate, string billingName, string billingAddress, string billingCity, string billingPostcode, string billingCountry)


            var dueDate = DateOnly.MinValue;
            //decimal amount = 0;

            //if (DueDate != String.Empty)
            //{
            //    dueDate = DateOnly.ParseExact(DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //}

            if (DueDate != String.Empty)
            {
                dueDate = DateOnly.ParseExact(DueDate,"dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            //if (VatValue != String.Empty)
            //{
            //    amount = decimal.Parse(VatValue, CultureInfo.InvariantCulture);
            //}


            int result = int.Parse(VatValue.TrimEnd('%'));

            Database.CreateInvoice(SelectedJob.ClientId, SelectedJob.JobId, Subtotal, result, TotalAmount, dueDate, BillingName, BillingAddress, BillingCity, BillingPostcode, BillingCountry);



            GenerateInvoice();

            _events.PublishOnUIThreadAsync(new RefreshDatabase());


            return Task.CompletedTask;
        }

        private void GenerateInvoice()
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "invoice_test.pdf");

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
                        row.RelativeItem().Text(SelectedJob.CompanyName).ExtraBold().FontSize(36).FontColor(Colors.Blue.Medium);

                        // cell 2
                        row.RelativeItem().AlignRight().Text("Invoice").SemiBold().FontSize(36).FontColor(Colors.Black);
                        //row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("d")).FontSize(12).FontColor(Colors.Grey.Medium);
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
                        //// vertical
                        //col.Item().Text("Line 1");
                        //col.Item().Text("Line 2");


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
                            //section2.Cell().Text("£100");

                            //section2.Cell().Text("VAT");
                            //section2.Cell().Text("£20");
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
                            section2.Cell().AlignCenter().Text(SelectedJob.Price).FontSize(14);
                            section2.Cell().AlignCenter().Text(SelectedJob.Price).FontSize(14);
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
                                Subtotal.RelativeItem().Text(SelectedJob.Price).FontSize(14);
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
                                Subtotal.RelativeItem().Text(TotalAmount.ToString("C")).FontSize(14);
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

            // Open it
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }

        #region Public View Variables
        public string BillingName
        {
            get { return _billingName; }
            set
            {
                _billingName = value;
                NotifyOfPropertyChange(() => BillingName);
            }
        }

        public string BillingAddress
        {
            get { return _billingAddress; }
            set
            {
                _billingAddress = value;
                NotifyOfPropertyChange(() => BillingAddress);
            }
        }

        public string BillingCity
        {
            get { return _billingCity; }
            set
            {
                _billingCity = value;
                NotifyOfPropertyChange(() => BillingCity);
            }
        }

        public string BillingPostcode
        {
            get { return _billingPostcode; }
            set
            {
                _billingPostcode = value;
                NotifyOfPropertyChange(() => BillingPostcode);
            }
        }

        public string BillingCountry
        {
            get { return _billingCountry; }
            set
            {
                _billingCountry = value;
                NotifyOfPropertyChange(() => BillingCountry);
            }
        }

        public string DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                NotifyOfPropertyChange(() => DueDate);
            }
        }

        public decimal Subtotal
        {
            get { return _subtotal; }
            set
            {
                _subtotal = value;
                TotalAmount = _subtotal + (_subtotal * (decimal.Parse(VatValue.TrimEnd('%')) / 100));
                NotifyOfPropertyChange(() => Subtotal);
            }
        }

        public string VatValue
        {
            get { return _vatValue; }
            set
            {
                _vatValue = value;
                TotalAmount = _subtotal + (_subtotal * (decimal.Parse(VatValue.TrimEnd('%')) / 100));
                NotifyOfPropertyChange(() => VatValue);

                // update VAT calculation
            }
        }

        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                _totalAmount = value;
                NotifyOfPropertyChange(() => TotalAmount);
            }
        }
        #endregion
    }
}
