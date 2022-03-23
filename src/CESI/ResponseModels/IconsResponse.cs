using Newtonsoft.Json;

// ReSharper disable InconsistentNaming
namespace CESI.ResponseModels
{
    public class IconsResponse
    {
        [JsonProperty("px64x64")]
        public string? Px64x64 { get; set; }
        
        [JsonProperty("px128x128")]
        public string? Px128x128 { get; set; }
        
        [JsonProperty("px256x256")]
        public string? Px256x256 { get; set; }
        
        [JsonProperty("px512x512")]
        public string? Px512x512 { get; set; }
    }
}