# 📊 Guía de Integración de Application Insights - AutoClick

## ✅ Cambios Completados

Se ha integrado **Microsoft Application Insights** para tracking de visitas en el Dashboard de Admin.

### 1️⃣ Archivos Modificados

#### **AutoClick.csproj**
- ✅ Agregado paquete NuGet: `Microsoft.ApplicationInsights.AspNetCore` v2.22.0

#### **Program.cs**
- ✅ Configurado Application Insights telemetry
- ✅ Registrado servicio `IApplicationInsightsService`

#### **appsettings.json**
- ✅ Agregada sección `ApplicationInsights` con placeholder para Connection String

### 2️⃣ Nuevos Archivos Creados

#### **Services/IApplicationInsightsService.cs**
- Interfaz del servicio de Application Insights

#### **Services/ApplicationInsightsService.cs**
- Implementación del servicio
- Métodos para obtener: visitas mensuales, semanales y diarias
- **NOTA**: Actualmente retorna 0 (requiere Azure Monitor REST API para datos históricos)

#### **Pages/Admin/Dashboard.cshtml.cs**
- ✅ Actualizado para inyectar `IApplicationInsightsService`
- ✅ Integrado con Application Insights para métricas de visitas
- ✅ Agregado método `FormatNumber()` para formatear números grandes (K/M)

#### **Pages/Admin/Dashboard.cshtml**
- ✅ Actualizado para mostrar datos reales de Application Insights

#### **setup-application-insights.ps1**
- Script PowerShell automatizado para crear el recurso en Azure

---

## 🚀 Pasos Para Completar la Integración

### **Paso 1: Ejecutar el Script de PowerShell**

Abre PowerShell y ejecuta:

```powershell
cd C:\Users\luisd\Documents\AutoClick
.\setup-application-insights.ps1
```

**IMPORTANTE**: Antes de ejecutar, modifica estas variables en el script:

```powershell
$resourceGroupName = "tu-resource-group"  # El resource group donde está tu base de datos
$location = "eastus"                      # La misma región que tu base de datos
```

El script creará:
1. Log Analytics Workspace (`autoclick-workspace`)
2. Application Insights resource (`autoclick-insights`)
3. Te mostrará el **Connection String** (guárdalo)

---

### **Paso 2: Actualizar appsettings.json**

Copia el **Connection String** que te dio el script y agrégalo en `appsettings.json`:

```json
"ApplicationInsights": {
  "ConnectionString": "InstrumentationKey=xxx;IngestionEndpoint=https://xxx"
}
```

**Ubicación del archivo**: `AutoClick\appsettings.json`

---

### **Paso 3: Verificar la Compilación**

```powershell
cd AutoClick
dotnet build
```

Debe compilar sin errores ✅

---

### **Paso 4: Publicar a Azure (si aún no lo has hecho)**

```powershell
dotnet publish --configuration Release
```

Luego despliega la aplicación a Azure App Service.

---

## 📈 Cómo Funciona Application Insights

### **Tracking Automático**
Application Insights **automáticamente trackea**:
- ✅ Page Views (cada vez que alguien visita una página)
- ✅ HTTP Requests (todas las peticiones a tu API/Controllers)
- ✅ Dependencies (llamadas a base de datos, APIs externas)
- ✅ Exceptions (errores no manejados)
- ✅ Performance (tiempos de respuesta)

### **En el Dashboard**
Una vez configurado, el Dashboard mostrará:
- **Visitas mensuales**: Últimos 30 días
- **Visitas semanales**: Últimos 7 días  
- **Visitas hoy**: Día actual

Los números se formatean automáticamente:
- `5234` → `5.2K`
- `1589432` → `1.6M`

---

## ⚠️ IMPORTANTE: Limitación Actual

El servicio `ApplicationInsightsService` actualmente **retorna 0** para métricas históricas porque:

- `TelemetryClient` NO permite consultar datos históricos directamente
- Para obtener datos reales necesitas usar **Azure Monitor REST API**

### **Solución A: Usar Azure Monitor REST API (Recomendado)**

Instala este paquete adicional:

```powershell
dotnet add package Azure.Monitor.Query
```

Y actualiza `ApplicationInsightsService.cs` para usar `LogsQueryClient`:

```csharp
// Ejemplo de consulta KQL para pageViews
var query = @"
pageViews
| where timestamp > ago(30d)
| summarize count()
";
```

### **Solución B: Portal de Azure (Para testing)**

Mientras implementas la API, puedes ver las métricas en:
1. Azure Portal → Application Insights
2. Ir a "Logs" o "Metrics"
3. Ver gráficas de Page Views

---

## 🔍 Ver los Datos en Azure Portal

1. Ve a [Azure Portal](https://portal.azure.com)
2. Busca tu recurso "autoclick-insights"
3. Menú izquierdo → **"Logs"**
4. Ejecuta esta query KQL:

```kql
pageViews
| where timestamp > ago(30d)
| summarize TotalViews = count() by bin(timestamp, 1d)
| order by timestamp desc
```

Verás todas las visitas por día.

---

## 📊 Próximos Pasos (Opcionales)

### **1. Implementar Azure Monitor Query API**
- Instalar `Azure.Monitor.Query` NuGet package
- Actualizar `ApplicationInsightsService.cs`
- Consultar datos históricos con KQL

### **2. Agregar Custom Events**
Ya tienes métodos disponibles en el servicio:

```csharp
_appInsightsService.TrackEvent("Usuario_Anuncio_Creado", new Dictionary<string, string>
{
    { "UserId", userId },
    { "AutoId", autoId }
});
```

### **3. Dashboards Personalizados en Azure**
- Crear dashboards en Azure Portal
- Exportar reportes automáticos
- Configurar alertas (ej: si visitas bajan 50%)

---

## ✅ Checklist de Integración

- [x] Paquete NuGet instalado (`Microsoft.ApplicationInsights.AspNetCore`)
- [x] Program.cs configurado
- [x] Servicios creados (Interface + Implementación)
- [x] Dashboard.cshtml.cs actualizado
- [ ] Ejecutar script PowerShell para crear recurso en Azure
- [ ] Copiar Connection String a appsettings.json
- [ ] Desplegar aplicación a Azure
- [ ] Verificar métricas en Azure Portal (después de algunas visitas)
- [ ] (Opcional) Implementar Azure Monitor Query API para datos históricos

---

## 🆘 Troubleshooting

### **Error: "ConnectionString is null"**
- Verifica que agregaste el Connection String en `appsettings.json`
- Reinicia la aplicación después de modificar appsettings.json

### **No veo datos en el Dashboard**
- Application Insights toma **5-10 minutos** en procesar los primeros datos
- Visita tu sitio varias veces para generar tráfico
- Verifica en Azure Portal → Application Insights → "Live Metrics" (datos en tiempo real)

### **Dashboard muestra "0" en visitas**
- Normal si acabas de configurar (datos históricos requieren Azure Monitor Query API)
- Por ahora, verifica las métricas directamente en Azure Portal

---

## 📚 Recursos Adicionales

- [Documentación oficial de Application Insights](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [Azure Monitor Query API](https://learn.microsoft.com/azure/azure-monitor/logs/api/overview)
- [KQL Query Language](https://learn.microsoft.com/azure/data-explorer/kusto/query/)

---

**Última actualización**: 24 de octubre de 2025
**Versión**: 1.0
