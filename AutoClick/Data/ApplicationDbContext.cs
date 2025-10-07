using Microsoft.EntityFrameworkCore;
using AutoClick.Models;

namespace AutoClick.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add your DbSets here
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Auto> Autos { get; set; }
    // Car model eliminado
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Usuario entity
        modelBuilder.Entity<Usuario>(entity =>
        {
            // Email como primary key
            entity.HasKey(u => u.Email);
            
            // Índices para mejorar performance
            entity.HasIndex(u => u.NumeroTelefono);
            entity.HasIndex(u => u.NombreAgencia);
            
            // Configuraciones adicionales
            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(150);
                  
            entity.Property(u => u.Nombre)
                  .IsRequired()
                  .HasMaxLength(50);
                  
            entity.Property(u => u.Apellidos)
                  .IsRequired()
                  .HasMaxLength(100);
                  
            entity.Property(u => u.NumeroTelefono)
                  .IsRequired()
                  .HasMaxLength(20);
                  
            entity.Property(u => u.Contrasena)
                  .IsRequired()
                  .HasMaxLength(100);
                  
            entity.Property(u => u.NombreAgencia)
                  .HasMaxLength(100);
        });
        
        // Configure Auto entity
        modelBuilder.Entity<Auto>(entity =>
        {
            // Primary key
            entity.HasKey(a => a.Id);
            
            // Índices para mejorar performance
            entity.HasIndex(a => a.Marca);
            entity.HasIndex(a => a.Modelo);
            entity.HasIndex(a => a.Ano);
            // Índice único que permite múltiples valores null
            entity.HasIndex(a => a.PlacaVehiculo)
                  .IsUnique()
                  .HasFilter("PlacaVehiculo IS NOT NULL AND PlacaVehiculo != ''");
            entity.HasIndex(a => a.Provincia);
            entity.HasIndex(a => a.Canton);
            entity.HasIndex(a => a.EmailPropietario);
            entity.HasIndex(a => a.FechaCreacion);
            entity.HasIndex(a => a.Activo);
            
            // Configuraciones de propiedades
            entity.Property(a => a.Marca)
                  .IsRequired()
                  .HasMaxLength(50);
                  
            entity.Property(a => a.Modelo)
                  .IsRequired()
                  .HasMaxLength(100);
                  
            entity.Property(a => a.PlacaVehiculo)
                  .HasMaxLength(20);
                  
            entity.Property(a => a.ValorFiscal)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();
                  
            entity.Property(a => a.EmailPropietario)
                  .IsRequired()
                  .HasMaxLength(150);
            
            // Relación con Usuario
            entity.HasOne(a => a.Propietario)
                  .WithMany()
                  .HasForeignKey(a => a.EmailPropietario)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Configure your other entity relationships here
    }
}