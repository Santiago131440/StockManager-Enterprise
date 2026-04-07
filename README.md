# StockManager-Enterprise

API REST completa para gestión de catálogo de productos e inventario, construida con **ASP.NET Core 8** y **Entity Framework Core**, con autenticación **JWT**, base de datos **SQLite** y documentación interactiva con **Swagger**.

---

## Tecnologías

| Tecnología | Versión |
|---|---|
| .NET / ASP.NET Core | 8.0 |
| Entity Framework Core | 8.0 |
| SQLite | vía EF Core |
| JWT Bearer Authentication | 8.0 |
| BCrypt.Net-Next | 4.0.3 |
| Swagger / Swashbuckle | 6.5.0 |

---

## Estructura del Proyecto

```
InventarioAPI/
├── Controllers/
│   ├── AuthController.cs        # Login y registro
│   ├── ProductosController.cs   # CRUD de productos
│   └── CategoriasController.cs  # CRUD de categorías
├── Data/
│   ├── AppDbContext.cs           # Contexto de Entity Framework
│   └── DbSeeder.cs              # Datos iniciales de ejemplo
├── DTOs/
│   ├── ProductoDtos.cs          # DTOs de producto
│   └── OtrosDtos.cs             # DTOs de auth, categoría y respuesta genérica
├── Helpers/
│   └── JwtHelper.cs             # Generación de tokens JWT
├── Middleware/
│   └── ErrorHandlingMiddleware.cs # Manejo global de errores
├── Models/
│   ├── Producto.cs
│   ├── Categoria.cs
│   └── Usuario.cs
├── Services/
│   ├── IProductoService.cs / ProductoService.cs
│   ├── ICategoriaService.cs / CategoriaService.cs  (en CategoriaService.cs)
│   └── IAuthService.cs / AuthService.cs             (en AuthService.cs)
├── appsettings.json
└── Program.cs
```

---

## Instalación y Ejecución

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Pasos

```bash
# 1. Clonar el repositorio
cd inventario-api/InventarioAPI

# 2. Restaurar dependencias
dotnet restore

# 3. Ejecutar la aplicación
dotnet run
```

Al iniciar, la app:
- Crea automáticamente la base de datos SQLite (`inventario.db`).
- Inserta datos de ejemplo (categorías, productos y 2 usuarios).

### Acceder a Swagger UI

```
http://localhost:5000
```

---

## Autenticación JWT

La API usa **JWT Bearer Tokens**. Para obtener un token:

### Credenciales de prueba

| Usuario | Email | Contraseña | Rol |
|---|---|---|---|
| admin | admin@inventario.com | Admin123! | Admin |
| usuario | usuario@inventario.com | User123! | User |

### Flujo

```
POST /api/auth/login
{
  "email": "admin@inventario.com",
  "password": "Admin123!"
}
```

Respuesta:
```json
{
  "exito": true,
  "datos": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "nombreUsuario": "admin",
    "rol": "Admin",
    "expiracion": "2024-01-01T08:00:00Z"
  }
}
```

Usar el token en cada petición:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

---

## Endpoints

### Auth

| Método | Ruta | Descripción | Auth |
|---|---|---|---|
| POST | `/api/auth/login` | Iniciar sesión |
| POST | `/api/auth/registro` | Registrar usuario |

### Productos

| Método | Ruta | Descripción | Rol mínimo |
|---|---|---|---|
| GET | `/api/productos` | Listar con filtros y paginación | User |
| GET | `/api/productos/{id}` | Obtener por ID | User |
| POST | `/api/productos` | Crear producto | Admin |
| PUT | `/api/productos/{id}` | Actualizar producto | Admin |
| DELETE | `/api/productos/{id}` | Eliminar producto | Admin |
| PATCH | `/api/productos/{id}/stock` | Ajustar stock | Admin |

#### Parámetros de filtro para GET `/api/productos`

| Parámetro | Tipo | Descripción |
|---|---|---|
| `busqueda` | string | Filtra por nombre, descripción o SKU |
| `categoriaId` | int | Filtra por categoría |
| `activo` | bool | Filtra activos/inactivos |
| `precioMin` | decimal | Precio mínimo |
| `precioMax` | decimal | Precio máximo |
| `pagina` | int | Número de página (default: 1) |
| `tamanoPagina` | int | Registros por página (default: 10) |

### Categorías

| Método | Ruta | Descripción | Rol mínimo |
|---|---|---|---|
| GET | `/api/categorias` | Listar todas | User |
| GET | `/api/categorias/{id}` | Obtener por ID | User |
| POST | `/api/categorias` | Crear categoría | Admin |
| PUT | `/api/categorias/{id}` | Actualizar categoría | Admin |
| DELETE | `/api/categorias/{id}` | Eliminar categoría | Admin |

---

## Ejemplos de Peticiones

### Crear producto
```json
POST /api/productos
Authorization: Bearer {token_admin}

{
  "nombre": "Monitor 27\" 4K",
  "descripcion": "Monitor UHD con panel IPS",
  "precio": 4999.99,
  "stock": 15,
  "sku": "ELEC-010",
  "categoriaId": 1
}
```

### Ajustar stock (entrada de mercancía)
```json
PATCH /api/productos/1/stock
Authorization: Bearer {token_admin}

{
  "cantidad": 50,
  "motivo": "Entrada de almacén - orden #1234"
}
```

### Ajustar stock (salida / venta)
```json
PATCH /api/productos/1/stock

{
  "cantidad": -5,
  "motivo": "Venta a cliente"
}
```

---

## Configuración

Edita `appsettings.json` para personalizar:

```json
{
  "JwtSettings": {
    "SecretKey": "TU_CLAVE_SECRETA_AQUI",
    "Issuer": "InventarioAPI",
    "Audience": "InventarioCliente",
    "ExpirationHours": 8
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=inventario.db"
  }
}
```

> **En producción**: usa variables de entorno o Azure Key Vault para la `SecretKey`. Nunca la subas al repositorio.

---

## Arquitectura

```
HTTP Request
    │
    ▼
[Middleware: JWT Auth + ErrorHandling]
    │
    ▼
[Controller]  ──► valida DTOs
    │
    ▼
[Service]     ──► lógica de negocio
    │
    ▼
[DbContext]   ──► EF Core → SQLite
```

---

## Pruebas Rápidas con cURL

```bash
# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@inventario.com","password":"Admin123!"}'

# Listar productos (reemplaza TOKEN)
curl http://localhost:5000/api/productos \
  -H "Authorization: Bearer TOKEN"

# Buscar productos de electrónica
curl "http://localhost:5000/api/productos?categoriaId=1&pagina=1&tamanoPagina=5" \
  -H "Authorization: Bearer TOKEN"
```

---

## Licencia

MIT License — libre para uso personal y comercial.

---

## Contribuciones

1. Haz fork del repositorio
2. Crea una rama: `git checkout -b feature/nueva-funcionalidad`
3. Haz commit: `git commit -m 'Agrega nueva funcionalidad'`
4. Push: `git push origin feature/nueva-funcionalidad`
5. Abre un Pull Request
