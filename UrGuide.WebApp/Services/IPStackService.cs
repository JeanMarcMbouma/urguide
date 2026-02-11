using Microsoft.Extensions.Logging;
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
        public IPStackService(IOptions<IPStackConfiguration> options, 
            IHttpClientFactory clientFactory,
            ILogger<IPStackService> logger)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOptions<IPStackConfiguration> Options { get; }
        public IHttpClientFactory ClientFactory { get; }
        public ILogger<IPStackService> Logger { get; }

        public async Task<IPStackInfo?> GetAsync(IPAddress ip)
        {
            var client = ClientFactory.CreateClient();
            client.BaseAddress = new Uri(Options.Value.Url);
            try
            {
                var response = await client.GetAsync($"{ip}?access_key={Options.Value.ApiKey}");
                if (response.IsSuccessStatusCode)
                {
                    var dataString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse>(dataString);
                    if (result == null)
                    {
                        return null;
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "failed to retrieve ip location");
            }
            return null;
        }

        class ApiResponse
        {
            public double? Longitude { get; set; }
            public double? Latitude { get; set; }
            [JsonProperty("country_name")]
            public string Country { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string ZipCode { get; set; } = string.Empty;

            public static implicit operator IPStackInfo?(ApiResponse response)
            { 
                if (response == null || !response.Longitude.HasValue || !response.Latitude.HasValue)
                {
                    return null;
                }

                return new IPStackInfo
                {
                    City = response.City,
                    Country = response.Country,
                    Latitude = response.Latitude.Value,
                    Longitude = response.Longitude.Value,
                    ZipCode = response.ZipCode
                };
            }
        }
    }
}
