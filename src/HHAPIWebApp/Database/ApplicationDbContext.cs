using Microsoft.Data.Entity; 
using Microsoft.AspNet.Identity.EntityFramework;



namespace HHAPIWebApp.Models
{
    //  public class MyDbContext : DbContext
    //{
    // public DbSet<ApplicationUser> Users { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //{
    //options.UseSqlServer(@"Server=(localdb)\\MSSQLLocalDB;Database=_CHANGE_ME;Trusted_Connection=True;");
    //    options.UseNpgsql(@"Server=127.0.0.1;Port=5432;Database=HHAPIWebApp;Integrated Security=true;User Id=postgres;Password=123;");
    // }
    //}

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }
        // public DbSet<ApplicationUser> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //options.UseSqlServer(@"Server=(localdb)\\MSSQLLocalDB;Database=_CHANGE_ME;Trusted_Connection=True;");
            options.UseNpgsql(@"Server=127.0.0.1;Port=5432;Database=HHAPIWebApp;Integrated Security=true;User Id=postgres;Password=123;");
        }
    }
}