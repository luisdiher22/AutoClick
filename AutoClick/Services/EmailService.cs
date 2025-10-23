using System.Net;
using System.Net.Mail;
using AutoClick.Models;

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
        Task<bool> EnviarNotificacionSolicitudEmpresaAsync(SolicitudEmpresa solicitud, List<string> correosAdmins);
    }

    /// <summary>
    /// Servicio de envío de correos electrónicos usando SMTP
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

        public async Task<bool> EnviarNotificacionSolicitudEmpresaAsync(SolicitudEmpresa solicitud, List<string> correosAdmins)
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
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPass = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? smtpUser;
                var fromName = _configuration["EmailSettings:FromName"] ?? "AutoClick.cr";

                // Validar configuración
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    _logger.LogWarning("Configuración SMTP incompleta. Email no enviado.");
                    // En desarrollo, logear la información en lugar de fallar
                    LogSolicitudEnConsola(solicitud, correosAdmins);
                    return true; // Retornar true para no bloquear el flujo en desarrollo
                }

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail!, fromName),
                        Subject = $"Nueva Solicitud de Empresa - {solicitud.NombreEmpresa}",
                        Body = GenerarCuerpoEmail(solicitud),
                        IsBodyHtml = true
                    };

                    // Agregar todos los administradores como destinatarios
                    foreach (var email in correosAdmins)
                    {
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            mailMessage.To.Add(email);
                        }
                    }

                    if (mailMessage.To.Count == 0)
                    {
                        _logger.LogWarning("No hay destinatarios válidos para enviar el correo");
                        return false;
                    }

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email enviado exitosamente a {mailMessage.To.Count} administrador(es)");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar email: {ex.Message}");
                // En desarrollo, logear en consola para no bloquear
                LogSolicitudEnConsola(solicitud, correosAdmins);
                return true; // Retornar true en desarrollo para no bloquear el flujo
            }
        }

        private string GenerarCuerpoEmail(SolicitudEmpresa solicitud)
        {
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
            <h1>🚗 Nueva Solicitud de Empresa</h1>
            <p style='color: white; margin: 10px 0 0 0;'>AutoClick.cr</p>
        </div>
        <div class='content'>
            <p style='font-size: 16px; color: #02081C;'>Se ha recibido una nueva solicitud de empresa interesada en anunciarse en AutoClick.cr:</p>
            
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
            _logger.LogInformation($"Empresa: {solicitud.NombreEmpresa}");
            _logger.LogInformation($"Representante: {solicitud.RepresentanteLegal}");
            _logger.LogInformation($"Industria: {solicitud.Industria}");
            _logger.LogInformation($"Email: {solicitud.CorreoElectronico}");
            _logger.LogInformation($"Teléfono: {solicitud.Telefono}");
            _logger.LogInformation($"Descripción: {solicitud.DescripcionEmpresa}");
            _logger.LogInformation($"Fecha: {solicitud.FechaCreacion}");
            _logger.LogInformation($"Destinatarios (admins): {string.Join(", ", correosAdmins)}");
            _logger.LogInformation("====================================");
        }
    }
}
