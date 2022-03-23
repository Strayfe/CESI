using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CESI.StaticData.Models
{
    [Table("invTypes")]
    public class InventoryType
    {
        [Key]
        [Column("typeID")]
        public int TypeId { get; set; }
        
        [Column("groupID")]
        public int? GroupId { get; set; }
        
        [Column("typeName")]
        public string? TypeName { get; set; }
        
        [Column("description")]
        public string? Description { get; set; }
        
        [Column("mass")]
        public double? Mass { get; set; }
        
        [Column("volume")]
        public double? Volume { get; set; }
        
        [Column("capacity")]
        public double? Capacity { get; set; }
        
        [Column("portionSize")]
        public int? PortionSize { get; set; }
        
        [Column("raceID")]
        public int? RaceId { get; set; }
        
        [Column("basePrice", TypeName = "decimal(19,4)")]
        public float? BasePrice { get; set; }
        
        [Column("published")]
        public bool? Published { get; set; }
        
        [Column("marketGroupID")]
        public int? MarketGroupId { get; set; }
        
        [Column("iconID")]
        public int? IconId { get; set; }
        
        [Column("soundID")]
        public int? SoundId { get; set; }
        
        [Column("graphicID")]
        public int? GraphicId { get; set; }
    }
}