
using Microsoft.Extensions.Configuration;

namespace Payarc;
public static class PayarcConfiguration
{
    private static string? apiKey;
    private static string? agentKey;
    private static string? baseUrl;
    
    private static IConfiguration? _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static string? ApiKey
    {
        get
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = _configuration?["PayArcApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("API Key is required but not provided.");
                }
            }
            return apiKey;
        }
        set => apiKey = value;
    }
    
    public static string? AgentKey
    {
        get
        {
            if (string.IsNullOrEmpty(agentKey))
            {
                agentKey = _configuration?["PayArcAgentKey"];
                if (string.IsNullOrEmpty(agentKey))
                {
                    throw new InvalidOperationException("Agent Key is required but not provided.");
                }
            }
            return agentKey;
        }
        set => agentKey = value;
    }
    
    public static string? BaseUrl
    {
        get
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = _configuration?["PayArcBaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("Base URL is required but not provided.");
                }
            }
            return baseUrl;
        }
        set => baseUrl = value;
    }
}