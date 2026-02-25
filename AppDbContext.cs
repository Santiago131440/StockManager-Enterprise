using InventarioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Producto>  Productos  { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Usuario>   Usuarios   { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unicidad SKU (ignorar nulos)
        modelBuilder.Entity<Producto>()
            .HasIndex(p => p.SKU)
            .IsUnique()
            .HasFilter("SKU IS NOT NULL");

        // Unicidad Email de usuario
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Relación Producto → Categoría
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
