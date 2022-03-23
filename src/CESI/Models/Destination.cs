using Newtonsoft.Json;

namespace CESI.Models
{
    public class Destination
    {
        [JsonProperty("stargate_id")]
        public int? StargateId { get; set; }
        
        [JsonProperty("system_id")]
        public int? SystemId { get; set; }
    }
}