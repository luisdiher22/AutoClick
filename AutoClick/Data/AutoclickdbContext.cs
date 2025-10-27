using System;
using System.Collections.Generic;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Data;

public partial class AutoclickdbContext : DbContext
{
    public AutoclickdbContext(DbContextOptions<AutoclickdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnunciosPublicidad> AnunciosPublicidad { get; set; }

    public virtual DbSet<Autos> Autos { get; set; }

    public virtual DbSet<EmpresasPublicidad> EmpresasPublicidad { get; set; }

    public virtual DbSet<Favoritos> Favoritos { get; set; }

    public virtual DbSet<Mensajes> Mensajes { get; set; }

    public virtual DbSet<Reclamos> Reclamos { get; set; }

    public virtual DbSet<SolicitudesEmpresa> SolicitudesEmpresa { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<VentasExternas> VentasExternas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnunciosPublicidad>(entity =>
        {
            entity.HasIndex(e => e.Activo, "IX_AnunciosPublicidad_Activo");

            entity.HasIndex(e => e.EmpresaPublicidadId, "IX_AnunciosPublicidad_EmpresaPublicidadId");

            entity.HasIndex(e => e.FechaPublicacion, "IX_AnunciosPublicidad_FechaPublicacion");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.UrlDestino).HasMaxLength(500);
            entity.Property(e => e.UrlImagen).HasMaxLength(500);

            entity.HasOne(d => d.EmpresaPublicidad).WithMany(p => p.AnunciosPublicidad).HasForeignKey(d => d.EmpresaPublicidadId);
        });

        modelBuilder.Entity<Autos>(entity =>
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
            entity.Property(e => e.VideosUrls).HasColumnType("text");

            entity.HasOne(d => d.EmailPropietarioNavigation).WithMany(p => p.Autos)
                .HasForeignKey(d => d.EmailPropietario)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmpresasPublicidad>(entity =>
        {
            entity.HasIndex(e => e.Activa, "IX_EmpresasPublicidad_Activa");

            entity.HasIndex(e => e.FechaInicio, "IX_EmpresasPublicidad_FechaInicio");

            entity.HasIndex(e => e.NombreEmpresa, "IX_EmpresasPublicidad_NombreEmpresa");

            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.EstadoCobros)
                .HasMaxLength(20)
                .HasDefaultValue("Al día");
            entity.Property(e => e.NombreEmpresa).HasMaxLength(200);
        });

        modelBuilder.Entity<Favoritos>(entity =>
        {
            entity.HasIndex(e => e.AutoId, "IX_Favoritos_AutoId");

            entity.HasIndex(e => e.EmailUsuario, "IX_Favoritos_EmailUsuario");

            entity.HasIndex(e => new { e.EmailUsuario, e.AutoId }, "IX_Favoritos_EmailUsuario_AutoId").IsUnique();

            entity.HasIndex(e => e.FechaCreacion, "IX_Favoritos_FechaCreacion");

            entity.Property(e => e.EmailUsuario).HasMaxLength(150);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Auto).WithMany(p => p.Favoritos).HasForeignKey(d => d.AutoId);

            entity.HasOne(d => d.EmailUsuarioNavigation).WithMany(p => p.Favoritos).HasForeignKey(d => d.EmailUsuario);
        });

        modelBuilder.Entity<Mensajes>(entity =>
        {
            entity.HasIndex(e => e.EmailCliente, "IX_Mensajes_EmailCliente");

            entity.HasIndex(e => e.Estado, "IX_Mensajes_Estado");

            entity.HasIndex(e => new { e.Estado, e.FechaCreacion }, "IX_Mensajes_Estado_FechaCreacion");

            entity.HasIndex(e => e.FechaCreacion, "IX_Mensajes_FechaCreacion");

            entity.HasIndex(e => e.Prioridad, "IX_Mensajes_Prioridad");

            entity.HasIndex(e => e.TipoConsulta, "IX_Mensajes_TipoConsulta");

            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Asunto).HasMaxLength(100);
            entity.Property(e => e.ContenidoMensaje).HasMaxLength(1000);
            entity.Property(e => e.EmailAdminRespuesta).HasMaxLength(150);
            entity.Property(e => e.EmailCliente).HasMaxLength(150);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Prioridad)
                .HasMaxLength(50)
                .HasDefaultValue("Media");
            entity.Property(e => e.RespuestaAdmin).HasMaxLength(1000);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.TipoConsulta).HasMaxLength(50);
        });

        modelBuilder.Entity<Reclamos>(entity =>
        {
            entity.HasIndex(e => e.EmailCliente, "IX_Reclamos_EmailCliente");

            entity.HasIndex(e => e.Estado, "IX_Reclamos_Estado");

            entity.HasIndex(e => new { e.Estado, e.FechaCreacion }, "IX_Reclamos_Estado_FechaCreacion");

            entity.HasIndex(e => e.FechaCreacion, "IX_Reclamos_FechaCreacion");

            entity.HasIndex(e => e.Prioridad, "IX_Reclamos_Prioridad");

            entity.HasIndex(e => e.TipoProblema, "IX_Reclamos_TipoProblema");

            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Asunto).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.EmailAdminRespuesta).HasMaxLength(150);
            entity.Property(e => e.EmailCliente).HasMaxLength(150);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Prioridad)
                .HasMaxLength(50)
                .HasDefaultValue("Media");
            entity.Property(e => e.RespuestaAdmin).HasMaxLength(1000);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.TipoProblema).HasMaxLength(50);
        });

        modelBuilder.Entity<SolicitudesEmpresa>(entity =>
        {
            entity.HasIndex(e => e.CorreoElectronico, "IX_SolicitudesEmpresa_CorreoElectronico");

            entity.HasIndex(e => e.Estado, "IX_SolicitudesEmpresa_Estado");

            entity.HasIndex(e => e.FechaCreacion, "IX_SolicitudesEmpresa_FechaCreacion");

            entity.HasIndex(e => e.NombreEmpresa, "IX_SolicitudesEmpresa_NombreEmpresa");

            entity.Property(e => e.CorreoElectronico).HasMaxLength(150);
            entity.Property(e => e.DescripcionEmpresa).HasMaxLength(1000);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Industria).HasMaxLength(50);
            entity.Property(e => e.NombreEmpresa).HasMaxLength(100);
            entity.Property(e => e.NotasInternas).HasMaxLength(500);
            entity.Property(e => e.RepresentanteLegal).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.HasIndex(e => e.NombreAgencia, "IX_Usuarios_NombreAgencia");

            entity.HasIndex(e => e.NumeroTelefono, "IX_Usuarios_NumeroTelefono");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Contrasena).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.NombreAgencia).HasMaxLength(100);
            entity.Property(e => e.NumeroTelefono).HasMaxLength(20);
        });

        modelBuilder.Entity<VentasExternas>(entity =>
        {
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.Marca).HasMaxLength(100);
            entity.Property(e => e.Modelo).HasMaxLength(100);
            entity.Property(e => e.Placa).HasMaxLength(20);
            entity.Property(e => e.PrecioVenta).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PromedioValorFiscal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PromedioValorMercado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ValorFiscal).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
