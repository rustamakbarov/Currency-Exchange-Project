using System;
using System.Collections.Generic;

namespace CurrencyExchangeService.Models
{
    public class NbpRate
    {
        public string currency { get; set; }
        public string code { get; set; }
        public decimal mid { get; set; }
    }

    public class NbpTableResponse
    {
        public string table { get; set; }
        public string no { get; set; }
        public DateTime effectiveDate { get; set; }
        public List<NbpRate> rates { get; set; }
    }

    public class NbpHistoricalRate
    {
        public string code { get; set; }
        public List<NbpHistoricalValue> rates { get; set; }
    }

    public class NbpHistoricalValue
    {
        public DateTime effectiveDate { get; set; }
        public decimal mid { get; set; }
    }
}