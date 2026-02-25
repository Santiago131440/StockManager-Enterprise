using InventarioAPI.DTOs;

namespace InventarioAPI.Services;

public interface IProductoService
{
    Task<PaginadoDto<ProductoDto>> ObtenerTodosAsync(FiltroProductoDto filtro);
    Task<ProductoDto?>             ObtenerPorIdAsync(int id);
    Task<ProductoDto>              CrearAsync(CrearProductoDto dto);
    Task<ProductoDto?>             ActualizarAsync(int id, ActualizarProductoDto dto);
    Task<bool>                     EliminarAsync(int id);
    Task<ProductoDto?>             AjustarStockAsync(int id, AjusteStockDto dto);
    Task<bool>                     ExisteSKUAsync(string sku, int? excluirId = null);
}
