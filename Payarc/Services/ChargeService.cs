using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using AnyOfTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Payarc.charges;
using Payarc.Entities.Charges;

namespace Payarc.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class ChargeService
{
    private static Dictionary<string, string> _headers;
    private readonly HttpClient _httpClient;

    public ChargeService(Dictionary<string, string> headers, HttpClient httpClient)
    {
        _headers = headers;
        _httpClient = httpClient;
    }

    public async Task<BaseResponse?> Create(ChargeCreateOptions obj, ChargeCreateOptions? chargeData = null)

    {
        try
        {
            chargeData = chargeData ?? obj;
            var chargePayload = new ChargeRequestPayload();
            chargePayload.Amount = chargeData.Amount;
            chargePayload.Currency = chargeData.Currency;
            if (chargeData.Source != null)
            {
                var source = chargeData.Source;
                if (source.Value.IsSecond)
                {
                    var second = chargeData.Source.Value.Second;
                    JsonConvert.PopulateObject(JsonConvert.SerializeObject(second), chargePayload);
                }
            }

            if (chargeData.ObjectId != null)
            {
                var objectId = chargeData.ObjectId;
                chargePayload.ObjectId = objectId.StartsWith("cus_") ? objectId.Substring("cus_".Length) : objectId;
            }

            if (chargeData.Source is { IsFirst: true })
            {
                var source = chargeData.Source.Value.First;
                switch (true)
                {
                    case true when source.StartsWith("tok_"):
                        chargePayload.TokenId = source.Substring(4);
                        break;
                    case true when source.StartsWith("cus_"):
                        chargePayload.CustomerId = source.Substring(4);
                        break;
                    case true when source.StartsWith("card_"):
                        chargePayload.CardId = source.Substring(5);
                        break;
                    case true when (source.StartsWith("bnk_") || chargeData.SecCode != null):
                        chargePayload.BankAccountId = source.StartsWith("bnk_")
                            ? source.Substring(4)
                            : chargeData.BankAccountId;
                        chargePayload.Type = "debit";
                        //handle achcharge
                        break;
                    case true when Regex.IsMatch(source, @"^\d"):
                        chargePayload.CardNumber = source;
                        break;
                }
            }

            var idPrefixes = new Dictionary<string, int>
            {
                { "TokenId", 3 },
                { "CustomerId", 3 },
                { "CardId", 4 }
            };
            NormalizeIDs(chargePayload, idPrefixes);
            return await HandleChargeAsync(HttpMethod.Post, "charges", chargePayload);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<BaseResponse?> Retrieve(string chargeId)
    {
        try
        {
            var (endpoint, id) = DetermineEndpointAndId(chargeId);
            return await GetChargeDataAsync(HttpMethod.Get, endpoint, id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<ListBaseResponse?> List(ChargeListOptions options)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "limit", options.Limit ?? 25 },
                { "page", options.Page ?? 1 }
            };
            if (!string.IsNullOrEmpty(options.Search))
            {
                var searchArray = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(options.Search);
                if (searchArray != null)
                {
                    foreach (var searchItem in searchArray)
                    {
                        foreach (var kvp in searchItem)
                        {
                            parameters.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
            }

            var query = BuildQueryString(parameters);
            return await GetChargesAsync("charges", query);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private string BuildQueryString(Dictionary<string, object> parameters)
    {
        var queryString = string.Join("&",
            parameters.Select(p =>
                $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value.ToString() ?? string.Empty)}"));
        return queryString;
    }

    public void CreateRefund()
    {
        Console.WriteLine("Charge refunded.");
    }

    public async Task<BaseResponse?> CreateRefund(AnyOf<string?, ChargeResponseData> charge,
        Dictionary<string, object>? chargeData = null)
    {
        bool achRegular = false;
        string url = "charges";
        string msg = string.Empty;
        string? chargeId = string.Empty;
        Dictionary<string, object>? parameters = null;

        chargeId = charge.IsSecond ? charge.Second.ObjectId : charge.First;

        try
        {
            if (chargeId != null)
            {
                if (chargeId.StartsWith("ch_"))
                {
                    chargeId = chargeId.Substring(3);
                    url = $"{url}/{chargeId}/refunds";
                }
                else if (chargeId.StartsWith("ach_"))
                {
                    achRegular = true;
                    parameters = GetAchChargeParams(charge, chargeData);
                    url = "achcharges";
                    msg = "ACH";
                }
            }

            var response = await HandleChargeAsync(HttpMethod.Post, url, new ChargeRequestPayload
            {
                Parameters = parameters
            });

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing refund for {msg} charge: {ex.Message}");
            throw new InvalidOperationException($"Failed to process refund for {msg} charge", ex);
        }
    }

    private Dictionary<string, object>? GetAchChargeParams(AnyOf<string?, ChargeResponseData?> charge,
        Dictionary<string, object>? parameters = null)
    {
        ChargeResponseData? chargeObj = null;
        if (charge.IsFirst)
        {
            if (charge.First != null)
            {
                chargeObj = (ChargeResponseData?)GetChargeDataAsync(HttpMethod.Get, "charges", charge.First).Result;
            }
        }
        else
        {
            chargeObj = charge.Second;
        }

        if (chargeObj != null)
        {
            // Initialize the parameters if null
            parameters ??= new Dictionary<string, object>();

            // Set default values for ACH-specific parameters
            parameters["type"] = "credit";
            parameters["amount"] = parameters.ContainsKey("amount") ? parameters["amount"] : chargeObj.Amount;
            // parameters["sec_code"] = parameters.ContainsKey("sec_code") ? parameters["sec_code"] : chargeObj.SecCode;

            // Handle bank account-related parameters
            // if (chargeObj.BankAccount?.Data?.ObjectId != null)
            // {
            //     parameters["bank_account_id"] = parameters.ContainsKey("bank_account_id")
            //         ? parameters["bank_account_id"]
            //         : chargeObj.BankAccount.Data.ObjectId;
            // }

            if (parameters.TryGetValue("bank_account_id", out var bankAccountId) &&
                bankAccountId is string bankAccountIdStr)
            {
                if (bankAccountIdStr.StartsWith("bnk_"))
                {
                    parameters["bank_account_id"] = bankAccountIdStr.Substring(4);
                }
            }
        }


        return parameters;
    }

    private async Task<ListBaseResponse?> GetChargesAsync(string endpoint, string? queryParams)
    {
        try
        {
            var url = $"{endpoint}?{queryParams}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var header in _headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response status code: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
                Console.WriteLine($"Error details: {JsonSerializer.Serialize(errorData)}");
                throw new InvalidOperationException($"HTTP error {response.StatusCode}: {responseBody}");
            }

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                throw new InvalidOperationException("Response body is empty.");
            }

            var responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            if (responseData == null || !responseData.TryGetValue("data", out var dataValue) ||
                !(dataValue is JsonElement dataElement))
            {
                throw new InvalidOperationException("Response data is invalid or missing.");
            }

            var rawData = dataElement.GetRawText();
            var jsonCharges = dataElement.Deserialize<List<Dictionary<string, object>>>();
            List<BaseResponse?>? charges = new List<BaseResponse?>();
            if (jsonCharges != null)
            {
                for (int i = 0; i < jsonCharges.Count; i++)
                {
                    var ch = AddObjectId(jsonCharges[i], JsonSerializer.Serialize(jsonCharges[i]));
                    charges?.Add(ch);
                }
            }

            var pagination = new Dictionary<string, object>();
            if (responseData.TryGetValue("meta", out var metaValue) && metaValue is JsonElement metaElement)
            {
                var paginationElement = metaElement.GetProperty("pagination");
                pagination["total"] = paginationElement.GetProperty("total").GetInt32();
                pagination["count"] = paginationElement.GetProperty("count").GetInt32();
                pagination["per_page"] = paginationElement.GetProperty("per_page").GetInt32();
                pagination["current_page"] = paginationElement.GetProperty("current_page").GetInt32();
                pagination["total_pages"] = paginationElement.GetProperty("total_pages").GetInt32();
            }

            pagination?.Remove("links");

            return new ChargeListResponse
            {
                Data = charges,
                Pagination = pagination,
                RawData = rawData
            };
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error processing charge: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON error processing charge: {ex.Message}");
            throw new InvalidOperationException("Failed to process JSON response.", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error handling charge: {ex.Message}");
            throw;
        }
    }

    private Dictionary<string, string> GetParams(string endpoint)
    {
        if (endpoint == "charges")
        {
            return new Dictionary<string, string>
            {
                { "include", "transaction_metadata,extra_metadata" }
            };
        }

        if (endpoint == "achcharges")
        {
            return new Dictionary<string, string>
            {
                { "include", "review" }
            };
        }

        return new Dictionary<string, string>();
    }

    private (string endpoint, string id) DetermineEndpointAndId(string chargeId)
    {
        if (chargeId.StartsWith("ch_"))
        {
            return ("charges", chargeId.Substring(3));
        }

        if (chargeId.StartsWith("ach_"))
        {
            return ("achcharges", chargeId.Substring(4));
        }

        throw new Exception("Invalid charge ID format.");
    }

    private async Task<BaseResponse?> GetChargeDataAsync(HttpMethod method, string endpoint, string chargeId)
    {
        try
        {
            var path = $"{endpoint}/{chargeId}";
            var parameters = GetParams(endpoint);
            var request = new HttpRequestMessage(method, path)
            {
                Content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json")
            };
            foreach (var header in _headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response status code: {response.StatusCode}");
            // Console.WriteLine($"Response body: {responseBody}");
            if (!response.IsSuccessStatusCode)
            {
                var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
                Console.WriteLine($"Error details: {JsonSerializer.Serialize(errorData)}");
                throw new InvalidOperationException($"HTTP error {response.StatusCode}: {responseBody}");
            }

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                throw new InvalidOperationException("Response body is empty.");
            }

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            if (data != null && data.TryGetValue("data", out var dataValue) && dataValue is JsonElement dataElement)
            {
                var dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(dataElement.GetRawText());
                if (dataDict != null)
                {
                    return AddObjectId(dataDict, dataElement.GetRawText());
                }
            }

            throw new InvalidOperationException("Response data is invalid or missing.");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error processing charge: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON error processing charge: {ex.Message}");
            throw new InvalidOperationException("Failed to process JSON response.", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error handling charge: {ex.Message}");
            throw;
        }
    }

    public void NormalizeIDs(ChargeRequestPayload payload, Dictionary<string, int> idPrefixes)
    {
        foreach (var (key, prefixLength) in idPrefixes)
        {
            var property = payload.GetType().GetProperty(key);
            if (property != null && property.PropertyType == typeof(string))
            {
                var value = property.GetValue(payload) as string;
                if (!string.IsNullOrEmpty(value) &&
                    value.StartsWith(key.ToLower().Substring(0, prefixLength) + "_"))
                {
                    property.SetValue(payload, value.Substring(prefixLength + 1));
                }
            }
        }
    }

    public async Task<BaseResponse?> HandleChargeAsync(HttpMethod method, string path,
        ChargeRequestPayload chargeData)
    {
        try
        {
            var request = new HttpRequestMessage(method, path)
            {
                Content = new StringContent(chargeData.ToJson(), Encoding.UTF8, "application/json")
            };
            Console.WriteLine(chargeData.ToJson());
            foreach (var header in _headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response status code: {response.StatusCode}");
            // Console.WriteLine($"Response body: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
                    Console.WriteLine($"Error details: {JsonSerializer.Serialize(errorData)}");
                    throw new InvalidOperationException($"HTTP error {response.StatusCode}: {responseBody}");
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"Failed to parse error JSON: {jsonEx.Message}");
                    throw new InvalidOperationException(
                        $"HTTP error {response.StatusCode}: Unable to parse error response.");
                }
            }

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                throw new InvalidOperationException("Response body is empty.");
            }

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            if (data != null && data.TryGetValue("data", out var dataValue) && dataValue is JsonElement dataElement)
            {
                var dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(dataElement.GetRawText());
                if (dataDict != null)
                {
                    return AddObjectId(dataDict, dataElement.GetRawText());
                }
            }

            throw new InvalidOperationException("Response data is invalid or missing.");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error processing charge: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON error processing charge: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error handling charge: {ex.Message}");
            throw;
        }
    }

    private BaseResponse? AddObjectId(Dictionary<string, object> obj, string? rawObj)
    {
        BaseResponse? response = null;
        if (obj["object"]?.ToString() == "Charge")
        {
            if (rawObj != null)
            {
                var chargeResponse = JsonConvert.DeserializeObject<ChargeResponseData>(rawObj);
                if (chargeResponse == null)
                {
                    chargeResponse = new ChargeResponseData();
                }

                chargeResponse.RawData = rawObj;
                chargeResponse.ObjectId ??= $"ch_{obj["id"]}";
                chargeResponse.CreateRefund = async (chargeData) => await CreateRefund(chargeResponse, chargeData);
                response = chargeResponse;
            }
        }

        return response;
    }
}