using System.Collections.Generic;

namespace AutoClick.Helpers
{
    /// <summary>
    /// Helper para convertir valores técnicos de extras a nombres descriptivos legibles
    /// </summary>
    public static class ExtrasHelper
    {
        private static readonly Dictionary<string, string> ExtrasDisplayNames = new Dictionary<string, string>
        {
            // Equipamiento Exterior
            { "faros-led", "Faros LED / Xenón / Halógenos" },
            { "luces-diurnas", "Luces diurnas (DRL)" },
            { "luces-automaticas", "Luces automáticas" },
            { "retrovisores-retractiles", "Retrovisores Auto-Retractibles" },
            { "aros-lujo", "Aros de lujo" },
            { "aleron-trasero", "Alerón trasero" },
            { "sensor-lluvia", "Sensor de lluvia" },
            { "vidrios-polarizados", "Vidrios polarizados" },
            { "techo-solar", "Techo solar / panorámico" },
            { "racks", "Racks" },
            { "estribos-laterales", "Estribos laterales" },
            
            // Equipamiento Interior
            { "asientos-cuero", "Asientos en cuero" },
            { "asientos-memoria", "Asientos con memoria" },
            { "asientos-electricos", "Asientos eléctricos" },
            { "encendido-remoto", "Encendido remoto" },
            { "aire-acondicionado", "Aire acondicionado" },
            { "boton-arranque", "Botón de arranque" },
            { "llave-inteligente", "Llave inteligente" },
            { "volante-multifuncion", "Volante multifunción" },
            { "volante-ajustable", "Volante ajustable eléctricamente" },
            { "iluminacion-ambiental", "Iluminación ambiental" },
            { "persianas-traseras", "Persianas traseras" },
            { "vidrios-electricos", "Vidrios eléctricos" },
            { "maletero-electrico", "Maletero eléctrico" },
            
            // Multimedia
            { "cargador-inalambrico", "Cargador inalámbrico" },
            { "pantalla-tactil", "Pantalla táctil multimedia" },
            { "android-carplay", "Android Auto / Apple CarPlay" },
            { "navegacion-gps", "Navegación GPS" },
            { "bluetooth-usb", "Bluetooth / USB / AUX" },
            { "sistema-sonido", "Sistema de sonido premium" },
            { "head-up-display", "Head-Up Display" },
            { "cluster-digital", "Clúster digital" },
            { "wifi", "Conectividad WiFi" },
            { "control-voz", "Control por voz" },
            { "control-gestual", "Control gestual" },
            { "tomacorrientes", "Tomacorriente 12V / USB-C / tomas domésticas" },
            
            // Seguridad
            { "camaras-parking", "Cámaras de parking" },
            { "frenos-abs", "Frenos ABS" },
            { "control-traccion", "Control de tracción" },
            { "control-estabilidad", "Control de estabilidad" },
            { "arranque-pendientes", "Asistente de arranque en pendientes" },
            { "freno-emergencia", "Freno automático de emergencia" },
            { "colision-frontal", "Advertencia de colisión frontal" },
            { "deteccion-peatones", "Detección de peatones / ciclistas" },
            { "mantenimiento-carril", "Asistente de mantenimiento de carril" },
            { "airbags", "Airbags" },
            { "punto-ciego", "Alerta de punto ciego" },
            { "monitor-presion", "Monitor de presión de neumáticos" },
            
            // Rendimiento
            { "modos-manejo", "Modos de manejo" },
            { "direccion-asistida", "Dirección asistida" },
            { "turbo", "Turbo/Twin Turbo/Biturbo" },
            { "supercargado", "Supercargado" },
            { "suspension-adaptativa", "Suspensión adaptativa" },
            { "freno-regenerativo", "Sistema de freno regenerativo" },
            
            // Antirrobo
            { "alarma", "Alarma" },
            { "cierre-central", "Cierre central" },
            { "gps-tracker", "GPS" },
            { "bloqueo-distancia", "Bloqueo a distancia" }
        };

        /// <summary>
        /// Convierte un valor técnico de extra a su nombre descriptivo
        /// </summary>
        /// <param name="technicalValue">Valor técnico (ej: "faros-led")</param>
        /// <returns>Nombre descriptivo (ej: "Faros LED / Xenón / Halógenos")</returns>
        public static string GetDisplayName(string technicalValue)
        {
            if (string.IsNullOrWhiteSpace(technicalValue))
                return string.Empty;

            // Buscar en el diccionario
            if (ExtrasDisplayNames.TryGetValue(technicalValue.ToLower().Trim(), out var displayName))
            {
                return displayName;
            }

            // Si no se encuentra, capitalizar el valor técnico como fallback
            return CapitalizeWords(technicalValue.Replace("-", " "));
        }

        /// <summary>
        /// Capitaliza cada palabra de un string
        /// </summary>
        private static string CapitalizeWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = char.ToUpper(words[i][0]) + (words[i].Length > 1 ? words[i].Substring(1).ToLower() : "");
                }
            }
            return string.Join(" ", words);
        }
    }
}
