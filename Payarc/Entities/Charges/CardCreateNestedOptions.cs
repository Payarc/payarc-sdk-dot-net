namespace Payarc;
using Newtonsoft.Json;
public class CardCreateNestedOptions
{
    [JsonProperty("card_id")]
    public string? CardId { get; set; }
    [JsonProperty("customer_id")]
    public string? CustomerId { get; set; }
    [JsonProperty("card_number")]
    public string? CardNumber { get; set; }
    [JsonProperty("exp_month")]
    public string? ExpMonth { get; set; }
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
    [JsonProperty("exp_year")]
    public string? ExpYear { get; set; }
    [JsonProperty("token_id")]
    public string? TokenId { get; set; }
}