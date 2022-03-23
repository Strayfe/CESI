using CESI.Models;
using Newtonsoft.Json;

namespace CESI.ResponseModels
{
    public class StructureResponse
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        
        [JsonProperty("owner_id")]
        public int? OwnerId { get; set; }
        
        [JsonProperty("position")] 
        public CartesianCoordinates? Position { get; set; }
        
        [JsonProperty("solar_system_id")]
        public int? SolarSystemId { get; set; }
        
        [JsonProperty("type_id")]
        public int? TypeId { get; set; }
    }
}