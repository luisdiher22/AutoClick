# Guía de Testing - Módulo de Publicidad Responsive

## Fecha: 22 de diciembre de 2025

---

## 🎯 Cambios Implementados

Se ha actualizado completamente el sistema de dimensionamiento de anuncios publicitarios para garantizar que **las imágenes NUNCA excedan el tamaño de su contenedor**.

### Nuevo Enfoque

**ANTES:**
- Dimensiones fijas con `height` en píxeles
- `min-height` podía hacer que el contenido exceda el contenedor
- Imágenes podían solaparse con contenido siguiente
- Inconsistencia entre breakpoints

**AHORA:**
- `min-height` + `max-height` establecen un rango fijo
- `aspect-ratio` mantiene proporciones correctas
- `object-fit: contain` asegura que imagen siempre quepa dentro
- Tamaños consistentes en todos los dispositivos
- **Las imágenes se verán más pequeñas pero NUNCA solaparán contenido**

---

## 📐 Dimensiones por Dispositivo

### Anuncios Horizontales (1010x189)

| Dispositivo | Min Height | Max Height | Aspect Ratio |
|-------------|------------|------------|--------------|
| Desktop (>1024px) | 120px | 189px | 1010 / 189 |
| Tablet (≤1024px) | 100px | 150px | 1010 / 189 |
| Móvil (≤768px) | 80px | 120px | 16 / 9 |
| Móvil pequeño (≤480px) | 70px | 100px | 16 / 9 |

### Anuncios Cuadrados/Grandes Verticales (344x423)

| Dispositivo | Min Height | Max Height | Aspect Ratio |
|-------------|------------|------------|--------------|
| Desktop (>1024px) | 250px | 423px | 344 / 423 |
| Tablet (≤1024px) | 220px | 370px | 344 / 423 |
| Móvil (≤768px) | 200px | 350px | 1 / 1 |
| Móvil pequeño (≤480px) | 180px | 280px | 1 / 1 |

### Anuncios Medio Verticales (401x287)

| Dispositivo | Min Height | Max Height | Aspect Ratio |
|-------------|------------|------------|--------------|
| Desktop (>1024px) | 200px | 287px | 401 / 287 |
| Tablet (≤1024px) | 180px | 250px | 401 / 287 |
| Móvil (≤768px) | 150px | 250px | 4 / 3 |
| Móvil pequeño (≤480px) | 130px | 200px | 4 / 3 |

---

## 🧪 Páginas para Testing

A continuación, las mejores páginas para probar cada tipo de anuncio:

### 1. Anuncios Horizontales

#### **Mejor para Testing: `/Explorar`**
- **Ubicación:** Tiene 2 banners horizontales (explorar-banner-1 y explorar-banner-2)
- **Por qué:** Puedes ver dos anuncios horizontales en secuencia vertical
- **Qué testear:** 
  - Verifica que no se solapen entre sí
  - Reduce tamaño de ventana gradualmente
  - Confirma que mantienen espacio consistente

#### **También en:**
- `/MisAnuncios` - Footer horizontal (mis-anuncios-footer)
- `/Guardados` - Footer horizontal (guardados-footer)
- `/Destacados` - Banner y footer horizontal
- `/RecienVistos` - Footer horizontal
- `/ReportarProblema` - Footer horizontal
- `/PpfCeramico` - Footer horizontal (rainx-faq-footer)

### 2. Anuncios Cuadrados/Grande Vertical

#### **Mejor para Testing: `/Producto`**
- **Ubicación:** Sidebar con 2 anuncios cuadrados (producto-sidebar-1 y producto-sidebar-2)
- **Por qué:** Estos son de 560x688px (más grandes que los normales)
- **Qué testear:**
  - Ver cómo se adaptan en desktop (muy grandes)
  - Cambiar a tablet y ver reducción gradual
  - En móvil deben ser cuadrados (1:1) y no exceder pantalla

#### **También en:**
- `/Explorar` - Sidebar cuadrado
- `/MisAnuncios` - Sidebar cuadrado
- `/Guardados` - Múltiples posiciones (grid-desktop, grid-mobile, grid-tablet)
- `/RecienVistos` - Múltiples posiciones
- `/Destacados` - Sidebar cuadrado
- `/PerfilAgencia` - Sidebar cuadrado

### 3. Anuncios Medio Vertical

#### **Mejor para Testing: `/Gumout`**
- **Ubicación:** Sidebar medio vertical (gumout-sidebar)
- **Por qué:** Único lugar que usa este formato
- **Qué testear:**
  - Proporción 401x287 en desktop
  - Cambio a 4:3 en móvil
  - Que no exceda el contenedor

---

## ✅ Checklist de Testing

### Test 1: Desktop (>1024px)
- [ ] Abrir `/Explorar` en pantalla completa
- [ ] Los 2 banners horizontales se ven completos
- [ ] Hay espacio claro entre ambos banners
- [ ] No hay solapamiento con contenido
- [ ] Placeholders se ven bien si no hay anuncios

### Test 2: Tablet (768px - 1024px)
- [ ] Reducir ventana a ~900px de ancho
- [ ] Anuncios horizontales reducen altura (máx 150px)
- [ ] Mantienen aspect ratio correcto
- [ ] Sidebar en `/Producto` se ajusta correctamente

### Test 3: Móvil (≤768px)
- [ ] Reducir ventana a ~600px
- [ ] Anuncios horizontales cambian a ratio 16:9
- [ ] Altura máxima 120px
- [ ] Anuncios cuadrados se vuelven 1:1 (perfectamente cuadrados)
- [ ] **CRÍTICO:** Ningún anuncio sobresale de su contenedor

### Test 4: Móvil Pequeño (≤480px)
- [ ] Reducir ventana a ~400px
- [ ] Anuncios horizontales: altura máx 100px
- [ ] Anuncios cuadrados: altura máx 280px
- [ ] Todo visible sin scroll horizontal

### Test 5: Placeholders
- [ ] Verificar que placeholders de escritorio cargan en desktop
- [ ] Verificar que placeholders móviles cargan en ≤768px
- [ ] Imagen `PlaceholderHorizontal_escritorio.png` se ve completa
- [ ] No hay distorsión en las imágenes

---

## 🐛 Problemas Específicos que se Resolvieron

### 1. **Placeholder horizontal se veía mal**
**Causa:** Usaba `object-fit: cover` que recortaba la imagen
**Solución:** Cambio a `object-fit: contain`
**Testear en:** Cualquier página sin anuncios, especialmente `/Explorar`

### 2. **Imágenes solapaban siguiente anuncio**
**Causa:** `min-height` sin `max-height` permitía crecimiento ilimitado
**Solución:** Sistema de `min-height` + `max-height` con rango controlado
**Testear en:** `/Explorar` con 2 banners horizontales

### 3. **Espacios de carousel más pequeños que imagen**
**Causa:** Dimensiones fijas sin considerar el contenedor padre
**Solución:** `aspect-ratio` + `max-width: 100%` + `contain: layout`
**Testear en:** `/Guardados` y `/RecienVistos` (múltiples carruseles)

---

## 📊 Verificación Visual Rápida

### Comando para probar:
```bash
# En la terminal del proyecto
dotnet run
```

### URLs de Testing Rápido:

1. **Horizontal:** http://localhost:5000/Explorar
2. **Cuadrado:** http://localhost:5000/Producto
3. **Medio Vertical:** http://localhost:5000/Gumout
4. **Múltiples formatos:** http://localhost:5000/Guardados

### Herramientas del Navegador:

**Chrome/Edge DevTools:**
1. F12 para abrir DevTools
2. Ctrl+Shift+M para modo responsive
3. Probar estos anchos: 1920px, 1024px, 768px, 480px, 375px
4. Inspeccionar elemento del anuncio y verificar:
   - `height` calculado ≤ `max-height`
   - `height` calculado ≥ `min-height`
   - No hay overflow visible

---

## 📝 Notas Importantes

### Comportamiento Esperado

1. **Las imágenes se verán más pequeñas que antes**
   - Esto es INTENCIONAL
   - Garantiza que nunca solapen contenido
   - Mantiene consistencia en todos los dispositivos

2. **`object-fit: contain`**
   - Las imágenes mantienen sus proporciones completas
   - Pueden aparecer "letterboxes" (barras negras) si aspect ratio no coincide
   - Esto es preferible a recortar contenido importante

3. **Aspect Ratios Diferentes en Móvil**
   - Horizontal: Cambia de 5.3:1 a 16:9
   - Cuadrado: Cambia de 0.8:1 a 1:1
   - Más apropiado para pantallas móviles pequeñas

### Archivos Modificados

```
Pages/Shared/Components/
├── CarruselPublicidad/Default.cshtml ✓
├── AnuncioHorizontal/Default.cshtml ✓
├── AnuncioGrandeVertical/Default.cshtml ✓
└── AnuncioMedioVertical/Default.cshtml ✓
```

---

## 🔧 Troubleshooting

### Si una imagen aún se ve recortada:
1. Verificar que el CSS se recargó (Ctrl+F5)
2. Inspeccionar con DevTools el `object-fit` (debe ser `contain`)
3. Verificar que no hay estilos inline sobrescribiendo

### Si los placeholders no cambian en móvil:
1. Verificar que el navegador soporta `<picture>`
2. Limpiar cache del navegador
3. Verificar que las URLs del blob de Azure son correctas

### Si hay espacios en blanco grandes:
1. Es comportamiento esperado con `object-fit: contain`
2. Significa que el aspect ratio de la imagen no coincide con el contenedor
3. Considerar subir imagen con aspect ratio más cercano al objetivo

---

## ✅ Resultado Esperado

Después de estos cambios, deberías poder:

✅ Reducir gradualmente el tamaño de la ventana sin que ningún anuncio sobresalga
✅ Ver claramente el espacio entre anuncios consecutivos
✅ Los placeholders se ven completos y legibles en todos los tamaños
✅ Consistencia visual en todas las páginas
✅ No hay scroll horizontal en móvil debido a anuncios

---

## 🎯 Prioridad de Testing

**Alta Prioridad:**
1. `/Explorar` - Verifica que 2 banners horizontales no se solapen
2. `/Producto` - Verifica anuncios grandes (560x688) en sidebar
3. Testing responsive en Chrome DevTools

**Media Prioridad:**
4. `/Guardados` y `/RecienVistos` - Múltiples carruseles
5. Verificar placeholders en páginas sin anuncios

**Baja Prioridad:**
6. Otras páginas con anuncios footer

---

**Compilación:** ✅ Exitosa sin errores
**Advertencias:** Solo pre-existentes (no relacionadas con estos cambios)
**Testing recomendado:** 15-20 minutos cubriendo todos los breakpoints
