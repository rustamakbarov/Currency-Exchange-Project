using System.ServiceModel;
using System.Xml;

namespace CurrencyExchangeWeb.Services;

public class CurrencyExchangeClient
{
    private readonly HttpClient _httpClient;
    private readonly string _soapEndpoint;

    public CurrencyExchangeClient(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _soapEndpoint = configuration["CurrencyExchangeService:Url"] ?? "http://localhost:5126/CurrencyExchange";
    }

    public async Task<List<ExchangeRate>> GetCurrentExchangeRatesAsync()
    {
        string soapRequest = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""
               xmlns:tem=""http://tempuri.org/"">
  <soap:Header/>
  <soap:Body>
    <tem:GetCurrentExchangeRates/>
  </soap:Body>
</soap:Envelope>";

        var content = new StringContent(soapRequest, System.Text.Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "http://tempuri.org/ICurrencyExchange/GetCurrentExchangeRates");

        var response = await _httpClient.PostAsync(_soapEndpoint, content);
        var responseText = await response.Content.ReadAsStringAsync();

        return ParseExchangeRates(responseText);
    }

    public async Task<CurrencyConversion> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
    {
    // SOAP request 
    string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""
               xmlns:tem=""http://tempuri.org/"">
  <soap:Header/>
  <soap:Body>
    <tem:ConvertCurrency>
      <tem:fromCurrency>{fromCurrency}</tem:fromCurrency>
      <tem:toCurrency>{toCurrency}</tem:toCurrency>
      <tem:amount>{amount.ToString(System.Globalization.CultureInfo.InvariantCulture)}</tem:amount>
    </tem:ConvertCurrency>
  </soap:Body>
</soap:Envelope>";

    var content = new StringContent(soapRequest, System.Text.Encoding.UTF8, "text/xml");
    content.Headers.Add("SOAPAction", "http://tempuri.org/ICurrencyExchange/ConvertCurrency");

    var response = await _httpClient.PostAsync(_soapEndpoint, content);
    var responseText = await response.Content.ReadAsStringAsync();
    
    // Debug 
    Console.WriteLine("=== SOAP RESPONSE ===");
    Console.WriteLine(responseText);
    
    return ParseConversionResult(responseText);
    }

    private List<ExchangeRate> ParseExchangeRates(string xmlResponse)
    {
        var rates = new List<ExchangeRate>();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlResponse);

        var nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
        nsMgr.AddNamespace("a", "http://schemas.datacontract.org/2004/07/CurrencyExchangeService");

        var nodes = xmlDoc.SelectNodes("//a:ExchangeRate", nsMgr);
        
        if (nodes != null)
        {
            foreach (XmlNode node in nodes)
            {
                var rate = new ExchangeRate
                {
                    CurrencyCode = node.SelectSingleNode("a:CurrencyCode", nsMgr)?.InnerText ?? "",
                    CurrencyName = node.SelectSingleNode("a:CurrencyName", nsMgr)?.InnerText ?? "",
                    Rate = decimal.TryParse(node.SelectSingleNode("a:Rate", nsMgr)?.InnerText, 
                        System.Globalization.NumberStyles.Any, 
                        System.Globalization.CultureInfo.InvariantCulture, out var r) ? r : 0,
                    Date = DateTime.Now
                };
                rates.Add(rate);
            }
        }
        return rates;
    }

    private CurrencyConversion ParseConversionResult(string xmlResponse)
    {
    var result = new CurrencyConversion();
    var xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(xmlResponse);

    var nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
    nsMgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
    nsMgr.AddNamespace("a", "http://schemas.datacontract.org/2004/07/CurrencyExchangeService");
    nsMgr.AddNamespace("tem", "http://tempuri.org/");

    // First response body
    var bodyNode = xmlDoc.SelectSingleNode("//s:Body", nsMgr);
    if (bodyNode != null)
    {
        // ConvertCurrencyResponse 
        var resultNode = bodyNode.SelectSingleNode(".//tem:ConvertCurrencyResponse/tem:ConvertCurrencyResult", nsMgr);
        if (resultNode == null)
        {
            resultNode = bodyNode.SelectSingleNode(".//*[local-name()='ConvertCurrencyResult']", nsMgr);
        }
        
        if (resultNode != null)
        {
            result.FromCurrency = GetNodeValue(resultNode, "a:FromCurrency", nsMgr);
            result.ToCurrency = GetNodeValue(resultNode, "a:ToCurrency", nsMgr);
            result.Amount = GetDecimalValue(resultNode, "a:Amount", nsMgr);
            result.ConvertedAmount = GetDecimalValue(resultNode, "a:ConvertedAmount", nsMgr);
            result.ExchangeRateUsed = GetDecimalValue(resultNode, "a:ExchangeRateUsed", nsMgr);
            result.ConversionDate = DateTime.Now;
        }
    }
    
    return result;
    }

private string GetNodeValue(XmlNode parentNode, string xpath, XmlNamespaceManager nsMgr)
{
    var node = parentNode.SelectSingleNode(xpath, nsMgr);
    return node?.InnerText ?? "";
}

private decimal GetDecimalValue(XmlNode parentNode, string xpath, XmlNamespaceManager nsMgr)
{
    var node = parentNode.SelectSingleNode(xpath, nsMgr);
    if (node != null && decimal.TryParse(node.InnerText, 
        System.Globalization.NumberStyles.Any, 
        System.Globalization.CultureInfo.InvariantCulture, 
        out var value))
    {
        return value;
    }
    return 0;
}
}

public class ExchangeRate
{
    public string CurrencyCode { get; set; } = "";
    public string CurrencyName { get; set; } = "";
    public decimal Rate { get; set; }
    public DateTime Date { get; set; }
}

public class CurrencyConversion
{
    public string FromCurrency { get; set; } = "";
    public string ToCurrency { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal ConvertedAmount { get; set; }
    public decimal ExchangeRateUsed { get; set; }
    public DateTime ConversionDate { get; set; }
}