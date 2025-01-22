﻿using System.Text.Json;
using Microsoft.Extensions.Configuration;
namespace TestPayarcSDK;

using Payarc;

public static class Program
{
    private static Payarc? _payarc;

    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        PayarcConfiguration.Initialize(configuration);
        PayarcConfiguration.BaseUrl = "http://localapi6.payarc.net";

        _payarc = new Payarc();

        // await CreateChargeExample(); 
        // await CreateChargeByCardIdExample();
        // await GetChargeById();
        await ListCharges();
    }


    private static async Task CreateChargeExample()
    {
        try
        {
            var options = new ChargeCreateOptions
            {
                Amount = 185,
                Source = new CardCreateNestedOptions
                {
                    CardNumber = "4012000098765439",
                    ExpMonth = "03",
                    ExpYear = "2025",
                    CountyCode = "USA"
                },
                Currency = "usd"
            };
            var charge = await _payarc.Charges.Create(options);
            Console.WriteLine("Charge Data");
            Console.WriteLine(charge);
            Console.WriteLine("Raw Data");
            Console.WriteLine(charge?.RawData);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
       
        
    }

    private static async Task CreateChargeByCardIdExample()
    {
        var options = new ChargeCreateOptions
        {
            Amount = 155,
            Source = new CardCreateNestedOptions
            {
                CardId = "card_Ly9v09NN2P59M0m1",
                CustomerId = "cus_jMNKVMPKnNxPVnDp"
            },
            Currency = "usd"
        };
        var charge = await _payarc.Charges.Create(options);
        Console.WriteLine("Charge Data");
        Console.WriteLine(charge);
        Console.WriteLine("Raw Data");
        Console.WriteLine(charge?.RawData);
    }

    private static async Task GetChargeById()
    {
        // var charge = await Payarc.Charges.Retrieve("ch_MnBROWLXBBXnoOWL");
        var charge = await _payarc.Charges.Retrieve("ch_XMbnObBXDDbMXORo");
        Console.WriteLine("Get charge By Id Data");
        Console.WriteLine(charge);
        Console.WriteLine("Raw Data");
        Console.WriteLine(charge?.RawData);
    }

    private static async Task ListCharges()
    {
        var options = new ChargeListOptions()
        {
            Limit = 25,
            Page = 1,
        };
        var responseData = await _payarc.Charges.List(options);
         Console.WriteLine("Charges Data");
         for (int i = 0; i < responseData?.Data?.Count; i++)
         {
             var t = responseData.Data[i];
             Console.WriteLine(responseData.Data[i]);
         }
         Console.WriteLine("Pagination Data");
         Console.WriteLine(responseData?.Pagination["total"]);
         Console.WriteLine(responseData?.Pagination["count"]);
         Console.WriteLine(responseData?.Pagination["per_page"]);
         Console.WriteLine(responseData?.Pagination["current_page"]);
         Console.WriteLine(responseData?.Pagination["total_pages"]);
         // Console.WriteLine("Raw Data");
         // Console.WriteLine(responseData?.RawData);
    }
}