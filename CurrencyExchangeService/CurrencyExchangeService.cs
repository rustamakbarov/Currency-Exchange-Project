// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net.Http;
// using System.Threading.Tasks;
// using Newtonsoft.Json;

// namespace CurrencyExchangeService
// {
//     public class CurrencyExchangeServiceImpl : ICurrencyExchange
//     {
//         private static readonly HttpClient _httpClient = new HttpClient();

//         public List<ExchangeRate> GetCurrentExchangeRates()
//         {
//             try
//             {
//                 Console.WriteLine("NBP API'den kurlar çekiliyor...");
//                 var task = Task.Run(() => GetRatesFromNbp());
//                 var rates = task.Result;
                
//                 Console.WriteLine($"{rates.Count} kur bulundu.");
                
//                 return rates.Select(r => new ExchangeRate
//                 {
//                     CurrencyCode = r.code ?? "Unknown",
//                     CurrencyName = r.currency ?? "Unknown",
//                     Rate = r.mid,
//                     Date = DateTime.Now.Date
//                 }).ToList();
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Hata: {ex.Message}");
//                 return new List<ExchangeRate>();
//             }
//         }

//         private async Task<List<NbpRate>> GetRatesFromNbp()
//         {
//             string url = "http://api.nbp.pl/api/exchangerates/tables/A/?format=json";
//             var response = await _httpClient.GetStringAsync(url);
//             var tables = JsonConvert.DeserializeObject<List<NbpTableResponse>>(response);
            
//             if (tables != null && tables.Count > 0 && tables[0].rates != null)
//             {
//                 return tables[0].rates;
//             }
//             return new List<NbpRate>();
//         }

//         public List<ExchangeRate> GetHistoricalExchangeRates(string currencyCode, string startDate, string endDate)
//         {
//             try
//             {
//                 Console.WriteLine($"Tarihsel kur çekiliyor: {currencyCode} - {startDate} / {endDate}");
//                 var task = Task.Run(() => GetHistoricalFromNbp(currencyCode, startDate, endDate));
//                 var historicalRates = task.Result;

//                 return historicalRates.Select(r => new ExchangeRate
//                 {
//                     CurrencyCode = currencyCode.ToUpper(),
//                     CurrencyName = "",
//                     Rate = r.mid,
//                     Date = r.effectiveDate
//                 }).ToList();
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Hata: {ex.Message}");
//                 return new List<ExchangeRate>();
//             }
//         }

//         private async Task<List<NbpHistoricalValue>> GetHistoricalFromNbp(string currencyCode, string startDate, string endDate)
//         {
//             string url = $"http://api.nbp.pl/api/exchangerates/rates/A/{currencyCode}/{startDate}/{endDate}/?format=json";
//             var response = await _httpClient.GetStringAsync(url);
//             var historicalData = JsonConvert.DeserializeObject<NbpHistoricalRate>(response);
//             return historicalData?.rates ?? new List<NbpHistoricalValue>();
//         }

//         public CurrencyConversion ConvertCurrency(string fromCurrency, string toCurrency, decimal amount)
//         {
//             try
//             {
//                 Console.WriteLine($"Çeviri: {amount} {fromCurrency} -> {toCurrency}");
                
//                 if (amount <= 0)
//                     throw new Exception("Miktar sıfırdan büyük olmalıdır");

//                 if (fromCurrency == toCurrency)
//                 {
//                     return new CurrencyConversion
//                     {
//                         FromCurrency = fromCurrency,
//                         ToCurrency = toCurrency,
//                         Amount = amount,
//                         ConvertedAmount = amount,
//                         ExchangeRateUsed = 1,
//                         ConversionDate = DateTime.Now
//                     };
//                 }

//                 var rates = GetCurrentExchangeRates();
                
//                 decimal fromRate = 1;
//                 decimal toRate = 1;
                
//                 // PLN özel durumu
//                 if (fromCurrency != "PLN")
//                 {
//                     var fromRateObj = rates.FirstOrDefault(r => r.CurrencyCode == fromCurrency);
//                     if (fromRateObj == null)
//                         throw new Exception($"Geçersiz döviz kodu: {fromCurrency}");
//                     fromRate = fromRateObj.Rate;
//                 }
                
//                 if (toCurrency != "PLN")
//                 {
//                     var toRateObj = rates.FirstOrDefault(r => r.CurrencyCode == toCurrency);
//                     if (toRateObj == null)
//                         throw new Exception($"Geçersiz döviz kodu: {toCurrency}");
//                     toRate = toRateObj.Rate;
//                 }
                
//                 decimal result;
//                 decimal usedRate;
                
//                 if (fromCurrency == "PLN")
//                 {
//                     // PLN -> diğer
//                     result = amount / toRate;
//                     usedRate = toRate;
//                 }
//                 else if (toCurrency == "PLN")
//                 {
//                     // diğer -> PLN
//                     result = amount * fromRate;
//                     usedRate = fromRate;
//                 }
//                 else
//                 {
//                     // diğer -> diğer (önce PLN'e çevir)
//                     decimal inPln = amount * fromRate;
//                     result = inPln / toRate;
//                     usedRate = fromRate / toRate;
//                 }
                
//                 return new CurrencyConversion
//                 {
//                     FromCurrency = fromCurrency,
//                     ToCurrency = toCurrency,
//                     Amount = amount,
//                     ConvertedAmount = Math.Round(result, 2),
//                     ExchangeRateUsed = Math.Round(usedRate, 4),
//                     ConversionDate = DateTime.Now
//                 };
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Çeviri hatası: {ex.Message}");
//                 throw;
//             }
//         }
//     }

//     // Models
//     public class NbpRate
//     {
//         public string? currency { get; set; }
//         public string? code { get; set; }
//         public decimal mid { get; set; }
//     }

//     public class NbpTableResponse
//     {
//         public string? table { get; set; }
//         public string? no { get; set; }
//         public DateTime effectiveDate { get; set; }
//         public List<NbpRate>? rates { get; set; }
//     }

//     public class NbpHistoricalRate
//     {
//         public string? code { get; set; }
//         public List<NbpHistoricalValue>? rates { get; set; }
//     }

//     public class NbpHistoricalValue
//     {
//         public DateTime effectiveDate { get; set; }
//         public decimal mid { get; set; }
//     }
// }

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace CurrencyExchangeService
{
    public class CurrencyExchangeServiceImpl : ICurrencyExchange
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public List<ExchangeRate> GetCurrentExchangeRates()
        {
            try
            {
                Console.WriteLine("NBP API'den kurlar çekiliyor...");
                var task = Task.Run(() => GetRatesFromNbp());
                var rates = task.Result;
                
                Console.WriteLine($"{rates.Count} kur bulundu.");
                
                return rates.Select(r => new ExchangeRate
                {
                    CurrencyCode = r.code ?? "Unknown",
                    CurrencyName = r.currency ?? "Unknown",
                    Rate = r.mid,
                    Date = DateTime.Now.Date
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return new List<ExchangeRate>();
            }
        }

        private async Task<List<NbpRate>> GetRatesFromNbp()
        {
            string url = "http://api.nbp.pl/api/exchangerates/tables/A/?format=json";
            var response = await _httpClient.GetStringAsync(url);
            var tables = JsonConvert.DeserializeObject<List<NbpTableResponse>>(response);
            
            if (tables != null && tables.Count > 0 && tables[0].rates != null)
            {
                return tables[0].rates;
            }
            return new List<NbpRate>();
        }

        public List<ExchangeRate> GetHistoricalExchangeRates(string currencyCode, string startDate, string endDate)
        {
            try
            {
                Console.WriteLine($"Tarihsel kur çekiliyor: {currencyCode}");
                var task = Task.Run(() => GetHistoricalFromNbp(currencyCode, startDate, endDate));
                var historicalRates = task.Result;

                return historicalRates.Select(r => new ExchangeRate
                {
                    CurrencyCode = currencyCode.ToUpper(),
                    CurrencyName = "",
                    Rate = r.mid,
                    Date = r.effectiveDate
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return new List<ExchangeRate>();
            }
        }

        private async Task<List<NbpHistoricalValue>> GetHistoricalFromNbp(string currencyCode, string startDate, string endDate)
        {
            string url = $"http://api.nbp.pl/api/exchangerates/rates/A/{currencyCode}/{startDate}/{endDate}/?format=json";
            var response = await _httpClient.GetStringAsync(url);
            var historicalData = JsonConvert.DeserializeObject<NbpHistoricalRate>(response);
            return historicalData?.rates ?? new List<NbpHistoricalValue>();
        }

        public CurrencyConversion ConvertCurrency(string fromCurrency, string toCurrency, decimal amount)
        {
            try
            {
                Console.WriteLine($"Çeviri: {amount} {fromCurrency} -> {toCurrency}");
                
                if (amount <= 0)
                    throw new FaultException("Miktar sıfırdan büyük olmalıdır");

                if (fromCurrency == toCurrency)
                {
                    return new CurrencyConversion
                    {
                        FromCurrency = fromCurrency,
                        ToCurrency = toCurrency,
                        Amount = amount,
                        ConvertedAmount = amount,
                        ExchangeRateUsed = 1,
                        ConversionDate = DateTime.Now
                    };
                }

                var rates = GetCurrentExchangeRates();
                
                decimal fromRate = 1;
                decimal toRate = 1;
                
                if (fromCurrency != "PLN")
                {
                    var fromRateObj = rates.FirstOrDefault(r => r.CurrencyCode == fromCurrency);
                    if (fromRateObj == null)
                        throw new FaultException($"Geçersiz döviz kodu: {fromCurrency}");
                    fromRate = fromRateObj.Rate;
                }
                
                if (toCurrency != "PLN")
                {
                    var toRateObj = rates.FirstOrDefault(r => r.CurrencyCode == toCurrency);
                    if (toRateObj == null)
                        throw new FaultException($"Geçersiz döviz kodu: {toCurrency}");
                    toRate = toRateObj.Rate;
                }
                
                decimal result;
                decimal usedRate;
                
                if (fromCurrency == "PLN")
                {
                    result = amount / toRate;
                    usedRate = toRate;
                }
                else if (toCurrency == "PLN")
                {
                    result = amount * fromRate;
                    usedRate = fromRate;
                }
                else
                {
                    result = (amount * fromRate) / toRate;
                    usedRate = fromRate / toRate;
                }
                
                return new CurrencyConversion
                {
                    FromCurrency = fromCurrency,
                    ToCurrency = toCurrency,
                    Amount = amount,
                    ConvertedAmount = Math.Round(result, 2),
                    ExchangeRateUsed = Math.Round(usedRate, 4),
                    ConversionDate = DateTime.Now
                };
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FaultException($"Döviz çevirme hatası: {ex.Message}");
            }
        }
    }

    // NBP API Models
    public class NbpRate
    {
        public string? currency { get; set; }
        public string? code { get; set; }
        public decimal mid { get; set; }
    }

    public class NbpTableResponse
    {
        public string? table { get; set; }
        public string? no { get; set; }
        public DateTime effectiveDate { get; set; }
        public List<NbpRate>? rates { get; set; }
    }

    public class NbpHistoricalRate
    {
        public string? code { get; set; }
        public List<NbpHistoricalValue>? rates { get; set; }
    }

    public class NbpHistoricalValue
    {
        public DateTime effectiveDate { get; set; }
        public decimal mid { get; set; }
    }
}