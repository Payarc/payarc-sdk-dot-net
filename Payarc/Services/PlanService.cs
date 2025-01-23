namespace Payarc.Services;

public class PlanService
{
    private static Dictionary<string, string> _headers;
    private readonly HttpClient _httpClient;

    public PlanService(HttpClient httpClient, Dictionary<string, string> headers)
    {
        _httpClient = httpClient;
        _headers = headers;
    }

    public void Create()
    {
        Console.WriteLine("Create Plan");
    }
}