# 🚀 ONVO Pay - Guía Rápida de Inicio

## ✅ Implementación Completada

La integración de ONVO Pay está **100% implementada y lista para usar**.

## 📦 Archivos Creados

### Backend
- ✅ `Models/PagoOnvo.cs` - Modelo de datos para pagos
- ✅ `Models/WebhookEventOnvo.cs` - Log de webhooks
- ✅ `Services/OnvoPayService.cs` - Lógica de integración
- ✅ `Services/OnvoPaySettings.cs` - Configuración
- ✅ `Services/OnvoPayDtos.cs` - DTOs
- ✅ `Controllers/Api/PagosController.cs` - API de pagos
- ✅ `Controllers/Api/OnvoWebhookController.cs` - Webhook handler
- ✅ `Migrations/xxxxx_AddOnvoPayTables.cs` - Migración de DB

### Frontend
- ✅ `Pages/Pagos/ProcessPayment.cshtml` - Página de checkout
- ✅ `Pages/Pagos/ProcessPayment.cshtml.cs` - PageModel
- ✅ `wwwroot/js/onvo-integration-examples.js` - Ejemplos de uso

### Documentación
- ✅ `DOCS/ONVO_PAY_INTEGRATION.md` - Documentación completa

## 🔧 Pasos para Activar

### 1. Aplicar Migración de Base de Datos

```bash
# Opción 1: Aplicar localmente
cd c:\Users\Admin\Desktop\repos vsc\AutoClick\AutoClick\AutoClick
dotnet ef database update --context ApplicationDbContext

# Opción 2: Aplicar en Azure (producción)
# Se aplicará automáticamente en el próximo despliegue
```

### 2. Configurar Webhook en ONVO Dashboard

1. Ir a https://dashboard.onvopay.com
2. Navegar a **Desarrolladores** → **Webhooks**
3. Agregar nuevo webhook:
   ```
   URL: https://autoclick.azurewebsites.net/api/onvowebhook
   ```
4. Copiar el **Secret** generado
5. Actualizar `appsettings.json`:
   ```json
   "OnvoPay": {
     "WebhookSecret": "tu_webhook_secret_aqui"
   }
   ```

### 3. Para Producción: Cambiar a Keys Live

En `appsettings.json` o variables de entorno:

```json
"OnvoPay": {
  "SecretKey": "onvo_live_secret_key_...",
  "PublishableKey": "onvo_live_publishable_key_...",
  "WebhookSecret": "tu_webhook_secret_de_produccion"
}
```

## 🎯 Cómo Usar

### Opción 1: Redirigir a Página de Pago

```html
<a href="/Pagos/ProcessPayment?anuncioId=123&amount=5000&currency=USD&description=Plan Premium"
   class="btn btn-primary">
    Pagar $50.00
</a>
```

### Opción 2: JavaScript

```javascript
function pagarAnuncio(anuncioId, monto) {
    window.location.href = `/Pagos/ProcessPayment?anuncioId=${anuncioId}&amount=${monto}&currency=USD`;
}
```

### Opción 3: Desde Backend (C#)

```csharp
// Crear Payment Intent programáticamente
var paymentIntent = await _onvoPayService.CreatePaymentIntentAsync(
    amount: 5000,        // $50.00 en centavos
    currency: "USD",
    description: "Plan Premium - 30 días",
    usuarioId: 123,
    anuncioPublicidadId: 456
);

// Redirigir al frontend con el paymentIntentId
return Redirect($"/Pagos/ProcessPayment?paymentIntentId={paymentIntent.id}");
```

## 🧪 Testing

### Tarjetas de Prueba

```
✅ Aprobada (VISA): 4242424242424242
❌ Declinada: 4000000000000002
🔐 3DS: 4000000000003220

Expiración: Cualquier fecha futura (ej: 12/26)
CVV: Cualquier 3 dígitos (ej: 123)
Nombre: Cualquier nombre
```

### SINPE Móvil de Prueba

```
✅ Exitoso: +50688888888
⏱️ Con retraso: +50688884444
❌ Fallido: +50688889521
```

## 📊 Monitoreo

### Ver Pagos en la DB

```sql
SELECT * FROM PagosOnvo 
ORDER BY CreatedAt DESC;
```

### Ver Webhooks Recibidos

```sql
SELECT * FROM WebhookEventsOnvo 
ORDER BY ReceivedAt DESC;
```

### API Endpoints

```bash
# Crear pago
POST /api/pagos/create-payment-intent

# Consultar estado
GET /api/pagos/status/{paymentIntentId}

# Health check del webhook
GET /api/onvowebhook/health

# Historial de webhooks
GET /api/onvowebhook/history?limit=10
```

## ⚡ Flujo Completo

1. Usuario completa formulario de anuncio
2. Sistema crea registro de anuncio (inactivo)
3. Usuario es redirigido a página de pago
4. Backend crea Payment Intent en ONVO
5. Frontend muestra formulario de pago (SDK)
6. Usuario ingresa datos y confirma
7. ONVO procesa el pago
8. ONVO envía webhook a tu servidor
9. Webhook actualiza estado del pago
10. Si es exitoso, activa el anuncio automáticamente
11. Usuario ve confirmación

## 🔒 Seguridad

- ✅ Secret Key nunca se expone al frontend
- ✅ Webhook valida secret header
- ✅ Todos los pagos se registran en DB
- ✅ Logs completos de transacciones
- ✅ HTTPS obligatorio en producción

## 📝 Próximos Pasos

1. [ ] Aplicar migración de base de datos
2. [ ] Configurar webhook en ONVO Dashboard
3. [ ] Probar con tarjetas de prueba
4. [ ] Integrar en tu flujo de publicar anuncios
5. [ ] Cuando estés listo, cambiar a keys de producción

## 📚 Documentación Completa

Ver `DOCS/ONVO_PAY_INTEGRATION.md` para:
- Guía completa de API
- Ejemplos avanzados
- Troubleshooting
- Estados de pago
- Mejores prácticas

## 🆘 Soporte

Si tienes dudas:
1. Revisar `DOCS/ONVO_PAY_INTEGRATION.md`
2. Ver ejemplos en `wwwroot/js/onvo-integration-examples.js`
3. Consultar logs de aplicación
4. Documentación oficial: https://docs.onvopay.com

---

✨ **¡La integración está lista para usar!** ✨
