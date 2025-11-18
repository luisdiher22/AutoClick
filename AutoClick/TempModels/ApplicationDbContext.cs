using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.TempModels;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auto> Autos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auto>(entity =>
        {
            entity.HasIndex(e => e.Activo, "IX_Autos_Activo");

            entity.HasIndex(e => e.Ano, "IX_Autos_Ano");

            entity.HasIndex(e => e.Canton, "IX_Autos_Canton");

            entity.HasIndex(e => e.EmailPropietario, "IX_Autos_EmailPropietario");

            entity.HasIndex(e => e.FechaCreacion, "IX_Autos_FechaCreacion");

            entity.HasIndex(e => e.Marca, "IX_Autos_Marca");

            entity.HasIndex(e => new { e.Marca, e.Modelo, e.Ano }, "IX_Autos_Marca_Modelo_Ano");

            entity.HasIndex(e => e.Modelo, "IX_Autos_Modelo");

            entity.HasIndex(e => e.PlacaVehiculo, "IX_Autos_PlacaVehiculo")
                .IsUnique()
                .HasFilter("([PlacaVehiculo] IS NOT NULL AND [PlacaVehiculo]<>'')");

            entity.HasIndex(e => e.Provincia, "IX_Autos_Provincia");

            entity.HasIndex(e => new { e.Provincia, e.Canton }, "IX_Autos_Provincia_Canton");

            entity.Property(e => e.Canton).HasMaxLength(50);
            entity.Property(e => e.Carroceria).HasMaxLength(50);
            entity.Property(e => e.Cilindrada).HasMaxLength(20);
            entity.Property(e => e.ColorExterior).HasMaxLength(30);
            entity.Property(e => e.ColorInterior).HasMaxLength(30);
            entity.Property(e => e.Combustible).HasMaxLength(30);
            entity.Property(e => e.Condicion).HasMaxLength(30);
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Divisa).HasMaxLength(5);
            entity.Property(e => e.EmailPropietario).HasMaxLength(150);
            entity.Property(e => e.EstadoAgendaFotografia)
                .HasMaxLength(20)
                .HasDefaultValue("");
            entity.Property(e => e.ExtrasAntiRobo).HasColumnType("text");
            entity.Property(e => e.ExtrasExterior).HasColumnType("text");
            entity.Property(e => e.ExtrasInterior).HasColumnType("text");
            entity.Property(e => e.ExtrasMultimedia).HasColumnType("text");
            entity.Property(e => e.ExtrasRendimiento).HasColumnType("text");
            entity.Property(e => e.ExtrasSeguridad).HasColumnType("text");
            entity.Property(e => e.ImagenPrincipal).HasMaxLength(500);
            entity.Property(e => e.ImagenesUrls).HasColumnType("text");
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Modelo).HasMaxLength(100);
            entity.Property(e => e.PlacaVehiculo).HasMaxLength(20);
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Provincia).HasMaxLength(50);
            entity.Property(e => e.Traccion).HasMaxLength(30);
            entity.Property(e => e.Transmision).HasMaxLength(30);
            entity.Property(e => e.UbicacionExacta).HasMaxLength(200);
            entity.Property(e => e.UnidadKilometraje)
                .HasMaxLength(10)
                .HasDefaultValue("");
            entity.Property(e => e.VideosUrls).HasColumnType("text");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
