using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Base.Models;

namespace Ark.Models
{
    public partial class DatabaseContext : DbContext
    {

        public DatabaseContext()
            : base("name=DefaultConnectionString")
        {
            Database.SetInitializer<DatabaseContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
    }
}
