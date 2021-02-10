using ClosedXML.Excel;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ListaResibo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            SelectCurrent();
        }

        private void processing(bool stat)
        {
            stack.IsVisible = !stat;
            lst.IsVisible = !stat;
            addbtn.IsVisible = !stat;
            prg.IsVisible = stat;
        }
        private void SelectCurrent()
        {
            pickMonth.SelectedItem = DateTime.Now.ToLocalTime().ToString("MMMM");
            pickYear.SelectedItem = DateTime.Now.ToLocalTime().ToString("yyyy");
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Piktyuran(pickMonth.SelectedItem as string, pickYear.SelectedItem as string));
        }

        private async void pickMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            await loaddata();
        }

        private async Task loaddata()
        {
            processing(true);
            var month = pickMonth.SelectedItem as string;
            var year = pickYear.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(month)
                || string.IsNullOrWhiteSpace(year))
            {
                lst.ItemsSource = new List<Rec>();
                processing(false);
                return;
            }

            var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filePath = System.IO.Path.Combine(dataPath, $"{year}-{month}.dat");

            if (!File.Exists(filePath))
            {
                lst.ItemsSource = new List<Rec>();
                processing(false);
                return;
            }

            var lines = await File.ReadAllLinesAsync(filePath);
            var list = new List<Rec>();
            foreach (var o in lines)
            {
                var temp = o.Split("■");
                list.Add(new Rec
                {
                    ID = temp[0],
                    TaxableMonth = DateTime.Parse(temp[1]),
                    TIN = temp[2],
                    StoreName = temp[3],
                    PersonName = temp[4],
                    GrossPurchaseAmount = decimal.Parse(temp[5]),
                    ExemptPurchaseAmount = decimal.Parse(temp[6]),
                    TaxablePurchaseAmount = decimal.Parse(temp[7]),
                    filename = temp[8]
                });
            }
            var dt = list.OrderBy(r => (r.StoreName + r.PersonName));
            lst.ItemsSource = dt;
            processing(false);
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            await loaddata();
        }

        private async void lst_Refreshing(object sender, EventArgs e)
        {
            await loaddata();
            lst.EndRefresh();
        }

        private async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            var sel = e.Parameter as Rec;
            if (sel != null)
            {
                var resp = await DisplayAlert("Delete", "Are you sure you want to delete this record?", "Yes, Delete", "No, I Change My Mind");
                if (!resp)
                    return;
                processing(true);
                var month = pickMonth.SelectedItem as string;
                var year = pickYear.SelectedItem as string;
                if (string.IsNullOrWhiteSpace(month)
                    || string.IsNullOrWhiteSpace(year))
                {
                    lst.ItemsSource = new List<Rec>();
                    processing(false);
                    return;
                }

                var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = System.IO.Path.Combine(dataPath, $"{year}-{month}.dat");

                if (!File.Exists(filePath))
                {
                    lst.ItemsSource = new List<Rec>();
                    processing(false);
                    return;
                }

                var lines = await File.ReadAllLinesAsync(filePath);
                var list = new List<Rec>();
                var towrite = new List<string>();
                foreach (var o in lines)
                {
                    var temp = o.Split("■");
                    if (sel.ID == temp[0])
                    {
                        if (File.Exists(temp[8]))
                            File.Delete(temp[8]);
                        continue;
                    }
                        
                    towrite.Add(o);
                    list.Add(new Rec
                    {
                        ID = temp[0],
                        TaxableMonth = DateTime.Parse(temp[1]),
                        TIN = temp[2],
                        StoreName = temp[3],
                        PersonName = temp[4],
                        GrossPurchaseAmount = decimal.Parse(temp[5]),
                        ExemptPurchaseAmount = decimal.Parse(temp[6]),
                        TaxablePurchaseAmount = decimal.Parse(temp[7]),
                        filename = temp[8]
                    });
                }
                await File.WriteAllLinesAsync(filePath, towrite);
                var dt = list.OrderBy(r => (r.StoreName + r.PersonName));
                lst.ItemsSource = dt;
                processing(false);
            }
        }

        private async void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            processing(true);
            var month = pickMonth.SelectedItem as string;
            var year = pickYear.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(month)
                || string.IsNullOrWhiteSpace(year))
            {
                lst.ItemsSource = new List<Rec>();
                processing(false);
                return;
            }

            var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filePath = System.IO.Path.Combine(dataPath, $"{year}-{month}.dat");
            var xlPath = System.IO.Path.Combine(dataPath, $"{year}-{month}.xlsx");

            if (!File.Exists(filePath))
            {
                processing(false);
                return;
            }

            var lines = await File.ReadAllLinesAsync(filePath);
            if (lines.Length == 0)
            {
                processing(false);
                return;
            }

            var list = new List<Rec>();
            foreach (var o in lines)
            {
                var temp = o.Split("■");
                list.Add(new Rec
                {
                    ID = temp[0],
                    TaxableMonth = DateTime.Parse(temp[1]),
                    TIN = temp[2],
                    StoreName = temp[3],
                    PersonName = temp[4],
                    GrossPurchaseAmount = decimal.Parse(temp[5]),
                    ExemptPurchaseAmount = decimal.Parse(temp[6]),
                    TaxablePurchaseAmount = decimal.Parse(temp[7]),
                    filename = temp[8]
                });
            }
            var dt = list.OrderBy(r => (r.StoreName + r.PersonName));

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet($"{month}-{year}");

            ws.Cell(1, 1).Value = $"{month}-{year} Receipts";
            ws.Cell(2, 1).Value = "TAXABLE MONTH";
            ws.Cell(2, 2).Value = "TIN";
            ws.Cell(2, 3).Value = "REGISTERED NAME";
            ws.Cell(2, 4).Value = "SUPPLIER NAME";
            ws.Cell(2, 5).Value = "GROSS AMOUNT";
            ws.Cell(2, 6).Value = "EXEMPT AMOUNT";
            ws.Cell(2, 7).Value = "TAXABLE AMOUNT";
            ws.Cell(2, 8).Value = "RECEIPT";

            var message = new EmailMessage
            {
                Subject = $"Receipts for Taxable Month {month}-{year}",
                Body = $"Dear Mark, Attached is the list of receipts for the month of {month} {year}. KTnxBye."
            };
            int r = 3;
            foreach(var o in dt)
            {
                ws.Cell(r, 1).Value = o.dtstring;
                ws.Cell(r, 2).Value = o.TIN;
                ws.Cell(r, 3).Value = o.StoreName;
                ws.Cell(r, 4).Value = o.PersonName;
                ws.Cell(r, 5).Value = o.GrossPurchaseAmount.ToString("###,###,###,##0.00");
                ws.Cell(r, 6).Value = o.ExemptPurchaseAmount.ToString("###,###,###,##0.00");
                ws.Cell(r, 7).Value = o.TaxablePurchaseAmount.ToString("###,###,###,##0.00");
                ws.Cell(r, 8).Value = $"{o.filename.Split(System.IO.Path.DirectorySeparatorChar).LastOrDefault()}";
                message.Attachments.Add(new EmailAttachment(o.filename));
                r++;
            }
            if (File.Exists(xlPath))
                File.Delete(xlPath);
            wb.SaveAs(xlPath);
            processing(false);

            var fn = $"{month}-{year}.xlsx";
            message.Attachments.Add(new EmailAttachment(xlPath));
            await Email.ComposeAsync(message);
        }
    }
}
