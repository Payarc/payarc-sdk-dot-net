using Newtonsoft.Json;
namespace Payarc;

public abstract class BaseResponse
{
    [JsonProperty("object")]
    public string? Object { get; set; }
    [JsonProperty("id")]
    public string? Id { get; set; }
    [JsonIgnore]
    public string? RawData { get; set; }
        
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}