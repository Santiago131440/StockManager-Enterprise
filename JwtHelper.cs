using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventarioAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace InventarioAPI.Helpers;

public class JwtHelper(IConfiguration config)
{
    public (string Token, DateTime Expiracion) GenerarToken(Usuario usuario)
    {
        var jwtSettings  = config.GetSection("JwtSettings");
        var secretKey    = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey no configurado");
        var horas        = int.TryParse(jwtSettings["ExpirationHours"], out var h) ? h : 8;
        var expiracion   = DateTime.UtcNow.AddHours(horas);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name,               usuario.NombreUsuario),
            new Claim(ClaimTypes.Role,               usuario.Rol)
        };

        var clave       = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             jwtSettings["Issuer"],
            audience:           jwtSettings["Audience"],
            claims:             claims,
            expires:            expiracion,
            signingCredentials: credenciales
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiracion);
    }
}
