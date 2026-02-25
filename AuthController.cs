using InventarioAPI.DTOs;
using InventarioAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Inicia sesión y obtiene un token JWT</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var resultado = await authService.LoginAsync(dto);

        if (resultado is null)
            return Unauthorized(ApiResponseDto<string>.Error("Credenciales incorrectas"));

        return Ok(ApiResponseDto<AuthResponseDto>.Ok(resultado, "Sesión iniciada correctamente"));
    }

    /// <summary>Registra un nuevo usuario (rol User)</summary>
    [HttpPost("registro")]
    [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await authService.RegistrarAsync(dto);
        return CreatedAtAction(nameof(Login), ApiResponseDto<AuthResponseDto>.Ok(resultado, "Usuario registrado correctamente"));
    }
}
