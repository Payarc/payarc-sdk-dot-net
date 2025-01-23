namespace Payarc.Services;

public class BillingService
{
    private static Dictionary<string, string> _headers;
    private readonly HttpClient _httpClient;

    public BillingService(Dictionary<string, string> headers, HttpClient httpClient )
    {
        _httpClient = httpClient;
        _headers = headers;
        Plan = new PlanService(_httpClient, _headers);
    }
    public PlanService Plan { get; set; }
}