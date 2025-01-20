using System.Configuration;

namespace Payarc;

public static class PayarcConfiguration
{
    private static string? apiKey;
    private static string? agentKey;
    private static string? baseUrl;

    public static string? ApiKey
    {
        get
        {
            if (string.IsNullOrEmpty(apiKey) &&
                !string.IsNullOrEmpty(ConfigurationManager.AppSettings["PayArcApiKey"]))
            {
                apiKey = ConfigurationManager.AppSettings["PayArcApiKey"];
            }

            return apiKey;
        }

        set =>
            apiKey = value;
    }
    
    public static string? AgentKey
    {
        get
        {
            if (string.IsNullOrEmpty(agentKey) &&
                !string.IsNullOrEmpty(ConfigurationManager.AppSettings["PayArcAgentKey"]))
            {
                agentKey = ConfigurationManager.AppSettings["PayArcAgentKey"];
            }

            return agentKey;
        }

        set =>
            agentKey = value;
    }
    
    public static string? BaseUrl
    {
        get
        {
            if (string.IsNullOrEmpty(baseUrl) &&
                !string.IsNullOrEmpty(ConfigurationManager.AppSettings["PayArcBaseUrl"]))
            {
                baseUrl = ConfigurationManager.AppSettings["PayArcBaseUrl"];
            }

            return baseUrl;
        }

        set =>
            baseUrl = value;
    }
}