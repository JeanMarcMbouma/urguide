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

        // i.e.: "http://YOUR_IP" or "http://YOUR_DNS_NAME"
        public const string AuthScheme = "urguide";
        private string _baseIdentityEndpoint;
        public const int Like = 2;
        public const int DisLike = 4;
        public const int Unknown = 0;

        public GlobalSetting()
        {
            AuthToken = "INSERT AUTHENTICATION TOKEN";

            BaseIdentityEndpoint = DefaultEndpoint;
        }

        public static GlobalSetting Instance { get; } = new GlobalSetting();

        public string BaseIdentityEndpoint
        {
            get { return _baseIdentityEndpoint; }
            set
            {
                _baseIdentityEndpoint = value;
                UpdateEndpoint(_baseIdentityEndpoint);
            }
        }


        public string ClientId { get { return "xamarin"; } }

        public string ClientSecret { get { return "secret"; } }

        public string AuthToken { get; set; }

        public string AuthorizeEndpoint { get; set; }

        public string Callback { get; set; }

        private void UpdateEndpoint(string endpoint)
        {
            var connectBaseEndpoint = $"{endpoint}/connect";
            AuthorizeEndpoint = $"{connectBaseEndpoint}/authorize";

            Callback = "urguide://callback";
        }
    }
}
