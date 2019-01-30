using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DefectServer.Models
{
    public class DefectServerContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public DefectServerContext() : base("name=DefectServerContext")
        {
        }

        public DbSet<Defect> Defects { get; set; }

        public override Task<int> SaveChangesAsync()
        {
            DateTime saveTime = DateTime.UtcNow;
            foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                if (entry.Property("DateCreated").CurrentValue == null)
                    entry.Property("DateCreated").CurrentValue = saveTime;
            }
            return base.SaveChangesAsync();
        }
    }
}
