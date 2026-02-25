using InventarioAPI.Models;

namespace InventarioAPI.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // ── Categorías ────────────────────────────────────────────────────
        if (!db.Categorias.Any())
        {
            db.Categorias.AddRange(
                new Categoria { Id = 1, Nombre = "Electrónica",  Descripcion = "Dispositivos y componentes electrónicos" },
                new Categoria { Id = 2, Nombre = "Ropa",         Descripcion = "Prendas de vestir y accesorios" },
                new Categoria { Id = 3, Nombre = "Alimentos",    Descripcion = "Productos alimenticios y bebidas" },
                new Categoria { Id = 4, Nombre = "Hogar",        Descripcion = "Artículos para el hogar" }
            );
            await db.SaveChangesAsync();
        }

        // ── Productos ─────────────────────────────────────────────────────
        if (!db.Productos.Any())
        {
            db.Productos.AddRange(
                new Producto { Nombre = "Laptop Pro 15\"",    Precio = 12999.99m, Stock = 25, SKU = "ELEC-001", CategoriaId = 1 },
                new Producto { Nombre = "Mouse Inalámbrico",  Precio = 349.00m,   Stock = 80, SKU = "ELEC-002", CategoriaId = 1 },
                new Producto { Nombre = "Teclado Mecánico",   Precio = 799.50m,   Stock = 40, SKU = "ELEC-003", CategoriaId = 1 },
                new Producto { Nombre = "Camiseta Algodón",   Precio = 159.00m,   Stock = 150, SKU = "ROPA-001", CategoriaId = 2 },
                new Producto { Nombre = "Jeans Slim Fit",     Precio = 459.00m,   Stock = 60, SKU = "ROPA-002", CategoriaId = 2 },
                new Producto { Nombre = "Café Premium 500g",  Precio = 89.90m,    Stock = 200, SKU = "ALIM-001", CategoriaId = 3 },
                new Producto { Nombre = "Aceite de Oliva 1L", Precio = 119.00m,   Stock = 120, SKU = "ALIM-002", CategoriaId = 3 },
                new Producto { Nombre = "Licuadora 1000W",    Precio = 649.00m,   Stock = 35, SKU = "HOG-001",  CategoriaId = 4 }
            );
            await db.SaveChangesAsync();
        }

        // ── Usuarios ──────────────────────────────────────────────────────
        if (!db.Usuarios.Any())
        {
            db.Usuarios.AddRange(
                new Usuario
                {
                    NombreUsuario = "admin",
                    Email         = "admin@inventario.com",
                    PasswordHash  = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Rol           = "Admin"
                },
                new Usuario
                {
                    NombreUsuario = "usuario",
                    Email         = "usuario@inventario.com",
                    PasswordHash  = BCrypt.Net.BCrypt.HashPassword("User123!"),
                    Rol           = "User"
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
