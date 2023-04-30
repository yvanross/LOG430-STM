using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Entities;
using Microsoft.Extensions.Configuration;

namespace AuthService.Dao
{
    public class UsersContext : IdentityUserContext<LabUser>
    {
        public UsersContext() { }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }

        public DbSet<LabUser> LabUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity(typeof(LabUser)).HasKey("UserName");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = $"Server={HostInfo.ServiceAddress};Port={HostInfo.PostgresPort};Username=postgres;Password=secret;Database=postgres;";
                
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}
