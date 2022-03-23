using System.Collections.Generic;
using CESI.Models;
using Newtonsoft.Json;

namespace CESI.ResponseModels
{
    public class SystemResponse
    {
        [JsonProperty("constellation_id")]
        public int? ConstellationId { get; set; }
        
        [JsonProperty("name")]
        public string? SystemName { get; set; }
        
        [JsonProperty("planets")]
        public IEnumerable<Planet>? Planets { get; set; }
        
        [JsonProperty("position")]
        public CartesianCoordinates? Position { get; set; }
        
        [JsonProperty("security_class")]
        public string? SecurityClass { get; set; }
        
        [JsonProperty("security_status")]
        public float? SecurityStatus { get; set; }
        
        [JsonProperty("star_id")]
        public int? StarId { get; set; }
        
        [JsonProperty("stargates")]
        public IEnumerable<int>? Stargates { get; set; }
        
        [JsonProperty("stations")]
        public IEnumerable<int>? Stations { get; set; }
        
        [JsonProperty("system_id")]
        public int? SystemId { get; set; }
    }
}