using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.TempModels;

public partial class AutoclickContext : DbContext
{
    public AutoclickContext()
    {
    }

    public AutoclickContext(DbContextOptions<AutoclickContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auto> Autos { get; set; }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=autoclick.db");

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

            entity.HasIndex(e => e.Modelo, "IX_Autos_Modelo");

            entity.HasIndex(e => e.PlacaVehiculo, "IX_Autos_PlacaVehiculo").IsUnique();

            entity.HasIndex(e => e.Provincia, "IX_Autos_Provincia");

            entity.Property(e => e.ValorFiscal).HasColumnType("decimal(18,2)");

            entity.HasOne(d => d.EmailPropietarioNavigation).WithMany(p => p.Autos)
                .HasForeignKey(d => d.EmailPropietario)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.HasIndex(e => e.NombreAgencia, "IX_Usuarios_NombreAgencia");

            entity.HasIndex(e => e.NumeroTelefono, "IX_Usuarios_NumeroTelefono");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
