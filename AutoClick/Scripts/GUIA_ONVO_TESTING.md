# Guía: Configurar ONVO Pay para Testing en Producción

## 🎯 Resumen

Esta guía explica cómo cambiar entre keys de **TEST** y **LIVE** de ONVO Pay en Azure **sin modificar el código**.

---

## 📋 Keys Disponibles

### Keys de TEST (para pruebas)
```
PublishableKey: onvo_test_publishable_key_h8X3OFE1Zl8bNZuQeSDffeSj49ECa6UJIWBJ4Exc681b1jXBIPaz9cSqsFW0olnKls0lrFgGBy8HKzSv_sBRwA
SecretKey: onvo_test_secret_key_HDeP2VKZsEn5B9vx8iZExxjXNiFL_TM6SO9kdKc_IqlU2CsuKn0BD_DiCgQTsWvfuM47OyeP4KuFvt9ml1eY-g
WebhookSecret: webhook_secret_oWeUDYVMMgP2e-sl
```

**Características:**
- ✅ Pagos simulados (no cobran dinero real)
- ✅ Usar tarjetas de prueba
- ✅ Ideal para testing y desarrollo

### Keys de LIVE (producción real)
```
(Debes obtenerlas del dashboard de ONVO)
```

**Características:**
- 💰 Pagos reales (cobran dinero verdadero)
- 💳 Tarjetas reales de clientes
- ⚠️ Usar solo cuando estés listo para producción

---

## 🚀 Método 1: Scripts de PowerShell (RECOMENDADO)

### Paso 1: Verificar configuración actual

```powershell
cd 'c:\Users\Admin\Desktop\repos vsc\AutoClick\AutoClick\AutoClick\Scripts'
powershell -ExecutionPolicy Bypass -File .\VerificarOnvoConfig.ps1
```

Esto te muestra:
- Qué keys están activas (TEST o LIVE)
- Si hay keys configuradas
- El webhook URL

### Paso 2: Activar keys de TEST en producción

```powershell
powershell -ExecutionPolicy Bypass -File .\ConfigurarOnvoTest.ps1
```

- Te pedirá confirmación
- Configurará las 3 keys de TEST
- La app se reiniciará automáticamente (~30 segundos)

### Paso 3: Configurar webhook en ONVO Dashboard

1. Ve a: https://dashboard.onvopay.com
2. **Desarrolladores** → **Webhooks**
3. Agrega un nuevo webhook:
   ```
   URL: https://autoclick.cr/api/onvowebhook
   Secret: webhook_secret_oWeUDYVMMgP2e-sl
   ```

### Paso 4: Hacer testing

Usa estas **tarjetas de prueba**:

**Aprobada:**
```
Número: 4242424242424242
Expiración: 12/26
CVV: 123
```

**Declinada:**
```
Número: 4000000000000002
Expiración: 12/26
CVV: 123
```

### Paso 5: Volver a keys LIVE (cuando termines el testing)

```powershell
powershell -ExecutionPolicy Bypass -File .\ConfigurarOnvoLive.ps1
```

- Te pedirá las keys LIVE
- Validará que no sean keys TEST
- Configurará producción real

---

## 🖥️ Método 2: Azure Portal (Manual)

### Paso 1: Ir a Azure Portal

1. Ve a: https://portal.azure.com
2. Busca tu App Service: **AutoClick**

### Paso 2: Configurar Application Settings

1. En el menú izquierdo: **Configuración** → **Configuración de la aplicación**
2. En **Configuración de la aplicación**, agrega o edita:

```
Nombre: OnvoPay__SecretKey
Valor: onvo_test_secret_key_HDeP2VKZsEn5B9vx8iZExxjXNiFL_TM6SO9kdKc_IqlU2CsuKn0BD_DiCgQTsWvfuM47OyeP4KuFvt9ml1eY-g

Nombre: OnvoPay__PublishableKey
Valor: onvo_test_publishable_key_h8X3OFE1Zl8bNZuQeSDffeSj49ECa6UJIWBJ4Exc681b1jXBIPaz9cSqsFW0olnKls0lrFgGBy8HKzSv_sBRwA

Nombre: OnvoPay__WebhookSecret
Valor: webhook_secret_oWeUDYVMMgP2e-sl
```

3. Click en **Guardar**
4. La app se reiniciará automáticamente

---

## 🔍 Método 3: Azure CLI (Comandos directos)

### Activar keys de TEST

```powershell
# Secret Key
az webapp config appsettings set `
    --name AutoClick `
    --resource-group AutoClick `
    --settings OnvoPay__SecretKey="onvo_test_secret_key_HDeP2VKZsEn5B9vx8iZExxjXNiFL_TM6SO9kdKc_IqlU2CsuKn0BD_DiCgQTsWvfuM47OyeP4KuFvt9ml1eY-g"

# Publishable Key
az webapp config appsettings set `
    --name AutoClick `
    --resource-group AutoClick `
    --settings OnvoPay__PublishableKey="onvo_test_publishable_key_h8X3OFE1Zl8bNZuQeSDffeSj49ECa6UJIWBJ4Exc681b1jXBIPaz9cSqsFW0olnKls0lrFgGBy8HKzSv_sBRwA"

# Webhook Secret
az webapp config appsettings set `
    --name AutoClick `
    --resource-group AutoClick `
    --settings OnvoPay__WebhookSecret="webhook_secret_oWeUDYVMMgP2e-sl"
```

### Verificar configuración

```powershell
az webapp config appsettings list `
    --name AutoClick `
    --resource-group AutoClick `
    --query "[?contains(name, 'OnvoPay')]"
```

---

## 📊 Configuración Local (Desarrollo)

Para desarrollo local, las keys de TEST ya están en `appsettings.Development.json`:

```json
{
  "OnvoPay": {
    "BaseUrl": "https://api.onvopay.com",
    "SecretKey": "onvo_test_secret_key_...",
    "PublishableKey": "onvo_test_publishable_key_...",
    "WebhookSecret": "webhook_secret_oWeUDYVMMgP2e-sl",
    "Currency": "CRC",
    "TimeoutSeconds": 30
  }
}
```

Cuando ejecutes `dotnet run` localmente, usará automáticamente las keys de TEST.

---

## ⚠️ IMPORTANTE: Seguridad

### ✅ LO QUE ESTÁ BIEN:
- Keys de TEST en el código (appsettings.Development.json)
- Keys vacías en appsettings.json (producción)
- Keys reales en Azure Application Settings

### ❌ NUNCA HAGAS ESTO:
- Poner keys LIVE en el código
- Commitear keys reales a Git
- Compartir keys LIVE públicamente

---

## 🧪 Flujo de Testing Recomendado

1. **Activar keys de TEST en producción:**
   ```powershell
   .\ConfigurarOnvoTest.ps1
   ```

2. **Hacer pruebas con tarjetas de prueba**
   - Probar flujo completo de pago
   - Verificar que el webhook se reciba
   - Confirmar que el auto/anuncio se active

3. **Revisar logs si hay problemas:**
   ```powershell
   .\VerificarOnvoConfig.ps1  # Ver configuración
   az webapp log tail --name AutoClick --resource-group AutoClick  # Ver logs
   ```

4. **Cuando todo funcione, cambiar a LIVE:**
   ```powershell
   .\ConfigurarOnvoLive.ps1
   ```

---

## 📞 URLs y Endpoints

### Webhook URL (Producción)
```
https://autoclick.cr/api/onvowebhook
```

### ONVO Dashboard
```
https://dashboard.onvopay.com
```

### Página de pago (ejemplo)
```
https://autoclick.cr/Pagos/ProcessPayment?autoId=161&amount=1561580
```

---

## ✅ Checklist de Testing

- [ ] Keys de TEST configuradas en Azure
- [ ] Webhook configurado en ONVO Dashboard
- [ ] App reiniciada (esperar ~30 segundos)
- [ ] Probar pago con tarjeta de prueba aprobada (4242...)
- [ ] Verificar que el webhook llegue
- [ ] Confirmar que el auto se active en la BD
- [ ] Probar pago con tarjeta declinada (4000...)
- [ ] Verificar manejo de errores
- [ ] Cambiar a keys LIVE cuando todo funcione

---

## 🛠️ Troubleshooting

### Problema: "Keys no configuradas"
```powershell
.\VerificarOnvoConfig.ps1  # Ver qué está configurado
.\ConfigurarOnvoTest.ps1   # Reconfigurar
```

### Problema: "Webhook no se recibe"
1. Verificar URL en ONVO Dashboard
2. Verificar que el secret sea correcto
3. Ver logs: `az webapp log tail --name AutoClick --resource-group AutoClick`

### Problema: "Pago no activa el auto"
1. Consultar base de datos para ver si el pago llegó
2. Verificar tabla `WebhookEventsOnvo` para errores
3. Revisar logs de la aplicación

---

## 📚 Scripts Disponibles

| Script | Descripción |
|--------|-------------|
| `ConfigurarOnvoTest.ps1` | Activa keys de TEST en producción |
| `ConfigurarOnvoLive.ps1` | Activa keys LIVE en producción |
| `VerificarOnvoConfig.ps1` | Muestra configuración actual |

---

¿Preguntas? Revisa los logs o contacta al equipo de desarrollo.
