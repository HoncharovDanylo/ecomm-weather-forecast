using DomainEntities.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<SelectedCity> SelectedCities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(mb =>
        {
            mb.HasMany(u => u.SelectedCities).WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);
        });

        modelBuilder.Entity<SelectedCity>(mb =>
        {
            mb.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}