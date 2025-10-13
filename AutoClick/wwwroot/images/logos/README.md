# Logos de AutoClick.cr

Esta carpeta contiene los logotipos oficiales de AutoClick.cr en diferentes resoluciones y formatos.

## 📁 Estructura de Archivos:

### `autoclick-logo-hd.png` ✅ **ACTIVO**
- **Uso:** Logo principal de alta resolución para el header del sitio web
- **Formato:** PNG con transparencia
- **Ubicación en código:** Usado en `_Header.cshtml`, `Explorar.cshtml`, `Guardados.cshtml`
- **Actualmente:** Copia de `LOGOS AUTOCLICK-04.png` ✅

### `LOGOS AUTOCLICK/` - Colección Completa:
1. **LOGOS AUTOCLICK-01.png** 
2. **LOGOS AUTOCLICK-02.png** 
3. **LOGOS AUTOCLICK-03.png**
4. **LOGOS AUTOCLICK-04.png** ← **ACTUALMENTE EN USO** ✅
5. **LOGOS AUTOCLICK-05.png**
6. **LOGOS AUTOCLICK-06.png**

## 🔄 Para Cambiar de Logo:

### Método 1: PowerShell (Rápido)
```powershell
# Cambia al logo 02
Copy-Item ".\LOGOS AUTOCLICK\LOGOS AUTOCLICK-02.png" ".\autoclick-logo-hd.png"

# Cambia al logo 03  
Copy-Item ".\LOGOS AUTOCLICK\LOGOS AUTOCLICK-03.png" ".\autoclick-logo-hd.png"

# etc...
```

### Método 2: Explorador de Archivos
1. Navega a `wwwroot/images/logos/LOGOS AUTOCLICK/`
2. Copia el logo que prefieras (ej: `LOGOS AUTOCLICK-03.png`)
3. Pégalo en la carpeta padre (`wwwroot/images/logos/`)
4. Renómbralo a `autoclick-logo-hd.png`
5. Confirma reemplazar el archivo existente

## 🎨 Estilos CSS Actuales:

```css
.logo-image {
    width: 148px;
    height: 23.49px;
}
```

## 📋 Ubicaciones que Usan el Logo:

- **Header Universal:** `Pages/Shared/_Header.cshtml`
- **Página Explorar:** `Pages/Explorar.cshtml` 
- **Página Guardados:** `Pages/Guardados.cshtml`

## 💡 Recomendaciones:

- **Logo 01-03:** Probablemente mejores para headers (más horizontales)
- **Logo 04-06:** Pueden ser mejor para usos cuadrados o favicon
- **Siempre usa PNG** para mantener la transparencia
- **Reinicia la aplicación** después de cambiar logos para ver cambios