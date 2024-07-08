using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ItemContext(DbContextOptions<ItemContext> options) : DbContext(options)
{
    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>().ToTable("items");
        modelBuilder.Entity<Item>().Property(x => x.Id).HasColumnName("id");
        modelBuilder.Entity<Item>().Property(x => x.Name).HasColumnName("name");
        modelBuilder.Entity<Item>().Property(x => x.Description).HasColumnName("description");
        modelBuilder.Entity<Item>().Property(x => x.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Item>().Property(x => x.Version).HasColumnName("version");
    }
}