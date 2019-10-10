using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            RunAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }

    private static async Task RunAsync()
    {
        var config = SampleConfiguration.ReadFromJsonFile("app-settings.json");
        var appConfig = config.PublicClientApplicationOptions;

        // var clientId = ConfigurationManager.AppSettings["clientId"].ToString();

        var app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig)
            .Build();
        var httpClient = new HttpClient();

        var myInformation = new MyInformation(app, httpClient, config.MicrosoftGraphBaseEndpoint);
        await myInformation.DisplayMeAndMyManagerAsync();
    }
}