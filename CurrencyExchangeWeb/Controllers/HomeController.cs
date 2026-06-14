using Microsoft.AspNetCore.Mvc;
using CurrencyExchangeWeb.Services;

namespace CurrencyExchangeWeb.Controllers;

public class HomeController : Controller
{
    private readonly CurrencyExchangeClient _client;

    public HomeController(CurrencyExchangeClient client)
    {
        _client = client;
    }

    public async Task<IActionResult> Index()
    {
        var rates = await _client.GetCurrentExchangeRatesAsync();
        return View(rates);
    }

    [HttpPost]
    public async Task<IActionResult> Convert(string fromCurrency, string toCurrency, decimal amount)
    {
        var result = await _client.ConvertCurrencyAsync(fromCurrency, toCurrency, amount);
        return Json(result);
    }
}