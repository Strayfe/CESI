using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CESI.StaticData.Models
{
    [Table("invGroups")]
    public class InventoryGroup
    {
        [Key]
        [Column("groupID")]
        public int GroupId { get; set; }
        
        [Column("categoryID")]
        public int? CategoryId { get; set; }
        
        [Column("groupName")]
        public string? GroupName { get; set; }
        
        [Column("iconID")]
        public int? IconId { get; set; }
        
        [Column("useBasePrice")]
        public bool? UseBasePrice { get; set; }
        
        [Column("anchored")]
        public bool? Anchored { get; set; }

        [Column("anchorable")]
        public bool? Anchorable { get; set; }

        [Column("fittableNonSingleton")]
        public bool? FittableNonSingleton { get; set; }
        
        [Column("published")]
        public bool? Published { get; set; }
    }
}