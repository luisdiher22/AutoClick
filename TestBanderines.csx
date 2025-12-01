using AutoClick.Models;

// Test de la funcionalidad BanderinesVideoUrls

// Caso 1: Auto con banderines usando slugs
var auto1 = new Auto
{
    Id = 37,
    Marca = "GMC",
    Modelo = "Envoy",
    Ano = 2024,
    BanderinAdquirido = 0,
    BanderinesAdquiridos = "unico-dueno,full-extras"
};

Console.WriteLine("=== Auto 37: GMC Envoy 2024 ===");
Console.WriteLine($"BanderinesAdquiridos: {auto1.BanderinesAdquiridos}");
Console.WriteLine($"BanderinesVideoUrls count: {auto1.BanderinesVideoUrls.Count}");
foreach (var url in auto1.BanderinesVideoUrls)
{
    Console.WriteLine($"  - {url}");
}
Console.WriteLine();

// Caso 2: Auto con banderin legacy (ID numérico)
var auto2 = new Auto
{
    Id = 8,
    Marca = "toyota",
    Modelo = "prius",
    Ano = 2018,
    BanderinAdquirido = 1,
    BanderinesAdquiridos = ""
};

Console.WriteLine("=== Auto 8: Toyota Prius 2018 ===");
Console.WriteLine($"BanderinAdquirido: {auto2.BanderinAdquirido}");
Console.WriteLine($"BanderinesVideoUrls count: {auto2.BanderinesVideoUrls.Count}");
foreach (var url in auto2.BanderinesVideoUrls)
{
    Console.WriteLine($"  - {url}");
}
Console.WriteLine();

// Caso 3: Auto con banderines mixtos (slugs)
var auto3 = new Auto
{
    Id = 33,
    Marca = "Acura",
    Modelo = "Integra",
    Ano = 2025,
    BanderinAdquirido = 0,
    BanderinesAdquiridos = "version-americana,unico-dueno"
};

Console.WriteLine("=== Auto 33: Acura Integra 2025 ===");
Console.WriteLine($"BanderinesAdquiridos: {auto3.BanderinesAdquiridos}");
Console.WriteLine($"BanderinesVideoUrls count: {auto3.BanderinesVideoUrls.Count}");
foreach (var url in auto3.BanderinesVideoUrls)
{
    Console.WriteLine($"  - {url}");
}

Console.WriteLine("\n✅ Test completado");
