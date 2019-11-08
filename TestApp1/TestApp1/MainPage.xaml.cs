using Android.Content.Res;
using Newtonsoft.Json;
using Plugin.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Xam.Plugins.OnDeviceCustomVision;
using Xamarin.Forms;


namespace TestApp1
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            takePhoto.Clicked += async (sender, args) =>
            {

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                    return;
                }
                try
                {
                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        SaveToAlbum = saveToGallery.IsToggled
                    });

                    if (file == null)
                        return;

                    var tags = await CrossImageClassifier.Current.ClassifyImage(file.GetStream());
                    //getPartInfo(tags.OrderByDescending(t => t.Probability).First().Tag);
                    //Loop through first three guesses
                    var n = 0;
                    info.Text = "";
                    while (n < 3)
                    {
                        //Get probability and round to 2 decimals/convert to string
                        var probability = tags.OrderByDescending(t => t.Probability).ElementAt(n).Probability * 100;
                        var probStr = probability.ToString("#.##");
                        //Set Label on MainPage
                        info.Text += n + 1 + ". " + tags.OrderByDescending(t => t.Probability).ElementAt(n).Tag + " with " + probStr + "% confidence\n";
                        ++n;
                    }

                    image.Source = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        file.Dispose();
                        return stream;
                    });
                }
                catch //(Exception ex)
                {
                    // Xamarin.Insights.Report(ex);
                    // await DisplayAlert("Uh oh", "Something went wrong, but don't worry we captured it in Xamarin Insights! Thanks.", "OK");
                }
            };

            pickPhoto.Clicked += async (sender, args) =>
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    return;
                }
                try
                {
                    Stream stream = null;
                    var file = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);


                    if (file == null)
                        return;

                    var tags = await CrossImageClassifier.Current.ClassifyImage(file.GetStream());
                    //getPartInfo(tags.OrderByDescending(t => t.Probability).First().Tag);
                    //Loop through first three guesses
                    var n = 0;
                    info.Text = "\n";
                    while (n < 3)
                    {
                        //Get probability and round to 2 decimals/convert to string
                        var probability = tags.OrderByDescending(t => t.Probability).ElementAt(n).Probability * 100;
                        var probStr = probability.ToString("#.##");
                        //Set Label on MainPage
                        info.Text += n + 1 + ". " + tags.OrderByDescending(t => t.Probability).ElementAt(n).Tag + " with " + probStr + "% confidence\n";
                        ++n;
                    }

                    stream = file.GetStream();
                    file.Dispose();

                    image.Source = ImageSource.FromStream(() => stream);

                }
                catch //(Exception ex)
                {
                    // Xamarin.Insights.Report(ex);
                    // await DisplayAlert("Uh oh", "Something went wrong, but don't worry we captured it in Xamarin Insights! Thanks.", "OK");
                }
            };
        }

        private void getPartInfo(string partId)
        {
            Part part;
            info.Text = "before/before";
            
            using (StreamReader r = new StreamReader("parts.json"))
            {
                info.Text = "before";
                string json = r.ReadToEnd();
                info.Text = "After";
                List<Part> parts = JsonConvert.DeserializeObject<List<Part>>(json);
                part = (Part)parts.Where(p => p.partId == partId);
            }
            resultInfo.Text = "Part ID: " + part.partId + "\nColour: " + part.colour + "\nQuantity: " + part.quantity + "\nNumber of Photos: " + part.numPhotos;
        }

        private class Part
        {
            public string partId;
            public int colour;
            public int quantity;
            public int numPhotos = 0;
        }
    }
}

