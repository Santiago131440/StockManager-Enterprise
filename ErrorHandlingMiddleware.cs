using System.Net;
using System.Text.Json;

namespace InventarioAPI.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error no controlado: {Mensaje}", ex.Message);
            await ManejarExcepcionAsync(context, ex);
        }
    }

    private static async Task ManejarExcepcionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, mensaje) = ex switch
        {
            InvalidOperationException  => (HttpStatusCode.BadRequest,          ex.Message),
            KeyNotFoundException       => (HttpStatusCode.NotFound,            ex.Message),
            UnauthorizedAccessException=> (HttpStatusCode.Unauthorized,        "No autorizado"),
            _                          => (HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        context.Response.StatusCode = (int)statusCode;

        var respuesta = new
        {
            exito   = false,
            mensaje,
            detalle = ex is InvalidOperationException or KeyNotFoundException ? null : ex.Message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(respuesta, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
        );
    }
}
