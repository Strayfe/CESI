using CESI.ResponseModels;
using Newtonsoft.Json;

namespace CESI.Models
{
    public class BlueprintResponse : AssetResponse
    {
        [JsonProperty("material_efficiency")]
        public int? MaterialEfficiency { get; set; }
        
        [JsonProperty("runs")] 
        public int? Runs { get; set; }

        [JsonProperty("time_efficiency")]
        public int? TimeEfficiency { get; set; }
    }
}