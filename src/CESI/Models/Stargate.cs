using Newtonsoft.Json;

namespace CESI.Models
{
    public class Stargate
    {
        [JsonProperty("destination")]
        public Destination? Destination { get; set; }
        
        [JsonProperty("name")]
        public string? Name { get; set; }
        
        [JsonProperty("position")]
        public CartesianCoordinates? Position { get; set; }
        
        [JsonProperty("stargate_id")]
        public int? StargateId { get; set; }
        
        [JsonProperty("system_id")]
        public int? SystemId { get; set; }
        
        [JsonProperty("type_id")]
        public int? TypeId { get; set; }
    }
}