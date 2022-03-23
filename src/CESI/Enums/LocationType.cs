using System.Runtime.Serialization;

namespace CESI.Enums
{
    public enum LocationType
    {
        [EnumMember(Value = "station")] Station,
        [EnumMember(Value = "solar_system")] SolarSystem,
        [EnumMember(Value = "item")] Item,
        [EnumMember(Value = "other")] Other
    }
}