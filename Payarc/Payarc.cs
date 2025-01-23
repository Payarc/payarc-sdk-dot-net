using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Payarc.charges;
using Payarc.Entities.Charges;
using Payarc.Services;

namespace Payarc;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class Payarc
{
    private readonly string? bearerToken;
    private readonly string version;
    private string baseURL;
    private readonly string? bearerTokenAgent;
    private readonly HttpClient _httpClient;
    private static Dictionary<string, string> _headers;

    public ChargeService Charges { get; private set; }
    public BillingService Billing { get; private set; }

    public Payarc(string apiVersion = "/v1/",
        string version = "1.0")
    {
        this.bearerToken = PayarcConfiguration.ApiKey;
        if (string.IsNullOrEmpty(this.bearerToken))
        {
            throw new ArgumentNullException(nameof(PayarcConfiguration.ApiKey), "Api Key is required");
        }

        this.version = version;
        baseURL = PayarcConfiguration.BaseUrl ?? "sandbox";
        baseURL = baseURL == "prod"
            ? "https://api.payarc.net"
            : (baseURL == "sandbox" ? "https://testapi.payarc.net" : baseURL);
        baseURL = apiVersion == "/v1/" ? $"{baseURL}{apiVersion}" : $"{baseURL}/v{apiVersion}/";
        this.bearerTokenAgent = PayarcConfiguration.AgentKey;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseURL)
        };
        _headers = new Dictionary<string, string>
        {
            // { "Content-Type", "application/json" },
            { "Accept", "application/json" },
            { "Authorization", $"Bearer {this.bearerToken}" }
        };

        // Initialize Charges operations
        Charges = new ChargeService(_headers, _httpClient);
        Billing = new BillingService(_headers, _httpClient);
    }
    
}