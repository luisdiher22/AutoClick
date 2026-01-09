# 🔧 Configuración de Email - Diagnóstico y Solución

## ❌ Problema Identificado

El formulario de "Anunciar Empresa" **no está enviando emails** porque falta la configuración SMTP en producción.

## ✅ Solución Implementada

### 1. Configuración agregada a `appsettings.json`

Se agregó la sección `EmailSettings` que estaba faltante. **DEBES CONFIGURAR** estos valores en producción:

```json
"EmailSettings": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": "587",
  "SmtpUser": "tu-email@gmail.com",
  "SmtpPassword": "tu-contraseña-de-aplicacion",
  "FromEmail": "noreply@autoclick.cr",
  "FromName": "AutoClick.cr"
}
```

### 2. Configuración en Azure App Service (PRODUCCIÓN)

En el portal de Azure, ve a tu App Service > Configuration > Application settings y agrega:

```
EmailSettings__SmtpHost = smtp.gmail.com
EmailSettings__SmtpPort = 587
EmailSettings__SmtpUser = tu-email@gmail.com
EmailSettings__SmtpPassword = tu-contraseña-de-aplicacion-de-gmail
EmailSettings__FromEmail = noreply@autoclick.cr
EmailSettings__FromName = AutoClick.cr
```

⚠️ **Nota importante:** En Azure, usa `__` (doble guión bajo) en lugar de `:` para separar niveles de configuración.

## 📧 Cómo obtener una contraseña de aplicación de Gmail

1. Ve a tu cuenta de Google: https://myaccount.google.com/
2. Seguridad > Verificación en dos pasos (debes habilitarla primero)
3. Contraseñas de aplicaciones
4. Selecciona "Correo" y "Otro" (escribe "AutoClick")
5. Copia la contraseña generada (16 caracteres sin espacios)
6. Usa esa contraseña en `SmtpPassword`

## 🔍 Verificación de Administradores

Para que los emails se envíen correctamente, necesitas tener al menos un usuario administrador en la base de datos:

```sql
-- Verificar administradores
SELECT Email, EsAdministrador 
FROM Usuarios 
WHERE EsAdministrador = 1;

-- Si no hay administradores, crear uno:
UPDATE Usuarios 
SET EsAdministrador = 1 
WHERE Email = 'tu-email-admin@ejemplo.com';
```

## 🔨 Alternativas de SMTP

### Opción 1: Gmail (Actual en Development)
- Host: `smtp.gmail.com`
- Puerto: `587`
- SSL: Habilitado
- Límite: ~500 emails/día

### Opción 2: SendGrid (Recomendado para producción)
- Host: `smtp.sendgrid.net`
- Puerto: `587`
- Usuario: `apikey`
- Contraseña: Tu API Key de SendGrid
- Límite: 100 emails/día (gratis), más con planes pagos

### Opción 3: Azure Communication Services
- Servicio nativo de Azure
- Integración más fácil con App Service
- Escalable y confiable

## 📋 Pasos para probar

1. **Configurar el SMTP** en `appsettings.json` o Azure
2. **Verificar que existe al menos un administrador** en la tabla `Usuarios`
3. **Reiniciar la aplicación** en Azure
4. **Llenar el formulario** en /AnunciarEmpresa
5. **Verificar los logs** en Azure Application Insights o Log Stream

## 🐛 Cómo verificar si está funcionando

### En Development (Local):
El archivo `appsettings.Development.json` ya tiene la configuración:
- Email: `pablosalazar1122@gmail.com`
- La contraseña de aplicación ya está configurada

### Ver logs en la aplicación:
Busca en los logs estos mensajes:
- ✅ "Email enviado exitosamente a X administrador(es)"
- ⚠️ "Configuración SMTP incompleta. Email no enviado."
- ❌ "Error al enviar email: [mensaje de error]"

## 🚀 Estado Actual

- ✅ Servicio `IEmailService` registrado en Program.cs
- ✅ Configuración agregada a `appsettings.json` (con valores placeholder)
- ✅ Configuración existente en `appsettings.Development.json` (funcionando)
- ⚠️ **PENDIENTE:** Configurar SMTP en producción (Azure App Service)
- ⚠️ **PENDIENTE:** Verificar que existan administradores en la BD

## 💡 Nota Técnica

El código actual en `EmailService.cs` (líneas 58-62 y 112) retorna `true` aunque la configuración no esté completa, lo que puede hacer creer que el email se envió cuando realmente no se configuró el SMTP. 

Esto es intencional para no bloquear el flujo en desarrollo, pero en producción **debe configurarse correctamente** para que los emails lleguen.
