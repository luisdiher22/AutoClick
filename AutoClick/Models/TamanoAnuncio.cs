namespace AutoClick.Models
{
    public enum TamanoAnuncio
    {
        Horizontal = 1,              // 1010x189 px (Desktop)
        MedioVertical = 2,           // 401x287 px
        GrandeVertical = 3,          // 344x423 px (Desktop)
        MobileHorizontal = 4,        // 291x120 px (Móvil horizontal)
        MobileGrandeVertical = 5,    // 291x180 px (Móvil grande vertical)
        TabletHorizontal = 6,        // 550x217 px (Tablet horizontal)
        TabletGrandeVertical = 7     // 340x373 px (Tablet grande vertical)
    }

    public static class TamanoAnuncioExtensions
    {
        public static (int Width, int Height) GetDimensions(this TamanoAnuncio tamano)
        {
            return tamano switch
            {
                TamanoAnuncio.Horizontal => (1010, 189),
                TamanoAnuncio.MedioVertical => (401, 287),
                TamanoAnuncio.GrandeVertical => (344, 423),
                TamanoAnuncio.MobileHorizontal => (291, 120),
                TamanoAnuncio.MobileGrandeVertical => (291, 180),
                TamanoAnuncio.TabletHorizontal => (550, 217),
                TamanoAnuncio.TabletGrandeVertical => (340, 373),
                _ => (1010, 189)
            };
        }

        public static string GetDisplayName(this TamanoAnuncio tamano)
        {
            return tamano switch
            {
                TamanoAnuncio.Horizontal => "Horizontal Desktop (1010x189px)",
                TamanoAnuncio.MedioVertical => "Medio Vertical (401x287px)",
                TamanoAnuncio.GrandeVertical => "Grande Vertical Desktop (344x423px)",
                TamanoAnuncio.MobileHorizontal => "Horizontal Móvil (291x120px)",
                TamanoAnuncio.MobileGrandeVertical => "Grande Vertical Móvil (291x180px)",
                TamanoAnuncio.TabletHorizontal => "Horizontal Tablet (550x217px)",
                TamanoAnuncio.TabletGrandeVertical => "Grande Vertical Tablet (340x373px)",
                _ => "Desconocido"
            };
        }

        public static string GetCssClass(this TamanoAnuncio tamano)
        {
            return tamano switch
            {
                TamanoAnuncio.Horizontal => "anuncio-horizontal",
                TamanoAnuncio.MedioVertical => "anuncio-medio-vertical",
                TamanoAnuncio.GrandeVertical => "anuncio-grande-vertical",
                TamanoAnuncio.MobileHorizontal => "anuncio-mobile-horizontal",
                TamanoAnuncio.MobileGrandeVertical => "anuncio-mobile-grande-vertical",
                TamanoAnuncio.TabletHorizontal => "anuncio-tablet-horizontal",
                TamanoAnuncio.TabletGrandeVertical => "anuncio-tablet-grande-vertical",
                _ => "anuncio-horizontal"
            };
        }

        public static string GetDeviceType(this TamanoAnuncio tamano)
        {
            return tamano switch
            {
                TamanoAnuncio.Horizontal => "Desktop",
                TamanoAnuncio.MedioVertical => "Desktop",
                TamanoAnuncio.GrandeVertical => "Desktop",
                TamanoAnuncio.MobileHorizontal => "Móvil",
                TamanoAnuncio.MobileGrandeVertical => "Móvil",
                TamanoAnuncio.TabletHorizontal => "Tablet",
                TamanoAnuncio.TabletGrandeVertical => "Tablet",
                _ => "Desktop"
            };
        }
    }
}
