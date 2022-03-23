using CESI.StaticData.Models;
using Microsoft.EntityFrameworkCore;

namespace CESI.StaticData.DbContexts
{
    public class StaticDataContext : DbContext
    {
        public StaticDataContext(DbContextOptions<StaticDataContext> options) : base(options)
        {
        }
        
        public DbSet<InventoryType> InventoryTypes { get; set; }
        public DbSet<InventoryGroup> InventoryGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryType>();
            modelBuilder.Entity<InventoryGroup>();
        }
    }
}