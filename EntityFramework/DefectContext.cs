using Microsoft.EntityFrameworkCore;
using EntityFramework.Models;

namespace EntityFramework
{
    public class DefectContext : DbContext
    {
        public DbSet<Defect> Defects { get; set; }

        private string DatabasePath { get; set; }

        public DefectContext()
        {

        }

        public DefectContext(string databasePath)
        {
            DatabasePath = databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DatabasePath}");
        }
    }
}
