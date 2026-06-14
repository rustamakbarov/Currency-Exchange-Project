# 💱 Currency Exchange Office System

A complete currency exchange system with WCF backend and ASP.NET Core MVC frontend, integrated with NBP (National Bank of Poland) real-time API.

Course name: Network Application Development
Rustam Akbarov
Student ID number: 73652

## 🚀 Features

- **Real-time Exchange Rates** - Fetches live rates for 32 currencies from NBP API
- **Currency Conversion** - Convert between any two currencies (PLN, USD, EUR, GBP, TRY, etc.)
- **Historical Data** - Query historical exchange rates for any date range
- **Modern UI** - Responsive Bootstrap 5 interface with real-time updates

## 🛠️ Technology Stack

| Layer           | Technology                       |
| --------------- | -------------------------------- |
| Backend API     | WCF / CoreWCF                    |
| Frontend        | ASP.NET Core MVC                 |
| UI Framework    | Bootstrap 5                      |
| API Integration | NBP REST API (http://api.nbp.pl) |
| Communication   | SOAP/HTTP                        |
| Language        | C# 12 / .NET 8                   |

## 📋 Prerequisites

- .NET 8 SDK or later
- Visual Studio 2022 / VS Code / JetBrains Rider

## 🏃‍♂️ Running the Project

### 1. Start the WCF Service

```bash
cd CurrencyExchangeService
dotnet run
```
