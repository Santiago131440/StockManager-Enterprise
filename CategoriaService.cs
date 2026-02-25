using InventarioAPI.Data;
using InventarioAPI.DTOs;
using InventarioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Services;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDto>> ObtenerTodasAsync();
    Task<CategoriaDto?>             ObtenerPorIdAsync(int id);
    Task<CategoriaDto>              CrearAsync(CrearCategoriaDto dto);
    Task<CategoriaDto?>             ActualizarAsync(int id, CrearCategoriaDto dto);
    Task<bool>                      EliminarAsync(int id);
}

public class CategoriaService(AppDbContext db) : ICategoriaService
{
    public async Task<IEnumerable<CategoriaDto>> ObtenerTodasAsync()
    {
        return await db.Categorias
            .Where(c => c.Activo)
            .Select(c => new CategoriaDto
            {
                Id              = c.Id,
                Nombre          = c.Nombre,
                Descripcion     = c.Descripcion,
                Activo          = c.Activo,
                TotalProductos  = c.Productos.Count(p => p.Activo)
            })
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<CategoriaDto?> ObtenerPorIdAsync(int id)
    {
        var c = await db.Categorias.Include(x => x.Productos).FirstOrDefaultAsync(x => x.Id == id);
        return c is null ? null : new CategoriaDto
        {
            Id             = c.Id,
            Nombre         = c.Nombre,
            Descripcion    = c.Descripcion,
            Activo         = c.Activo,
            TotalProductos = c.Productos.Count(p => p.Activo)
        };
    }

    public async Task<CategoriaDto> CrearAsync(CrearCategoriaDto dto)
    {
        var cat = new Categoria { Nombre = dto.Nombre, Descripcion = dto.Descripcion };
        db.Categorias.Add(cat);
        await db.SaveChangesAsync();
        return new CategoriaDto { Id = cat.Id, Nombre = cat.Nombre, Descripcion = cat.Descripcion, Activo = cat.Activo };
    }

    public async Task<CategoriaDto?> ActualizarAsync(int id, CrearCategoriaDto dto)
    {
        var cat = await db.Categorias.FindAsync(id);
        if (cat is null) return null;

        cat.Nombre      = dto.Nombre;
        cat.Descripcion = dto.Descripcion;
        await db.SaveChangesAsync();

        return new CategoriaDto { Id = cat.Id, Nombre = cat.Nombre, Descripcion = cat.Descripcion, Activo = cat.Activo };
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var cat = await db.Categorias.FindAsync(id);
        if (cat is null) return false;

        var tieneProductos = await db.Productos.AnyAsync(p => p.CategoriaId == id);
        if (tieneProductos) throw new InvalidOperationException("No se puede eliminar: la categoría tiene productos asociados.");

        db.Categorias.Remove(cat);
        await db.SaveChangesAsync();
        return true;
    }
}
