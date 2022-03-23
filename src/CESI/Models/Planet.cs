using System.Collections.Generic;
using Newtonsoft.Json;

namespace CESI.Models
{
    public class Planet
    {
        [JsonProperty("asteroid_belts")]
        public IEnumerable<int>? AsteroidBelts { get; set; }
        
        [JsonProperty("moons")]
        public IEnumerable<int>? Moons { get; set; }
        
        [JsonProperty("planet_id")]
        public int? PlanetId { get; set; }
    }
}