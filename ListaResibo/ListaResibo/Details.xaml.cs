using Android.Content;
using Android.Graphics;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Xaml;

namespace ListaResibo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Details : ContentPage
    {
        private byte[] src;
        private string month;
        private string year;
        private Rec rec;
        public Details(byte[] img, string month, string year)
        {
            InitializeComponent();
            src = img;
            this.month = month;
            this.year = year;
        }

        private void getForm()
        {
            var newid = Guid.NewGuid().ToString();
            var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var fpath = System.IO.Path.Combine(dataPath, $"{newid}.jpg");
            rec = new Rec
            {
                StoreName = regname.Text.Trim(),
                filename = fpath,
                ID = newid,
                PersonName = suppliername.Text.Trim(),
                TaxableMonth = taxableMonth.Date,
                TIN = tin.Text.Trim(),
                ExemptPurchaseAmount = isdecimal(exemptamt.Text),
                GrossPurchaseAmount = isdecimal(grossamt.Text),
                TaxablePurchaseAmount = isdecimal(taxableamt.Text)
            };
        }

        private decimal isdecimal(string input)
        {
            decimal o;
            if (decimal.TryParse(input, out o))
                return o;
            return 0;
        }

        private async void save_Clicked(object sender, EventArgs e)
        {
            var dataPath =Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData);
            var filePath = System.IO.Path.Combine(dataPath, $"{year}-{month}.dat");
            getForm();
            await File.AppendAllTextAsync(filePath, rec.csvrow);
            await File.WriteAllBytesAsync(rec.filename, src);
            await DisplayAlert("Save", "Thank you come again", "Ok");
            await Navigation.PopToRootAsync();
            return;
        }
    }
}