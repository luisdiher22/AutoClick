using AutoClick.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AutoClick.Services
{
    /// <summary>
    /// Interfaz para el servicio de envío de correos electrónicos
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía una notificación por email a los administradores cuando se recibe una solicitud de empresa
        /// </summary>
        Task<bool> EnviarNotificacionSolicitudEmpresaAsync(SolicitudEmpresa solicitud, List<string> correosAdmins, bool esEspacioPublicitario = false);
        
        /// <summary>
        /// Envía un email con el token de recuperación de contraseña
        /// </summary>
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken);
    }

    /// <summary>
    /// Servicio de envío de correos electrónicos usando MailKit (más confiable para Azure)
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EnviarNotificacionSolicitudEmpresaAsync(SolicitudEmpresa solicitud, List<string> correosAdmins, bool esEspacioPublicitario = false)
        {
            try
            {
                if (correosAdmins == null || !correosAdmins.Any())
                {
                    _logger.LogWarning("No se encontraron correos de administradores para enviar la notificación");
                    return false;
                }

                // Configuración SMTP desde appsettings
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "465");
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPass = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? smtpUser;
                var fromName = _configuration["EmailSettings:FromName"] ?? "AutoClick.cr";

                // Validar configuración
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    _logger.LogWarning("Configuración SMTP incompleta. SmtpHost: {Host}, SmtpUser: {User}, SmtpPass: {Pass}", 
                        smtpHost ?? "NULL", 
                        smtpUser ?? "NULL", 
                        string.IsNullOrEmpty(smtpPass) ? "NULL" : "***SET***");
                    LogSolicitudEnConsola(solicitud, correosAdmins);
                    return true;
                }

                var subject = esEspacioPublicitario 
                    ? $"Solicitud de Espacio Publicitario - {solicitud.NombreEmpresa}"
                    : $"Nueva Solicitud de Empresa - {solicitud.NombreEmpresa}";

                // Crear mensaje con MimeKit
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                
                foreach (var email in correosAdmins)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        message.To.Add(MailboxAddress.Parse(email));
                    }
                }

                if (message.To.Count == 0)
                {
                    _logger.LogWarning("No hay destinatarios válidos para enviar el correo");
                    return false;
                }

                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = GenerarCuerpoEmail(solicitud, esEspacioPublicitario)
                };

                // Enviar con MailKit
                using (var client = new SmtpClient())
                {
                    _logger.LogInformation("Conectando a SMTP {Host}:{Port}...", smtpHost, smtpPort);
                    
                    // Puerto 465 usa SSL directo, puerto 587 usa STARTTLS
                    if (smtpPort == 465)
                    {
                        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.SslOnConnect);
                    }
                    else
                    {
                        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                    }
                    
                    _logger.LogInformation("Autenticando con usuario {User}...", smtpUser);
                    await client.AuthenticateAsync(smtpUser, smtpPass);
                    
                    _logger.LogInformation("Enviando email a {Count} destinatario(s)...", message.To.Count);
                    await client.SendAsync(message);
                    
                    await client.DisconnectAsync(true);
                    
                    _logger.LogInformation("Email enviado exitosamente a {Count} administrador(es)", message.To.Count);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email: {Message}", ex.Message);
                LogSolicitudEnConsola(solicitud, correosAdmins);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            try
            {
                // Configuración SMTP desde appsettings
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "465");
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPass = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? smtpUser;
                var fromName = _configuration["EmailSettings:FromName"] ?? "AutoClick.cr";

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    _logger.LogWarning("Configuración SMTP incompleta. SmtpHost: {Host}, SmtpUser: {User}, SmtpPass: {Pass}", 
                        smtpHost ?? "NULL", 
                        smtpUser ?? "NULL", 
                        string.IsNullOrEmpty(smtpPass) ? "NULL" : "***SET***");
                    return false;
                }

                // Construir la URL de reseteo
                var resetUrl = $"https://autoclick.cr/ResetPassword?token={resetToken}";

                // Crear mensaje con MimeKit
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = "Recuperación de contraseña - AutoClick.cr";
                message.Body = new TextPart("html")
                {
                    Text = GenerarCuerpoEmailResetPassword(resetUrl)
                };

                // Enviar con MailKit
                using (var client = new SmtpClient())
                {
                    _logger.LogInformation("Conectando a SMTP {Host}:{Port} para reset de contraseña...", smtpHost, smtpPort);
                    
                    // Puerto 465 usa SSL directo, puerto 587 usa STARTTLS
                    if (smtpPort == 465)
                    {
                        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.SslOnConnect);
                    }
                    else
                    {
                        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                    }
                    
                    _logger.LogInformation("Autenticando...");
                    await client.AuthenticateAsync(smtpUser, smtpPass);
                    
                    _logger.LogInformation("Enviando email de recuperación a {Email}...", toEmail);
                    await client.SendAsync(message);
                    
                    await client.DisconnectAsync(true);
                    
                    _logger.LogInformation("Email de recuperación enviado exitosamente a {Email}", toEmail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email de recuperación: {Message}", ex.Message);
                return false;
            }
        }

        private string GenerarCuerpoEmail(SolicitudEmpresa solicitud, bool esEspacioPublicitario = false)
        {
            var tituloEncabezado = esEspacioPublicitario 
                ? "🎯 Solicitud de Espacio Publicitario"
                : "🚗 Nueva Solicitud de Empresa";

            var mensajeIntro = esEspacioPublicitario
                ? "Se ha recibido una nueva solicitud de espacio publicitario en AutoClick.cr:"
                : "Se ha recibido una nueva solicitud de empresa interesada en anunciarse en AutoClick.cr:";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Montserrat', Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #02081C; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .header h1 {{ margin: 0; color: #FF931E; }}
        .content {{ background: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .field {{ margin-bottom: 20px; }}
        .field-label {{ font-weight: bold; color: #02081C; margin-bottom: 5px; }}
        .field-value {{ color: #555; padding: 10px; background: white; border-left: 3px solid #FF931E; }}
        .footer {{ background: #02081C; color: white; padding: 15px; text-align: center; border-radius: 0 0 8px 8px; font-size: 12px; }}
        .button {{ display: inline-block; padding: 12px 24px; background: #FF931E; color: white; text-decoration: none; border-radius: 4px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>{tituloEncabezado}</h1>
            <p style='color: white; margin: 10px 0 0 0;'>AutoClick.cr</p>
        </div>
        <div class='content'>
            <p style='font-size: 16px; color: #02081C;'>{mensajeIntro}</p>
            
            <div class='field'>
                <div class='field-label'>🏢 Nombre de la Empresa:</div>
                <div class='field-value'>{solicitud.NombreEmpresa}</div>
            </div>
            
            <div class='field'>
                <div class='field-label'>👤 Representante Legal:</div>
                <div class='field-value'>{solicitud.RepresentanteLegal}</div>
            </div>
            
            <div class='field'>
                <div class='field-label'>🏭 Industria:</div>
                <div class='field-value'>{solicitud.Industria}</div>
            </div>
            
            <div class='field'>
                <div class='field-label'>📧 Correo Electrónico:</div>
                <div class='field-value'><a href='mailto:{solicitud.CorreoElectronico}'>{solicitud.CorreoElectronico}</a></div>
            </div>
            
            <div class='field'>
                <div class='field-label'>📱 Teléfono:</div>
                <div class='field-value'><a href='tel:{solicitud.Telefono}'>{solicitud.Telefono}</a></div>
            </div>
            
            <div class='field'>
                <div class='field-label'>📝 Descripción de la Empresa:</div>
                <div class='field-value'>{solicitud.DescripcionEmpresa}</div>
            </div>
            
            <div class='field'>
                <div class='field-label'>📅 Fecha de Solicitud:</div>
                <div class='field-value'>{solicitud.FechaCreacion:dd/MM/yyyy HH:mm} UTC</div>
            </div>
            
            <p style='margin-top: 30px; padding: 15px; background: #fff3cd; border-left: 4px solid #FF931E;'>
                <strong>⚡ Acción Requerida:</strong><br>
                Por favor, revise esta solicitud y contacte al cliente lo antes posible para discutir las oportunidades de publicidad.
            </p>
        </div>
        <div class='footer'>
            <p style='margin: 0;'>Este correo fue generado automáticamente por el sistema AutoClick.cr</p>
            <p style='margin: 5px 0 0 0;'>© 2025 AutoClick.cr - Todos los derechos reservados</p>
        </div>
    </div>
</body>
</html>";
        }

        private void LogSolicitudEnConsola(SolicitudEmpresa solicitud, List<string> correosAdmins)
        {
            _logger.LogInformation("====================================");
            _logger.LogInformation("NUEVA SOLICITUD DE EMPRESA (Email no configurado - LOG)");
            _logger.LogInformation("====================================");
            _logger.LogInformation("Empresa: {Empresa}", solicitud.NombreEmpresa);
            _logger.LogInformation("Representante: {Rep}", solicitud.RepresentanteLegal);
            _logger.LogInformation("Industria: {Ind}", solicitud.Industria);
            _logger.LogInformation("Email: {Email}", solicitud.CorreoElectronico);
            _logger.LogInformation("Teléfono: {Tel}", solicitud.Telefono);
            _logger.LogInformation("Descripción: {Desc}", solicitud.DescripcionEmpresa);
            _logger.LogInformation("Fecha: {Fecha}", solicitud.FechaCreacion);
            _logger.LogInformation("Destinatarios (admins): {Admins}", string.Join(", ", correosAdmins));
            _logger.LogInformation("====================================");
        }

        private string GenerarCuerpoEmailResetPassword(string resetUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Montserrat', Arial, sans-serif; background-color: #02081C; color: #ffffff; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 40px auto; background-color: #030D2B; border-radius: 12px; padding: 40px; border: 1px solid rgba(255, 255, 255, 0.2); }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .logo {{ color: #FF931E; font-size: 32px; font-weight: 700; margin: 0; }}
        .subtitle {{ color: rgba(255, 255, 255, 0.7); font-size: 14px; margin-top: 8px; }}
        .content {{ margin-bottom: 30px; line-height: 1.8; }}
        .content h2 {{ color: #ffffff; margin-bottom: 20px; font-size: 22px; }}
        .content p {{ color: rgba(255, 255, 255, 0.85); margin-bottom: 16px; }}
        .button-container {{ text-align: center; margin: 30px 0; }}
        .button {{ display: inline-block; background: linear-gradient(135deg, #FF931E, #FF7A00); color: white; text-decoration: none; padding: 16px 40px; border-radius: 8px; font-weight: 600; font-size: 16px; transition: all 0.3s ease; }}
        .button:hover {{ transform: translateY(-2px); box-shadow: 0 8px 25px rgba(255, 147, 30, 0.3); }}
        .url-box {{ background-color: #02081C; padding: 16px; border-radius: 6px; word-break: break-all; border: 1px solid rgba(255, 255, 255, 0.1); margin: 20px 0; }}
        .url-box a {{ color: #FF931E; text-decoration: none; }}
        .warning {{ background-color: rgba(255, 193, 7, 0.15); border-left: 4px solid #FFC107; padding: 16px; margin: 25px 0; border-radius: 4px; }}
        .warning-title {{ color: #FFC107; font-weight: 700; margin-bottom: 8px; }}
        .footer {{ text-align: center; color: rgba(255, 255, 255, 0.5); font-size: 12px; margin-top: 40px; padding-top: 25px; border-top: 1px solid rgba(255, 255, 255, 0.1); }}
        .footer p {{ margin: 8px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 class='logo'>AutoClick.cr</h1>
            <p class='subtitle'>Tu plataforma de compra y venta de autos</p>
        </div>
        <div class='content'>
            <h2>🔐 Recuperación de contraseña</h2>
            <p>Hola,</p>
            <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en <strong>AutoClick.cr</strong>.</p>
            <p>Para crear una nueva contraseña y recuperar el acceso a tu cuenta, haz clic en el siguiente botón:</p>
            <div class='button-container'>
                <a href='{resetUrl}' class='button'>Restablecer mi contraseña</a>
            </div>
            <p style='font-size: 14px; color: rgba(255, 255, 255, 0.7);'>Si el botón no funciona, copia y pega el siguiente enlace en tu navegador:</p>
            <div class='url-box'>
                <a href='{resetUrl}'>{resetUrl}</a>
            </div>
            <div class='warning'>
                <div class='warning-title'>⚠️ Importante - Lee con atención</div>
                <p style='margin: 0; font-size: 14px; color: rgba(255, 255, 255, 0.85);'>
                    • Este enlace es <strong>válido por 1 hora</strong> solamente<br>
                    • Si no solicitaste este cambio, <strong>ignora este correo</strong><br>
                    • Tu contraseña actual permanecerá sin cambios si no usas este enlace<br>
                    • Por seguridad, nunca compartas este enlace con nadie
                </p>
            </div>
            <p style='font-size: 14px; color: rgba(255, 255, 255, 0.7); margin-top: 25px;'>
                Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.
            </p>
        </div>
        <div class='footer'>
            <p><strong>Este es un correo automático, por favor no respondas a este mensaje.</strong></p>
            <p>&copy; 2025 AutoClick.cr - Todos los derechos reservados</p>
            <p style='margin-top: 15px;'>🚗 Encuentra tu próximo auto en AutoClick.cr</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
