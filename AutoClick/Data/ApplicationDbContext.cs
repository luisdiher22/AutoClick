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
    public DbSet<Reclamo> Reclamos { get; set; }
    public DbSet<Mensaje> Mensajes { get; set; }
    public DbSet<VentaExterna> VentasExternas { get; set; }
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
        
        // Configure Reclamo entity
        modelBuilder.Entity<Reclamo>(entity =>
        {
            // Primary key
            entity.HasKey(r => r.Id);
            
            // Índices para mejorar performance
            entity.HasIndex(r => r.EmailCliente);
            entity.HasIndex(r => r.FechaCreacion);
            entity.HasIndex(r => r.Estado);
            entity.HasIndex(r => r.TipoProblema);
            entity.HasIndex(r => r.Prioridad);
            
            // Configuraciones de propiedades
            entity.Property(r => r.EmailCliente)
                  .IsRequired()
                  .HasMaxLength(150);
                  
            entity.Property(r => r.Nombre)
                  .IsRequired()
                  .HasMaxLength(50);
                  
            entity.Property(r => r.Apellidos)
                  .IsRequired()
                  .HasMaxLength(100);
                  
            entity.Property(r => r.TipoProblema)
                  .IsRequired()
                  .HasMaxLength(50);
                  
            entity.Property(r => r.Asunto)
                  .IsRequired()
                  .HasMaxLength(100);
                  
            entity.Property(r => r.Descripcion)
                  .IsRequired()
                  .HasMaxLength(1000);
                  
            entity.Property(r => r.Prioridad)
                  .HasMaxLength(50)
                  .HasDefaultValue("Media");
                  
            entity.Property(r => r.RespuestaAdmin)
                  .HasMaxLength(1000);
                  
            entity.Property(r => r.EmailAdminRespuesta)
                  .HasMaxLength(150);
            
            // Relaciones con Usuario (opcional)
            entity.HasOne(r => r.Cliente)
                  .WithMany()
                  .HasForeignKey(r => r.EmailCliente)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasOne(r => r.AdminRespuesta)
                  .WithMany()
                  .HasForeignKey(r => r.EmailAdminRespuesta)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configure Mensaje entity
        modelBuilder.Entity<Mensaje>(entity =>
        {
            // Primary key
            entity.HasKey(m => m.Id);
            
            // Índices para mejorar performance
            entity.HasIndex(m => m.EmailCliente);
            entity.HasIndex(m => m.FechaCreacion);
            entity.HasIndex(m => m.Estado);
            entity.HasIndex(m => m.TipoConsulta);
            entity.HasIndex(m => m.Prioridad);
            
            // Configuraciones de propiedades
            entity.Property(m => m.EmailCliente)
                  .IsRequired()
                  .HasMaxLength(150);
                  
            entity.Property(m => m.Nombre)
                  .IsRequired()
                  .HasMaxLength(50);
                  
            entity.Property(m => m.Apellidos)
                  .IsRequired()
                  .HasMaxLength(100);
                  
            entity.Property(m => m.TipoConsulta)
                  .IsRequired()
                  .HasMaxLength(50);
                  
            entity.Property(m => m.Asunto)
                  .IsRequired()
                  .HasMaxLength(100);
                  
            entity.Property(m => m.ContenidoMensaje)
                  .IsRequired()
                  .HasMaxLength(1000);
                  
            entity.Property(m => m.Telefono)
                  .HasMaxLength(20);
                  
            entity.Property(m => m.Prioridad)
                  .HasMaxLength(50)
                  .HasDefaultValue("Media");
                  
            entity.Property(m => m.RespuestaAdmin)
                  .HasMaxLength(1000);
                  
            entity.Property(m => m.EmailAdminRespuesta)
                  .HasMaxLength(150);
            
            // Relaciones con Usuario (opcional) - Sin restricciones de clave foránea
            entity.HasOne(m => m.Cliente)
                  .WithMany()
                  .HasForeignKey(m => m.EmailCliente)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasOne(m => m.AdminRespuesta)
                  .WithMany()
                  .HasForeignKey(m => m.EmailAdminRespuesta)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configure your other entity relationships here
    }
}