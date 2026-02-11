using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Regions;
using UrGuide.Services.Contracts;

namespace UrGuide.Services.Seeding
{
    public class DataSeedingService : IDataSeedingService
    {
        private readonly UrGuideContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataSeedingService> _logger;

        public DataSeedingService(
            UrGuideContext context,
            IConfiguration configuration,
            ILogger<DataSeedingService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedDataAsync()
        {
            try
            {
                await SeedCountriesAsync();
                await SeedCurrenciesAsync();
                await SeedRegionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding data");
                throw;
            }
        }

        private async Task SeedCountriesAsync()
        {
            if (await _context.Countries.AnyAsync())
            {
                return; // Countries already seeded
            }

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "countries.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("countries.json file not found");
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var countryData = JsonConvert.DeserializeObject<List<dynamic>>(jsonContent) ?? [];

            foreach (var item in countryData)
            {
                var country = new Country
                {
                    Name = item.name.ToString(),
                    Code = item.code.ToString(),
                    DialCode = item.dial_code.ToString()
                };

                _context.Countries.Add(country);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} countries", countryData.Count);
        }

        private async Task SeedCurrenciesAsync()
        {
            if (await _context.Currencies.AnyAsync())
            {
                return; // Currencies already seeded
            }

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "currencies.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("currencies.json file not found");
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var currencyData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent) ?? [];

            foreach (var item in currencyData)
            {
                var currency = new Currency
                {
                    Name = item.Value.name.ToString(),
                    Code = item.Key,
                    Symbol = item.Value.symbol?.ToString(),
                    SymbolNative = item.Value.symbol_native?.ToString(),
                    Rounding = (int)item.Value.rounding,
                    DecimalDigits = (int)item.Value.decimal_digits,
                    NamePlural = item.Value.name_plural?.ToString()
                };

                _context.Currencies.Add(currency);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} currencies", currencyData.Count);
        }

        private async Task SeedRegionsAsync()
        {
            if (await _context.Regions.AnyAsync())
            {
                return; // Regions already seeded
            }

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "regions.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("regions.json file not found");
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var regionData = JsonConvert.DeserializeObject<List<RegionSeedData>>(jsonContent) ?? [];

            foreach (var item in regionData)
            {
                // Find the country by code
                var country = await _context.Countries.FirstOrDefaultAsync(c => c.Code == item.CountryCode);
                if (country == null)
                {
                    _logger.LogWarning("Country with code {CountryCode} not found for region {RegionName}", item.CountryCode, item.Name);
                    continue;
                }

                // Find the currency by code
                var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.Code == item.CurrencyCode);
                if (currency == null)
                {
                    _logger.LogWarning("Currency with code {CurrencyCode} not found for region {RegionName}", item.CurrencyCode, item.Name);
                    continue;
                }

                var region = new Region
                {
                    Name = item.Name,
                    CountryId = country.Name,
                    CurrencyId = currency.Name,
                    Flags = new RegionFlags
                    {
                        Active = true,
                        CanRaiseTourRequests = true,
                        CanMakePayments = true,
                        CanMakeReservations = true,
                        CanRegisterUsers = true
                    },
                    Stats = new RegionStats
                    {
                        RegisteredUsers = 0,
                        RegisteredGuides = 0,
                        ToursOverallCount = 0
                    }
                };

                _context.Regions.Add(region);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} regions", regionData.Count);
        }
    }
}