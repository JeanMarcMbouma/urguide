using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UrGuide.Shared;
using UrGuide.Shared.Configuration;
using UrGuide.Shared.Contracts;

namespace UrGuide.WebApp.Services
{
    public class IPStackService : IIPStackService
    {
        public IPStackService(IOptions<IPStackConfiguration> options, IHttpClientFactory clientFactory)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public IOptions<IPStackConfiguration> Options { get; }
        public IHttpClientFactory ClientFactory { get; }

        public async Task<IPStackInfo> GetAsync(IPAddress ip)
        {
            var client = ClientFactory.CreateClient();
            client.BaseAddress = new Uri(Options.Value.Url);
            var response = await client.GetAsync($"{ip}?access_key={Options.Value.ApiKey}");
            if(response.IsSuccessStatusCode)
            {
                var dataString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse>(dataString);
            }
            return null;
        }

        class ApiResponse
        {
            public double? Longitude { get; set; }
            public double? Latitude { get; set; }
            [JsonProperty("country_name")]
            public string Country { get; set; }
            public string City { get; set; }
            public string ZipCode { get; set; }

            public static implicit operator IPStackInfo(ApiResponse response)
            { 
                return response.Longitude.HasValue ? new IPStackInfo
                {
                    City = response.City,
                    Country = response.Country,
                    Latitude = response.Latitude.Value,
                    Longitude = response.Longitude.Value,
                    ZipCode = response.ZipCode
                } : null;
            }
        }
    }
}
