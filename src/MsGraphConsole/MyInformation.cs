﻿/*
 The MIT License (MIT)

Copyright (c) 2015 Microsoft Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

public class MyInformation
{
    protected ProtectedApiCallHelper protectedApiCallHelper;

    protected PublicAppUsingDeviceCodeFlow tokenAcquisitionHelper;

    public MyInformation(IPublicClientApplication app, HttpClient client, string microsoftGraphBaseEndpoint)
    {
        tokenAcquisitionHelper = new PublicAppUsingDeviceCodeFlow(app);
        protectedApiCallHelper = new ProtectedApiCallHelper(client);
        MicrosoftGraphBaseEndpoint = microsoftGraphBaseEndpoint;
    }

    /// <summary>
    ///     Scopes to request access to the protected Web API (here Microsoft Graph)
    /// </summary>
    private static string[] Scopes { get; } = {"User.Read", "User.ReadBasic.All"};

    /// <summary>
    ///     Base endpoint for Microsoft Graph
    /// </summary>
    private string MicrosoftGraphBaseEndpoint { get; }

    /// <summary>
    ///     URLs of the protected Web APIs to call (here Microsoft Graph endpoints)
    /// </summary>
    private string WebApiUrlMe => $"{MicrosoftGraphBaseEndpoint}/v1.0/me";

    private string WebApiUrlMyManager => $"{MicrosoftGraphBaseEndpoint}/v1.0/me/manager";

    /// <summary>
    ///     Calls the Web API and displays its information
    /// </summary>
    /// <returns></returns>
    public async Task DisplayMeAndMyManagerAsync()
    {
        var authenticationResult = await tokenAcquisitionHelper.AcquireATokenFromCacheOrDeviceCodeFlowAsync(Scopes);
        if (authenticationResult != null)
        {
            DisplaySignedInAccount(authenticationResult.Account);

            var accessToken = authenticationResult.AccessToken;
            await CallWebApiAndDisplayResultAsync(WebApiUrlMe, accessToken, "Me");
            await CallWebApiAndDisplayResultAsync(WebApiUrlMyManager, accessToken, "My manager");
        }
    }

    private static void DisplaySignedInAccount(IAccount account)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{account.Username} successfully signed-in");
    }

    private async Task CallWebApiAndDisplayResultAsync(string url, string accessToken, string title)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(title);
        Console.ResetColor();
        await protectedApiCallHelper.CallWebApiAndProcessResultAsync(url, accessToken, Display);
        Console.WriteLine();
    }

    /// <summary>
    ///     Display the result of the Web API call
    /// </summary>
    /// <param name="result">Object to display</param>
    private static void Display(JObject result)
    {
        foreach (var child in result.Properties().Where(p => !p.Name.StartsWith('@')))
            Console.WriteLine($"{child.Name} = {child.Value}");
    }
}