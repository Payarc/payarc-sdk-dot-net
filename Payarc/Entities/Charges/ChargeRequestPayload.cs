using Newtonsoft.Json;

namespace Payarc.charges;

public class ChargeRequestPayload
{
    [JsonProperty("amount")]
    public long? Amount { get; set; }
    [JsonProperty("currency")]
    public string? Currency { get; set; }
    [JsonProperty("card_id")]
    public string? CardId { get; set; }
    [JsonProperty("exp_month")]
    public string? ExpMonth { get; set; }
    [JsonProperty("exp_year")]
    public string? ExpYear { get; set; }
    
    [JsonProperty("country")]
    public string? CountyCode { get; set; }
    
    [JsonProperty("state")]
    public string? State { get; set; }
    
    [JsonProperty("city")]
    public string? City { get; set; }
    
    [JsonProperty("address_line1")]
    public string? AddressLine1 { get; set; }
    
    [JsonProperty("zip")]
    public string? ZipCode { get; set; }
    
    [JsonProperty("customer_id")]
    public string? CustomerId { get; set; }
    
    [JsonProperty("object_id")]
    public string? ObjectId { get; set; }
    
    [JsonProperty("token_id")]
    public string? TokenId { get; set; }
    [JsonProperty("bank_account_id")]
    public string? BankAccountId { get; set; }
    [JsonProperty("type")]
    public string? Type { get; set; }
    [JsonProperty("card_number")]
    public string? CardNumber { get; set; }

    public Dictionary<string, object>? Parameters { get; set; }
    public string ToJson()
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
    
        return JsonConvert.SerializeObject(this, settings);
    }
}