using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.DTOs;

// ─── Lectura ────────────────────────────────────────────────────────────────

public class ProductoDto
{
    public int    Id               { get; set; }
    public string Nombre          { get; set; } = string.Empty;
    public string? Descripcion    { get; set; }
    public decimal Precio         { get; set; }
    public int    Stock           { get; set; }
    public string? SKU            { get; set; }
    public bool   Activo          { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int    CategoriaId     { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
}

// ─── Creación ───────────────────────────────────────────────────────────────

public class CrearProductoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(150, ErrorMessage = "Máximo 150 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [Required]
    [Range(0.01, 9999999, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    [MaxLength(50)]
    public string? SKU { get; set; }

    [Required(ErrorMessage = "Debe indicar la categoría")]
    public int CategoriaId { get; set; }
}

// ─── Actualización ──────────────────────────────────────────────────────────

public class ActualizarProductoDto
{
    [MaxLength(150)]
    public string? Nombre       { get; set; }

    [MaxLength(500)]
    public string? Descripcion  { get; set; }

    [Range(0.01, 9999999)]
    public decimal? Precio      { get; set; }

    [Range(0, int.MaxValue)]
    public int? Stock           { get; set; }

    [MaxLength(50)]
    public string? SKU          { get; set; }

    public bool? Activo         { get; set; }

    public int? CategoriaId     { get; set; }
}

// ─── Ajuste de Stock ────────────────────────────────────────────────────────

public class AjusteStockDto
{
    [Required]
    public int Cantidad { get; set; }  // positivo = entrada, negativo = salida

    [MaxLength(200)]
    public string? Motivo { get; set; }
}

// ─── Filtro / Búsqueda ──────────────────────────────────────────────────────

public class FiltroProductoDto
{
    public string? Busqueda   { get; set; }
    public int?    CategoriaId { get; set; }
    public bool?   Activo      { get; set; }
    public decimal? PrecioMin  { get; set; }
    public decimal? PrecioMax  { get; set; }
    public int     Pagina      { get; set; } = 1;
    public int     TamanoPagina { get; set; } = 10;
}
