using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CurrencyExchangeService.Models;

namespace CurrencyExchangeService
{
    public class NbpApiClient
    {
        private static readonly HttpClient client = new HttpClient();
        private const string BaseUrl = "http://api.nbp.pl/api/";

        public async Task<List<NbpRate>> GetCurrentRatesAsync()
        {
            string url = $"{BaseUrl}exchangerates/tables/A/?format=json";
            var response = await client.GetStringAsync(url);
            var tables = JsonConvert.DeserializeObject<List<NbpTableResponse>>(response);
            return tables[0].rates;
        }

        public async Task<List<NbpHistoricalValue>> GetHistoricalRatesAsync(string currencyCode, string startDate, string endDate)
        {
            string url = $"{BaseUrl}exchangerates/rates/A/{currencyCode}/{startDate}/{endDate}/?format=json";
            var response = await client.GetStringAsync(url);
            var historicalData = JsonConvert.DeserializeObject<NbpHistoricalRate>(response);
            return historicalData?.rates ?? new List<NbpHistoricalValue>();
        }
    }
}