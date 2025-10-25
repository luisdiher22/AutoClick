# Implementación del Sistema de Favoritos - AutoClick

## Archivos Creados

### 1. Modelo de Favorito
- **Ruta**: `Models/Favorito.cs`
- **Descripción**: Modelo de datos para la tabla de Favoritos

### 2. Controlador API
- **Ruta**: `Controllers/FavoritosController.cs`
- **Endpoints**:
  - `POST /api/Favoritos/Toggle` - Agregar/quitar favorito
  - `GET /api/Favoritos/Check/{autoId}` - Verificar si un auto es favorito
  - `GET /api/Favoritos/GetUserFavorites` - Obtener lista de IDs de favoritos del usuario
  - `GET /api/Favoritos/GetFavoriteAutos` - Obtener autos favoritos completos

### 3. JavaScript
- **Ruta**: `wwwroot/js/favoritos.js`
- **Funciones**:
  - `initializeFavoritos(emailUsuario)` - Inicializar sistema
  - `toggleFavorito(autoId, emailUsuario)` - Toggle favorito
  - `updateAllFavoriteButtons()` - Actualizar UI

### 4. CSS
- **Ruta**: `wwwroot/css/favoritos.css`
- **Estilos para botón de favoritos y overlays**

### 5. Migración de Base de Datos
- **Tabla**: `Favoritos`
- **Columnas**:
  - `Id` (PK)
  - `EmailUsuario` (FK a Usuarios)
  - `AutoId` (FK a Autos)
  - `FechaCreacion`
- **Índice único**: `EmailUsuario + AutoId` para evitar duplicados

## Pasos para Completar la Implementación

### Paso 1: Detener la aplicación y crear la migración

```powershell
# Detener la aplicación en ejecución
# Luego ejecutar:
cd C:\Users\luisd\Documents\AutoClick\AutoClick
dotnet ef migrations add AddFavoritosTable
dotnet ef database update
```

### Paso 2: Agregar referencias en _Layout.cshtml

Agregar antes del cierre de `</head>`:

```html
<link rel="stylesheet" href="~/css/favoritos.css" asp-append-version="true" />
```

Agregar antes del cierre de `</body>`:

```html
<script src="~/js/favoritos.js" asp-append-version="true"></script>
```

Y agregar el email del usuario en el body tag:

```csharp
<body data-user-email="@(User.Identity?.IsAuthenticated == true ? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value : "")">
```

### Paso 3: Modificar los Tiles de Auto

En cada lugar donde se renderizan tiles de auto (Index.cshtml, BusquedaAvanzada.cshtml, Destacados.cshtml, Explorar.cshtml, Guardados.cshtml), agregar el botón de favoritos dentro del contenedor de la imagen:

```html
<div style="width: 100%; height: 260px; position: relative; overflow: hidden; border-radius: 4px;">
    <img style="width: 100%; height: 100%; object-fit: cover;" 
         src="@(string.IsNullOrEmpty(auto.ImagenPrincipal) ? "https://placehold.co/392x200" : auto.ImagenPrincipal)" 
         alt="@auto.NombreCompleto" />
    
    <!-- BOTÓN DE FAVORITOS - AGREGAR ESTO -->
    <button class="favorite-btn" 
            data-auto-id="@auto.Id" 
            data-email-usuario="@(User.Identity?.IsAuthenticated == true ? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value : "")"
            title="Agregar a favoritos"
            type="button">
        <img src="https://autoclickstorage.blob.core.windows.net/uploads/Component%20183.png" 
             alt="Favorito" />
    </button>
    
    @if (!string.IsNullOrEmpty(auto.BanderinVideoUrl) && Model.BanderinUrls.ContainsKey(auto.Id))
    {
        <!-- Banderín existente -->
    }
</div>
```

### Paso 4: Actualizar la página de Guardados

Modificar `Pages/Guardados.cshtml.cs` para cargar los autos favoritos:

```csharp
public async Task OnGetAsync()
{
    if (User.Identity?.IsAuthenticated == true)
    {
        var emailUsuario = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value;
        
        if (!string.IsNullOrEmpty(emailUsuario))
        {
            // Obtener los IDs de los autos favoritos
            var favoritosIds = await _context.Favoritos
                .Where(f => f.EmailUsuario == emailUsuario)
                .Select(f => f.AutoId)
                .ToListAsync();
            
            // Cargar los autos favoritos
            AutosGuardados = await _context.Autos
                .Where(a => favoritosIds.Contains(a.Id) && a.Activo)
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }
    }
}
```

### Paso 5: Actualizar la sección de "Autos Guardados" en Index

Modificar `Pages/Index.cshtml.cs` para cargar los autos favoritos del usuario:

```csharp
// En OnGetAsync(), agregar:
if (User.Identity?.IsAuthenticated == true)
{
    var emailUsuario = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value;
    
    if (!string.IsNullOrEmpty(emailUsuario))
    {
        var favoritosIds = await _context.Favoritos
            .Where(f => f.EmailUsuario == emailUsuario)
            .Select(f => f.AutoId)
            .ToListAsync();
        
        AutosGuardados = await _context.Autos
            .Where(a => favoritosIds.Contains(a.Id) && a.Activo)
            .OrderByDescending(a => a.FechaCreacion)
            .Take(3)
            .ToListAsync();
    }
}
else
{
    // Si no está logueado, mostrar mensaje o autos vacíos
    AutosGuardados = new List<Auto>();
}
```

## Archivos a Modificar

1. **AutoClick/Pages/Shared/_Layout.cshtml** - Agregar referencias CSS/JS y data attribute
2. **AutoClick/Pages/Index.cshtml** - Agregar botón de favoritos a los tiles
3. **AutoClick/Pages/Index.cshtml.cs** - Cargar autos favoritos
4. **AutoClick/Pages/BusquedaAvanzada.cshtml** - Agregar botón de favoritos
5. **AutoClick/Pages/Destacados.cshtml** - Agregar botón de favoritos
6. **AutoClick/Pages/Explorar.cshtml** - Agregar botón de favoritos
7. **AutoClick/Pages/Guardados.cshtml** - Agregar botón de favoritos
8. **AutoClick/Pages/Guardados.cshtml.cs** - Cargar solo autos favoritos

## Iconos Utilizados

- **Vacío (Component 183.png)**: https://autoclickstorage.blob.core.windows.net/uploads/Component%20183.png
- **Lleno (CORAZON.png)**: https://autoclickstorage.blob.core.windows.net/uploads/CORAZON.png

## Funcionamiento

1. El usuario hace clic en el icono de corazón en cualquier tile de auto
2. Si no está logueado, se puede mostrar un mensaje (opcional)
3. Si está logueado, se hace una llamada al API para toggle el favorito
4. El icono cambia de vacío a lleno (o viceversa) con animación
5. Los cambios se reflejan en todas las instancias del mismo auto en la página
6. La vista de Guardados muestra solo los autos favoritos del usuario
7. La sección "Autos Guardados" en Index muestra los 3 autos más recientes que el usuario ha marcado como favoritos

## Próximos Pasos

1. Detener la aplicación
2. Crear y aplicar la migración
3. Modificar _Layout.cshtml
4. Modificar todos los archivos de vistas con tiles de autos
5. Probar la funcionalidad
6. Reiniciar la aplicación

## Notas Adicionales

- El sistema utiliza un cache local en JavaScript para minimizar llamadas al servidor
- Los favoritos se sincronizan automáticamente entre diferentes páginas
- El botón tiene z-index: 10 para estar encima de la imagen pero debajo del banderín
- El diseño es responsive y funciona en móviles
