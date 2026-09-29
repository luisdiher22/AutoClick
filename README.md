# AutoClick

AutoClick es una aplicación web para publicar, buscar y administrar anuncios de vehículos. El proyecto está orientado al mercado de Costa Rica e incluye funciones para usuarios particulares, agencias y administradores.

## Funcionalidades

- Registro e inicio de sesión, perfiles de usuario y recuperación de contraseña.
- Publicación y administración de anuncios de vehículos, con carga y procesamiento de imágenes.
- Búsqueda avanzada, vehículos destacados, favoritos y anuncios vistos recientemente.
- Perfiles y solicitudes para agencias, además de solicitudes de fotografía y preaprobación.
- Pagos en línea mediante ONVO Pay y recepción de notificaciones de pago por webhook.
- Espacios publicitarios, registro de impresiones y clics, y herramientas administrativas.
- Secciones de soporte, mensajes, reclamos y páginas informativas.
- Importación de ventas externas desde las herramientas de administración.

## Tecnologías

- C# y ASP.NET Core Razor Pages sobre .NET 9.
- Controladores ASP.NET Core para endpoints HTTP y API.
- Entity Framework Core 9 y SQL Server/Azure SQL.
- Azure Blob Storage para imágenes en la nube o almacenamiento local para desarrollo.
- MailKit para correo SMTP, ONVO Pay para pagos y EPPlus para hojas de cálculo.

## Estructura del repositorio

```text
.
├── AutoClick.sln
└── AutoClick/
    ├── Controllers/      # Endpoints MVC y API
    ├── Data/             # Contextos de Entity Framework
    ├── DOCS/             # Notas de configuración y operación
    ├── Migrations/       # Migraciones de la base de datos
    ├── Models/           # Entidades y modelos de la aplicación
    ├── Pages/            # Páginas Razor
    ├── Services/         # Correo, almacenamiento, pagos y lógica de negocio
    ├── wwwroot/          # CSS, JavaScript, bibliotecas e imágenes
    ├── Program.cs        # Registro de servicios y configuración de la aplicación
    └── AutoClick.csproj
```

## Requisitos

- SDK de .NET 9.
- Una instancia de SQL Server o Azure SQL accesible desde el equipo.
- Credenciales de Azure Blob Storage si se usa almacenamiento en Azure.
- Configuración SMTP para enviar correos.
- Credenciales de ONVO Pay para habilitar pagos.

SQL Server es necesario para las funciones que consultan o actualizan información. El almacenamiento de archivos se puede configurar en modo local durante el desarrollo.

## Configuración local

La configuración compartida de `AutoClick/appsettings.json` no contiene credenciales. Configura los valores sensibles mediante variables de entorno; ASP.NET Core convierte `__` en `:` al leer la configuración.

En PowerShell, define las variables necesarias para la sesión actual antes de iniciar la aplicación:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=<servidor>;Database=<base>;User Id=<usuario>;Password=<contraseña>;Encrypt=True;TrustServerCertificate=False;"

# Desarrollo con archivos en el disco local
$env:UseAzureStorage = "false"
$env:LocalStoragePath = "LocalStorage"

# Opcional: configura SMTP si vas a probar el envío de correos
$env:EmailSettings__SmtpHost = "<servidor-smtp>"
$env:EmailSettings__SmtpPort = "587"
$env:EmailSettings__SmtpUser = "<usuario-smtp>"
$env:EmailSettings__SmtpPassword = "<contraseña-smtp>"
$env:EmailSettings__FromEmail = "<correo-remitente>"
$env:EmailSettings__FromName = "AutoClick"

# Opcional: configura ONVO Pay si vas a probar pagos
$env:OnvoPay__SecretKey = "<clave-secreta>"
$env:OnvoPay__PublishableKey = "<clave-publica>"
$env:OnvoPay__WebhookSecret = "<secreto-del-webhook>"
```

Para almacenar archivos en Azure Blob Storage, configura `UseAzureStorage` como `true` y proporciona la cadena de conexión:

```powershell
$env:UseAzureStorage = "true"
$env:ConnectionStrings__AzureStorage = "<cadena-de-conexion-de-azure-storage>"
```

No guardes valores reales en este README ni en archivos que se vayan a subir al repositorio. `appsettings.Development.json` está excluido por `.gitignore`; úsalo solo para configuración local y no lo fuerces al control de versiones.

## Restaurar, compilar y ejecutar

Desde la raíz del repositorio:

```powershell
dotnet restore .\AutoClick\AutoClick.csproj
dotnet build .\AutoClick.sln
dotnet run --project .\AutoClick\AutoClick.csproj
```

Los perfiles de desarrollo también están configurados en `AutoClick/Properties/launchSettings.json`. Si el inicio por HTTPS falla por un certificado local, crea y confía el certificado de desarrollo de .NET con `dotnet dev-certs https --trust`, o inicia el perfil HTTP.

La aplicación estará disponible en la URL que indique `dotnet run` (los perfiles incluidos usan `https://localhost:7231` y `http://localhost:5160`).

## Base de datos

Las migraciones de Entity Framework están en `AutoClick/Migrations`. La aplicación no aplica automáticamente las migraciones al arrancar. Instala la herramienta `dotnet-ef` de la versión 9 si aún no está disponible y ejecuta desde la raíz:

```powershell
dotnet ef database update --project .\AutoClick\AutoClick.csproj --startup-project .\AutoClick\AutoClick.csproj
```

El comando necesita que `ConnectionStrings__DefaultConnection` apunte a una base de datos accesible y que el usuario tenga permisos para aplicar cambios de esquema.

## Publicación

Compila una publicación Release con:

```powershell
dotnet publish .\AutoClick\AutoClick.csproj --configuration Release --output .\publish
```

Antes de desplegar, configura en el entorno del servidor las variables de conexión a SQL Server, almacenamiento, SMTP y ONVO Pay que correspondan. El repositorio incluye `AutoClick/web.config` para hospedaje compatible con IIS. La guía de [configuración de carga de imágenes](AutoClick/DOCS/CONFIGURACION_PRODUCCION_IMAGENES.md) contiene notas operativas adicionales.

## Seguridad antes de producción

- No publiques secretos, cadenas de conexión, contraseñas ni archivos locales de configuración.
- Cambia o elimina cualquier credencial que haya sido expuesta anteriormente en el historial Git; borrar un archivo en un commit nuevo no la elimina de commits anteriores.
- Revisa el aprovisionamiento inicial de administrador implementado en `AutoClick/Program.cs` antes de desplegar. Debe reemplazarse por un mecanismo seguro, sin credenciales predeterminadas en el código.
- Protege las credenciales de ONVO Pay y el secreto del webhook en la configuración del entorno de producción.

## Documentación adicional

- [Configuración de carga de imágenes en producción](AutoClick/DOCS/CONFIGURACION_PRODUCCION_IMAGENES.md)
- [Información sobre logos](AutoClick/wwwroot/images/logos/README.md)
