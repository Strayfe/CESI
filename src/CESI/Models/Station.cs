using System.Collections.Generic;
using CESI.Enums;
using Newtonsoft.Json;

namespace CESI.Models
{
    public class Station
    {
        [JsonProperty("max_dockable_ship_volume")]
        public float? MaxDockableShipVolume { get; set; }
        
        [JsonProperty("name")]
        public string? Name { get; set; }
        
        [JsonProperty("office_rental_cost")]
        public float? OfficeRentalCost { get; set; }
        
        [JsonProperty("owner")]
        public int? Owner { get; set; }
        
        [JsonProperty("position")]
        public CartesianCoordinates? Position { get; set; }
        
        [JsonProperty("race_id")]
        public int? RaceId { get; set; }
        
        [JsonProperty("reprocessing_efficiency")]
        public float? ReprocessingEfficiency { get; set; }
        
        [JsonProperty("reprocessing_stations_take")]
        public float? ReprocessingStationsTake { get; set; }
        
        [JsonProperty("services")]
        public IEnumerable<Service>? Services { get; set; }
        
        [JsonProperty("station_id")]
        public int? StationId { get; set; }
        
        [JsonProperty("system_id")]
        public int? SystemId { get; set; }
        
        [JsonProperty("type_id")]
        public int? TypeId { get; set; }
    }
}