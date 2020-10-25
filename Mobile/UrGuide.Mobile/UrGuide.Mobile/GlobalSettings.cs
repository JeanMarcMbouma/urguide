namespace UrGuide.Mobile
{
    public class GlobalSetting
    {
        public const string DefaultEndpoint =
#if !DEBUG
            "https://urguide.azurewebsites.net";
#else
            "http://192.168.0.110:5000";
#endif

        public const string AuthScheme = "urguide";
        public const int Like = 2;
        public const int DisLike = 4;
        public const int Unknown = 0;

        public GlobalSetting()
        {
            BaseIdentityEndpoint = DefaultEndpoint;
        }

        public static GlobalSetting Instance { get; } = new GlobalSetting();

        public string BaseIdentityEndpoint { get; set; }


        public string ClientId { get { return "xamarin"; } }

        public string ClientSecret { get { return "secret"; } }

        public string Callback { get; } = "urguide://callback";
        public string City { get; internal set; }
    }
}
