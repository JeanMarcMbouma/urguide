using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePost : ContentPage
    {
        public CreatePost()
        {
            InitializeComponent();
            BindingContext = new VM();
        }

        class VM
        {

            public string Description { get; set; }
            public string Title { get; set; }
            public List<API.ItineraryModel> Itineraries { get; set; } = new List<API.ItineraryModel> { 
                new API.ItineraryModel
                {
                    Description = "Yaounde",
                    Title = "The city of your heart"
                },
                new API.ItineraryModel
                {
                    Description = "Douala",
                    Title = "The city of your heart"
                },
                new API.ItineraryModel
                {
                    Description = "Edea",
                    Title = "The city of your heart"
                }
            };
            public List<API.ImageFileModel> Images { get; set; } = new List<API.ImageFileModel> { 
                new API.ImageFileModel
                {
                    Name = "image",
                    ImageBase64 = $"{GlobalSetting.DefaultEndpoint}/images/B4DE7FA6-96A8-4F6F-A0A9-97BAB64AD24E.png"
                },
                new API.ImageFileModel
                {
                    Name = "image",
                    ImageBase64 = $"{GlobalSetting.DefaultEndpoint}/images/amusement.png"
                },
                new API.ImageFileModel
                {
                    Name = "image",
                    ImageBase64 = $"{GlobalSetting.DefaultEndpoint}/images/amusement.png"
                }
            };
            public List<string> Cities { get; set; } = Constants.Countries.ToList();
            public string City { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }

            public int ItineraryCount => Itineraries.Count();
        }
    }
}