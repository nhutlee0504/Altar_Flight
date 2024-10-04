using API_Flight_Altar.Model;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API_Flight_Altar.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<User> users { get; set; }
        public DbSet<TypeDoc> typeDocs { get; set; }
        public DbSet<GroupModel> groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TypeDoc>()
                .HasOne(o => o.User)
                .WithMany(c => c.typeDocs)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<GroupModel>()
                .HasOne(o => o.User)
                .WithMany(c => c.groups)
                .HasForeignKey(o => o.UserId);
        }
    }
}
