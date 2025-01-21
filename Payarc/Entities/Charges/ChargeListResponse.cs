using Newtonsoft.Json;

namespace Payarc.Entities.Charges;

public class ChargeListResponse : ListBaseResponse
{
    [JsonProperty("charges")]
    public override List<BaseResponse?>? Data { get; set; }
}