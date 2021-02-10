using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ListaResibo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Piktyuran : ContentPage
    {
        private string month;
        private string year;
        MediaFile file;
        byte[] filebytes;
        public Piktyuran(string month, string year)
        {
            InitializeComponent();
            this.month = month;
            this.year = year;
            LoadCamera().ConfigureAwait(false);
        }

        private async Task LoadCamera()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No Camera Available", "OK");
                return;
            }

            this.file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                Directory = "Receipts"                
            });

            if (this.file == null)
                return;
            
            piktyur.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                filebytes = MediaFileToArray(stream);
                file.Dispose();
                return stream;
            });

            
        }

        private byte[] MediaFileToArray(Stream s)
        {
            using var ms = new MemoryStream();
            s.CopyTo(ms);
            s.Position = 0;
            return ms.ToArray();
        }

        private async void snap_Clicked(object sender, EventArgs e)
        {
            await LoadCamera();
        }

        private async void dets_Clicked(object sender, EventArgs e)
        {
            if (piktyur.Source == null)
            {
                await DisplayAlert("No", "Please Don't Me", "Ok");
                return;
            }
            await Navigation.PushAsync(new Details(filebytes, month, year));
        }
    }
}