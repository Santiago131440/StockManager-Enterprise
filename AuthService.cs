using InventarioAPI.Data;
using InventarioAPI.DTOs;
using InventarioAPI.Helpers;
using InventarioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<AuthResponseDto>  RegistrarAsync(RegistroDto dto);
}

public class AuthService(AppDbContext db, JwtHelper jwt) : IAuthService
{
    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Activo);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return null;

        var (token, exp) = jwt.GenerarToken(usuario);

        return new AuthResponseDto
        {
            Token          = token,
            NombreUsuario  = usuario.NombreUsuario,
            Email          = usuario.Email,
            Rol            = usuario.Rol,
            Expiracion     = exp
        };
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegistroDto dto)
    {
        var existe = await db.Usuarios.AnyAsync(u => u.Email == dto.Email);
        if (existe) throw new InvalidOperationException("El email ya está registrado.");

        var usuario = new Usuario
        {
            NombreUsuario = dto.NombreUsuario,
            Email         = dto.Email,
            PasswordHash  = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol           = "User"
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var (token, exp) = jwt.GenerarToken(usuario);

        return new AuthResponseDto
        {
            Token         = token,
            NombreUsuario = usuario.NombreUsuario,
            Email         = usuario.Email,
            Rol           = usuario.Rol,
            Expiracion    = exp
        };
    }
}
