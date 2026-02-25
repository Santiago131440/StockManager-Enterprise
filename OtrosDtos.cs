using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.DTOs;

// ════════════════════════════════════════
//  AUTH
// ════════════════════════════════════════

public class LoginDto
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    public string Password { get; set; } = string.Empty;
}

public class RegistroDto
{
    [Required]
    [MaxLength(80)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmarPassword { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token          { get; set; } = string.Empty;
    public string NombreUsuario  { get; set; } = string.Empty;
    public string Email          { get; set; } = string.Empty;
    public string Rol            { get; set; } = string.Empty;
    public DateTime Expiracion   { get; set; }
}

// ════════════════════════════════════════
//  CATEGORIA
// ════════════════════════════════════════

public class CategoriaDto
{
    public int    Id          { get; set; }
    public string Nombre      { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool   Activo      { get; set; }
    public int    TotalProductos { get; set; }
}

public class CrearCategoriaDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Descripcion { get; set; }
}

// ════════════════════════════════════════
//  RESPUESTA PAGINADA
// ════════════════════════════════════════

public class PaginadoDto<T>
{
    public IEnumerable<T> Datos      { get; set; } = Enumerable.Empty<T>();
    public int Total                 { get; set; }
    public int Pagina                { get; set; }
    public int TamanoPagina          { get; set; }
    public int TotalPaginas          { get; set; }
}

// ════════════════════════════════════════
//  RESPUESTA GENÉRICA
// ════════════════════════════════════════

public class ApiResponseDto<T>
{
    public bool   Exito    { get; set; }
    public string Mensaje  { get; set; } = string.Empty;
    public T?     Datos    { get; set; }

    public static ApiResponseDto<T> Ok(T datos, string mensaje = "Operación exitosa") =>
        new() { Exito = true, Mensaje = mensaje, Datos = datos };

    public static ApiResponseDto<T> Error(string mensaje) =>
        new() { Exito = false, Mensaje = mensaje };
}
