# Configuración para Subida de Imágenes en Producción

## 🚨 Problema Identificado

**Síntoma:** En producción, al subir 6 imágenes de 6 MB cada una (36 MB total), aparece el error: "Error al enviar el formulario"

**Causa:** Los límites de tamaño predeterminados del servidor web (IIS/Kestrel/Azure) son más restrictivos que el entorno de desarrollo local.

## ✅ Solución Implementada

### 1. Configuración de Kestrel (Program.cs)

Se agregaron límites de 150 MB para el servidor Kestrel:

```csharp
// Configure Kestrel server limits for file uploads (150 MB)
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 150_000_000; // 150 MB
});

// Configure Form options for multipart form data
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 150_000_000; // 150 MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});
```

### 2. Configuración de IIS (web.config)

Se creó un archivo `web.config` en la raíz del proyecto con:

```xml
<security>
  <requestFiltering>
    <!-- 150 MB = 157286400 bytes -->
    <requestLimits maxAllowedContentLength="157286400" />
  </requestFiltering>
</security>

<httpRuntime maxRequestLength="153600" executionTimeout="120" />
```

### 3. Mensajes de Error Mejorados (JavaScript)

Se agregó detección específica de errores HTTP:
- **413:** Imágenes demasiado grandes
- **500:** Error del servidor
- **400:** Error en datos enviados
- **404:** Página no encontrada

## 📋 Checklist de Despliegue

### Antes de Publicar

- [x] ✅ Verificar que `web.config` está en la raíz del proyecto
- [x] ✅ Verificar cambios en `Program.cs`
- [x] ✅ Verificar cambios en `anunciar-auto.js`
- [x] ✅ Compilar sin errores

### Al Publicar

1. **Compilar en modo Release:**
   ```bash
   dotnet publish -c Release
   ```

2. **Verificar que `web.config` se copió a la carpeta de publicación:**
   - Debe estar en: `bin/Release/net9.0/publish/web.config`

3. **Subir todos los archivos al servidor**

### Después de Publicar

1. **Si usa Azure App Service:**
   - Ir a **Configuración → Configuración general**
   - Verificar que **HTTP 2.0** está habilitado
   - Verificar **Plataforma**: 64 bits
   - Si el problema persiste, agregar en **Configuración de la aplicación**:
     ```
     Nombre: WEBSITE_LOAD_USER_PROFILE
     Valor: 1
     ```

2. **Si usa IIS en servidor propio:**
   - Abrir **Administrador de IIS**
   - Seleccionar el sitio web
   - **Filtrado de solicitudes → Editar configuración de características**
   - Verificar:
     - ✅ Permitir solicitudes de URL dobles: `true`
     - ✅ Longitud máxima de contenido (bytes): `157286400`
   - **Reiniciar el sitio web**

3. **Probar la funcionalidad:**
   - Subir 6 imágenes de 6 MB cada una
   - Verificar que se suben correctamente
   - Verificar mensaje de error descriptivo si excede límites

## 🔍 Diagnóstico de Problemas

### Si el error persiste en producción:

1. **Verificar logs del servidor:**
   - Azure: Portal → App Service → Registros → Log stream
   - IIS: `C:\inetpub\logs\LogFiles\`

2. **Verificar en el navegador (F12):**
   - **Network tab** → Ver el request fallido
   - Buscar código de respuesta HTTP (413, 500, etc.)
   - Ver tamaño del payload en **Request headers**

3. **Verificar límites específicos de Azure:**
   - Portal de Azure → App Service → Configuración avanzada
   - Puede haber límites adicionales por plan (Basic, Standard, Premium)

### Códigos de Error Comunes

| Código | Significado | Solución |
|--------|------------|----------|
| 413 | Payload Too Large | Verificar web.config y Program.cs |
| 408 | Request Timeout | Aumentar timeout en web.config |
| 500 | Server Error | Ver logs del servidor para detalles |
| 502 | Bad Gateway | Problema de proxy/load balancer |

## 📊 Límites Configurados

| Componente | Límite | Ubicación |
|------------|--------|-----------|
| Por imagen | 10 MB | JavaScript (cliente) |
| Total imágenes | 100 MB | JavaScript (cliente) |
| Request body (C#) | 150 MB | AnunciarMiAuto.cshtml.cs |
| Kestrel | 150 MB | Program.cs |
| IIS MaxAllowedContentLength | 150 MB | web.config |
| IIS maxRequestLength | 150 MB | web.config |
| Timeout | 120 seg | JavaScript + web.config |

## ⚠️ Notas Importantes

1. **web.config es crítico:** Si no se incluye en la publicación, IIS usará sus valores predeterminados (~30 MB).

2. **Cache del navegador:** Después de desplegar, los usuarios pueden necesitar hacer **Ctrl+F5** para recargar el JavaScript actualizado.

3. **Azure App Service:** Los planes básicos pueden tener límites adicionales. Considerar plan Standard o superior si persisten problemas.

4. **Monitoreo:** Después del despliegue, monitorear logs por 24-48 horas para identificar otros problemas.

## 🎯 Pruebas Recomendadas

### Prueba 1: Subida Normal
- Subir 6 imágenes de 6 MB cada una (36 MB total)
- ✅ Debe funcionar correctamente

### Prueba 2: Exceder Límite Individual
- Intentar subir 1 imagen de 15 MB
- ✅ Debe mostrar: "Las siguientes imágenes exceden el tamaño máximo de 10 MB"

### Prueba 3: Exceder Límite Total
- Intentar subir 15 imágenes de 10 MB cada una (150 MB)
- ✅ Debe mostrar: "El tamaño total de todas las imágenes (150 MB) excede el límite de 100 MB"

### Prueba 4: Timeout
- Simular conexión lenta (Chrome DevTools → Network → Slow 3G)
- Subir varias imágenes grandes
- ✅ Debe mostrar mensaje de timeout después de 2 minutos

## 📞 Soporte

Si después de seguir todos estos pasos el problema persiste:

1. Capturar screenshot del error en navegador (con F12 Network tab abierto)
2. Revisar logs del servidor
3. Documentar:
   - Tamaño exacto de las imágenes
   - Código de respuesta HTTP
   - Tiempo que tarda antes de fallar
   - Proveedor de hosting (Azure, IIS local, etc.)

---

**Última actualización:** 17 de enero de 2026  
**Versión:** 2.0 (Configuración de producción)
