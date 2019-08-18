using Microsoft.EntityFrameworkCore;
using User.Api.Model;

namespace User.Api.Data
{
    public class UserContext : DbContext
    {
        private DbContextOptionsBuilder<UserContext> options;

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserProperty> UserProperty { get; set; }

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().ToTable("Users").HasKey(u => u.Id); 
            modelBuilder.Entity<UserProperty>().ToTable("UserProperties").HasKey(u => new { u.Key, u.AppUserId, u.Value });
            modelBuilder.Entity<UserTag>().ToTable("UserTags").HasKey(u => new { u.AppUserId, u.Tag });
            modelBuilder.Entity<BPfile>().ToTable("UserBPFiles").HasKey(u => u.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
