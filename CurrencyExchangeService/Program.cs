using CoreWCF;
using CoreWCF.Configuration;
using CurrencyExchangeService;

var builder = WebApplication.CreateBuilder(args);

// CoreWCF servisini ekle
builder.Services.AddServiceModelServices();
builder.Services.AddSingleton<CurrencyExchangeServiceImpl>();

var app = builder.Build();

// CoreWCF middleware'ini ekle
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CurrencyExchangeServiceImpl>();
    serviceBuilder.AddServiceEndpoint<CurrencyExchangeServiceImpl, ICurrencyExchange>(new BasicHttpBinding(), "/CurrencyExchange");
});

app.Run();