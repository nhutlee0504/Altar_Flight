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
        public DbSet<Group_User> group_Users { get; set; }
        public DbSet<Permission> permissions { get; set; }
        public DbSet<Group_Type> group_Types { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TypeDoc>()
                .HasOne(o => o.User)
                .WithMany(c => c.typeDocs)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Thay đổi ở đây

            modelBuilder.Entity<GroupModel>()
                .HasOne(o => o.User)
                .WithMany(c => c.groups)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Thay đổi ở đây

            modelBuilder.Entity<Group_User>()
                .HasOne(o => o.User)
                .WithMany(c => c.group_Users)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict); // Thay đổi ở đây

            modelBuilder.Entity<Group_User>()
                .HasOne(o => o.Group)
                .WithMany(c => c.group_Users)
                .HasForeignKey(o => o.GroupID);

            modelBuilder.Entity<Group_Type>()
                .HasOne(o => o.GroupModel)
                .WithMany(c => c.group_Types)
                .HasForeignKey(o => o.IdGroup);

            modelBuilder.Entity<Group_Type>()
                .HasOne(o => o.TypeDoc)
                .WithMany(c => c.group_Types)
                .HasForeignKey(o => o.IdType);

            modelBuilder.Entity<Group_Type>()
                .HasOne(o => o.Permission)
                .WithMany(c => c.group_Types)
                .HasForeignKey(o => o.IdPermission)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
