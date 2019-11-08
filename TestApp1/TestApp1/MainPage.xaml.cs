using Android.Content.Res;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TestApp1.Class;
using Xam.Plugins.OnDeviceCustomVision;
using Xamarin.Forms;


namespace TestApp1
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            List<Part> parts = new List<Part>();
            parts.Add(new Part("2780", 0, 95, 10));
            parts.Add(new Part("6536", 71, 8, 0));
            parts.Add(new Part("6558", 1, 38, 10));
            parts.Add(new Part("32034", 4, 6, 10));
            parts.Add(new Part("32054", 4, 10, 20));
            parts.Add(new Part("32523", 0, 12, 17));
            parts.Add(new Part("42003", 4, 14, 10));
            parts.Add(new Part("43093", 1, 28, 19));
            parts.Add(new Part("54821", 4, 3, 10));
            parts.Add(new Part("57585", 71, 1, 9));
            

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
                    var partId = tags.OrderByDescending(t => t.Probability).First().Tag;
                    Part p = parts.Find(t => t.partId == partId);
                    getPartInfo(p);
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
                    var partId = tags.OrderByDescending(t => t.Probability).First().Tag;
                    Part p = parts.Find(t => t.partId == partId);
                    getPartInfo(p);
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

        private void getPartInfo(Part part)
        {
            
            resultInfo.Text = "Part ID: " + part.partId + "\nColour: " + part.colour + "\nQuantity: " + part.quantity + "\nNumber of Photos: " + part.numPhotos;
        }
    }
}

