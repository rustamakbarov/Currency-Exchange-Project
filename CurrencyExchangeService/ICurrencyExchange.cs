// using System;
// using System.Collections.Generic;
// using CoreWCF;

// namespace CurrencyExchangeService
// {
//     [ServiceContract]
//     public interface ICurrencyExchange
//     {
//         [OperationContract]
//         List<ExchangeRate> GetCurrentExchangeRates();

//         [OperationContract]
//         List<ExchangeRate> GetHistoricalExchangeRates(string currencyCode, string startDate, string endDate);

//         [OperationContract]
//         CurrencyConversion ConvertCurrency(string fromCurrency, string toCurrency, decimal amount);
//     }

//     // [DataContract] attribute'larını kaldırdım
//     public class ExchangeRate
//     {
//         public string CurrencyCode { get; set; }
//         public string CurrencyName { get; set; }
//         public decimal Rate { get; set; }
//         public DateTime Date { get; set; }
//     }

//     public class CurrencyConversion
//     {
//         public string FromCurrency { get; set; }
//         public string ToCurrency { get; set; }
//         public decimal Amount { get; set; }
//         public decimal ConvertedAmount { get; set; }
//         public decimal ExchangeRateUsed { get; set; }
//         public DateTime ConversionDate { get; set; }
//     }
// }

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace CurrencyExchangeService
{
    [ServiceContract]
    public interface ICurrencyExchange
    {
        [OperationContract]
        List<ExchangeRate> GetCurrentExchangeRates();

        [OperationContract]
        List<ExchangeRate> GetHistoricalExchangeRates(string currencyCode, string startDate, string endDate);

        [OperationContract]
        CurrencyConversion ConvertCurrency(string fromCurrency, string toCurrency, decimal amount);
    }

    [DataContract]
    public class ExchangeRate
    {
        [DataMember]
        public string? CurrencyCode { get; set; }

        [DataMember]
        public string? CurrencyName { get; set; }

        [DataMember]
        public decimal Rate { get; set; }

        [DataMember]
        public DateTime Date { get; set; }
    }

    [DataContract]
    public class CurrencyConversion
    {
        [DataMember]
        public string? FromCurrency { get; set; }

        [DataMember]
        public string? ToCurrency { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public decimal ConvertedAmount { get; set; }

        [DataMember]
        public decimal ExchangeRateUsed { get; set; }

        [DataMember]
        public DateTime ConversionDate { get; set; }
    }
}