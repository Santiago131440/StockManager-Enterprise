using InventarioAPI.DTOs;
using InventarioAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventarioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class CategoriasController(ICategoriaService catService) : ControllerBase
{
    /// <summary>Lista todas las categorías activas</summary>
    [HttpGet]
    [Authorize(Policy = "UserOrAdmin")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<CategoriaDto>>), 200)]
    public async Task<IActionResult> GetTodas()
    {
        var lista = await catService.ObtenerTodasAsync();
        return Ok(ApiResponseDto<IEnumerable<CategoriaDto>>.Ok(lista));
    }

    /// <summary>Obtiene una categoría por ID</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "UserOrAdmin")]
    [ProducesResponseType(typeof(ApiResponseDto<CategoriaDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPorId(int id)
    {
        var cat = await catService.ObtenerPorIdAsync(id);
        if (cat is null) return NotFound(ApiResponseDto<string>.Error($"Categoría {id} no encontrada"));
        return Ok(ApiResponseDto<CategoriaDto>.Ok(cat));
    }

    /// <summary>Crea una nueva categoría [Solo Admin]</summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponseDto<CategoriaDto>), 201)]
    public async Task<IActionResult> Crear([FromBody] CrearCategoriaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var creada = await catService.CrearAsync(dto);
        return CreatedAtAction(nameof(GetPorId), new { id = creada.Id },
            ApiResponseDto<CategoriaDto>.Ok(creada, "Categoría creada correctamente"));
    }

    /// <summary>Actualiza una categoría [Solo Admin]</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponseDto<CategoriaDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CrearCategoriaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var actualizada = await catService.ActualizarAsync(id, dto);
        if (actualizada is null) return NotFound(ApiResponseDto<string>.Error($"Categoría {id} no encontrada"));
        return Ok(ApiResponseDto<CategoriaDto>.Ok(actualizada, "Categoría actualizada correctamente"));
    }

    /// <summary>Elimina una categoría (sólo si no tiene productos) [Solo Admin]</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminada = await catService.EliminarAsync(id);
        if (!eliminada) return NotFound(ApiResponseDto<string>.Error($"Categoría {id} no encontrada"));
        return NoContent();
    }
}
