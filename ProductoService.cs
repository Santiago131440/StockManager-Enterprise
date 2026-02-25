using InventarioAPI.Data;
using InventarioAPI.DTOs;
using InventarioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Services;

public class ProductoService(AppDbContext db) : IProductoService
{
    public async Task<PaginadoDto<ProductoDto>> ObtenerTodosAsync(FiltroProductoDto filtro)
    {
        var query = db.Productos.Include(p => p.Categoria).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
        {
            var b = filtro.Busqueda.ToLower();
            query = query.Where(p =>
                p.Nombre.ToLower().Contains(b) ||
                (p.Descripcion != null && p.Descripcion.ToLower().Contains(b)) ||
                (p.SKU         != null && p.SKU.ToLower().Contains(b)));
        }

        if (filtro.CategoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == filtro.CategoriaId);

        if (filtro.Activo.HasValue)
            query = query.Where(p => p.Activo == filtro.Activo);

        if (filtro.PrecioMin.HasValue)
            query = query.Where(p => p.Precio >= filtro.PrecioMin);

        if (filtro.PrecioMax.HasValue)
            query = query.Where(p => p.Precio <= filtro.PrecioMax);

        var total = await query.CountAsync();

        var datos = await query
            .OrderBy(p => p.Nombre)
            .Skip((filtro.Pagina - 1) * filtro.TamanoPagina)
            .Take(filtro.TamanoPagina)
            .Select(p => MapearDto(p))
            .ToListAsync();

        return new PaginadoDto<ProductoDto>
        {
            Datos        = datos,
            Total        = total,
            Pagina       = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtro.TamanoPagina)
        };
    }

    public async Task<ProductoDto?> ObtenerPorIdAsync(int id)
    {
        var p = await db.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
        return p is null ? null : MapearDto(p);
    }

    public async Task<ProductoDto> CrearAsync(CrearProductoDto dto)
    {
        var producto = new Producto
        {
            Nombre      = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio      = dto.Precio,
            Stock       = dto.Stock,
            SKU         = dto.SKU,
            CategoriaId = dto.CategoriaId
        };

        db.Productos.Add(producto);
        await db.SaveChangesAsync();
        await db.Entry(producto).Reference(p => p.Categoria).LoadAsync();

        return MapearDto(producto);
    }

    public async Task<ProductoDto?> ActualizarAsync(int id, ActualizarProductoDto dto)
    {
        var producto = await db.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
        if (producto is null) return null;

        if (dto.Nombre      is not null) producto.Nombre      = dto.Nombre;
        if (dto.Descripcion is not null) producto.Descripcion = dto.Descripcion;
        if (dto.Precio      is not null) producto.Precio      = dto.Precio.Value;
        if (dto.Stock       is not null) producto.Stock       = dto.Stock.Value;
        if (dto.SKU         is not null) producto.SKU         = dto.SKU;
        if (dto.Activo      is not null) producto.Activo      = dto.Activo.Value;
        if (dto.CategoriaId is not null) producto.CategoriaId = dto.CategoriaId.Value;

        producto.FechaActualizacion = DateTime.UtcNow;

        await db.SaveChangesAsync();
        await db.Entry(producto).Reference(p => p.Categoria).LoadAsync();

        return MapearDto(producto);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var producto = await db.Productos.FindAsync(id);
        if (producto is null) return false;

        db.Productos.Remove(producto);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ProductoDto?> AjustarStockAsync(int id, AjusteStockDto dto)
    {
        var producto = await db.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
        if (producto is null) return null;

        var nuevoStock = producto.Stock + dto.Cantidad;
        if (nuevoStock < 0) throw new InvalidOperationException("El stock no puede quedar negativo.");

        producto.Stock              = nuevoStock;
        producto.FechaActualizacion = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return MapearDto(producto);
    }

    public async Task<bool> ExisteSKUAsync(string sku, int? excluirId = null)
    {
        var query = db.Productos.Where(p => p.SKU == sku);
        if (excluirId.HasValue) query = query.Where(p => p.Id != excluirId);
        return await query.AnyAsync();
    }

    // ─── Mapper ────────────────────────────────────────────────────────────

    private static ProductoDto MapearDto(Producto p) => new()
    {
        Id               = p.Id,
        Nombre           = p.Nombre,
        Descripcion      = p.Descripcion,
        Precio           = p.Precio,
        Stock            = p.Stock,
        SKU              = p.SKU,
        Activo           = p.Activo,
        FechaCreacion    = p.FechaCreacion,
        CategoriaId      = p.CategoriaId,
        CategoriaNombre  = p.Categoria?.Nombre ?? string.Empty
    };
}
