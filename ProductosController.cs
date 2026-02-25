using InventarioAPI.DTOs;
using InventarioAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventarioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProductosController(IProductoService productoService) : ControllerBase
{
    /// <summary>Lista productos con paginación y filtros opcionales</summary>
    [HttpGet]
    [Authorize(Policy = "UserOrAdmin")]
    [ProducesResponseType(typeof(ApiResponseDto<PaginadoDto<ProductoDto>>), 200)]
    public async Task<IActionResult> GetTodos([FromQuery] FiltroProductoDto filtro)
    {
        var resultado = await productoService.ObtenerTodosAsync(filtro);
        return Ok(ApiResponseDto<PaginadoDto<ProductoDto>>.Ok(resultado));
    }

    /// <summary>Obtiene un producto por ID</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "UserOrAdmin")]
    [ProducesResponseType(typeof(ApiResponseDto<ProductoDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPorId(int id)
    {
        var producto = await productoService.ObtenerPorIdAsync(id);
        if (producto is null) return NotFound(ApiResponseDto<string>.Error($"Producto {id} no encontrado"));
        return Ok(ApiResponseDto<ProductoDto>.Ok(producto));
    }

    /// <summary>Crea un nuevo producto [Solo Admin]</summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponseDto<ProductoDto>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Crear([FromBody] CrearProductoDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (dto.SKU is not null && await productoService.ExisteSKUAsync(dto.SKU))
            return BadRequest(ApiResponseDto<string>.Error($"El SKU '{dto.SKU}' ya existe"));

        var creado = await productoService.CrearAsync(dto);
        return CreatedAtAction(nameof(GetPorId), new { id = creado.Id },
            ApiResponseDto<ProductoDto>.Ok(creado, "Producto creado correctamente"));
    }

    /// <summary>Actualiza un producto existente [Solo Admin]</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponseDto<ProductoDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProductoDto dto)
    {
        if (dto.SKU is not null && await productoService.ExisteSKUAsync(dto.SKU, id))
            return BadRequest(ApiResponseDto<string>.Error($"El SKU '{dto.SKU}' ya existe en otro producto"));

        var actualizado = await productoService.ActualizarAsync(id, dto);
        if (actualizado is null) return NotFound(ApiResponseDto<string>.Error($"Producto {id} no encontrado"));

        return Ok(ApiResponseDto<ProductoDto>.Ok(actualizado, "Producto actualizado correctamente"));
    }

    /// <summary>Elimina un producto [Solo Admin]</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await productoService.EliminarAsync(id);
        if (!eliminado) return NotFound(ApiResponseDto<string>.Error($"Producto {id} no encontrado"));
        return NoContent();
    }

    /// <summary>Ajusta el stock de un producto (positivo=entrada, negativo=salida) [Solo Admin]</summary>
    [HttpPatch("{id:int}/stock")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponseDto<ProductoDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AjustarStock(int id, [FromBody] AjusteStockDto dto)
    {
        var actualizado = await productoService.AjustarStockAsync(id, dto);
        if (actualizado is null) return NotFound(ApiResponseDto<string>.Error($"Producto {id} no encontrado"));
        return Ok(ApiResponseDto<ProductoDto>.Ok(actualizado, $"Stock ajustado en {dto.Cantidad} unidades. Motivo: {dto.Motivo}"));
    }
}
