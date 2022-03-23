using Newtonsoft.Json;

namespace CESI.Models
{
    public class CartesianCoordinates
    {
        [JsonProperty("x")]
        public double? X { get; set; }
        
        [JsonProperty("y")]
        public double? Y { get; set; }
        
        [JsonProperty("z")]
        public double? Z { get; set; }
    }
}