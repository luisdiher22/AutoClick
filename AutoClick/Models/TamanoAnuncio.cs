namespace AutoClick.Models
{
    public enum TamanoAnuncio
    {
        Horizontal = 1,      // 1010x189 px
        MedioVertical = 2,   // 401x287 px
        GrandeVertical = 3   // 344x423 px
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
                _ => (1010, 189)
            };
        }

        public static string GetDisplayName(this TamanoAnuncio tamano)
        {
            return tamano switch
            {
                TamanoAnuncio.Horizontal => "Horizontal (1010x189px)",
                TamanoAnuncio.MedioVertical => "Medio Vertical (401x287px)",
                TamanoAnuncio.GrandeVertical => "Grande Vertical (344x423px)",
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
                _ => "anuncio-horizontal"
            };
        }
    }
}
