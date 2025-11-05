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
    public DbSet<SolicitudEmpresa> SolicitudesEmpresa { get; set; }
    public DbSet<Favorito> Favoritos { get; set; }
    public DbSet<EmpresaPublicidad> EmpresasPublicidad { get; set; }
    public DbSet<AnuncioPublicidad> AnunciosPublicidad { get; set; }
    public DbSet<SolicitudPreAprobacion> SolicitudesPreAprobacion { get; set; }
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
                  .IsRequired(false) // Allow null for foreign key
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
            
            // Ignorar relaciones con Usuario para evitar problemas de FK
            entity.Ignore(r => r.Cliente);
            entity.Ignore(r => r.AdminRespuesta);
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
                  .IsRequired(false) // Allow null for foreign key
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
            
            // Ignorar relaciones con Usuario para evitar problemas de FK
            entity.Ignore(m => m.Cliente);
            entity.Ignore(m => m.AdminRespuesta);
        });
        
        // Configure SolicitudEmpresa entity
        modelBuilder.Entity<SolicitudEmpresa>(entity =>
        {
            // Primary key
            entity.HasKey(s => s.Id);
            
            // Índices para mejorar performance
            entity.HasIndex(s => s.CorreoElectronico);
            entity.HasIndex(s => s.FechaCreacion);
            entity.HasIndex(s => s.Estado);
            entity.HasIndex(s => s.NombreEmpresa);
            
            // Configuraciones de propiedades
            entity.Property(s => s.NombreEmpresa)
                  .IsRequired()
                  .HasMaxLength(100);
                  
            entity.Property(s => s.RepresentanteLegal)
                  .IsRequired()
                  .HasMaxLength(100);
                  
            entity.Property(s => s.Industria)
                  .IsRequired()
                  .HasMaxLength(50);
                  
            entity.Property(s => s.CorreoElectronico)
                  .IsRequired()
                  .HasMaxLength(150);
                  
            entity.Property(s => s.Telefono)
                  .IsRequired()
                  .HasMaxLength(20);
                  
            entity.Property(s => s.DescripcionEmpresa)
                  .IsRequired()
                  .HasMaxLength(1000);
                  
            entity.Property(s => s.Estado)
                  .IsRequired()
                  .HasMaxLength(20)
                  .HasDefaultValue("Pendiente");
                  
            entity.Property(s => s.NotasInternas)
                  .HasMaxLength(500);
                  
            entity.Property(s => s.FechaCreacion)
                  .IsRequired()
                  .HasDefaultValueSql("GETUTCDATE()");
        });
        
        // Configure Favorito entity
        modelBuilder.Entity<Favorito>(entity =>
        {
            // Primary key
            entity.HasKey(f => f.Id);
            
            // Índice único compuesto para evitar duplicados
            entity.HasIndex(f => new { f.EmailUsuario, f.AutoId })
                  .IsUnique()
                  .HasDatabaseName("IX_Favoritos_EmailUsuario_AutoId");
            
            // Índices para mejorar performance
            entity.HasIndex(f => f.EmailUsuario);
            entity.HasIndex(f => f.AutoId);
            entity.HasIndex(f => f.FechaCreacion);
            
            // Configuraciones de propiedades
            entity.Property(f => f.EmailUsuario)
                  .IsRequired()
                  .HasMaxLength(150);
                  
            entity.Property(f => f.AutoId)
                  .IsRequired();
                  
            entity.Property(f => f.FechaCreacion)
                  .IsRequired()
                  .HasDefaultValueSql("GETUTCDATE()");
            
            // Relaciones
            entity.HasOne(f => f.Usuario)
                  .WithMany()
                  .HasForeignKey(f => f.EmailUsuario)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(f => f.Auto)
                  .WithMany()
                  .HasForeignKey(f => f.AutoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure EmpresaPublicidad entity
        modelBuilder.Entity<EmpresaPublicidad>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.NombreEmpresa);
            entity.HasIndex(e => e.Activa);
            entity.HasIndex(e => e.FechaInicio);
            
            entity.Property(e => e.NombreEmpresa)
                  .IsRequired()
                  .HasMaxLength(200);
                  
            entity.Property(e => e.FechaInicio)
                  .IsRequired();
                  
            entity.Property(e => e.EstadoCobros)
                  .IsRequired()
                  .HasMaxLength(20)
                  .HasDefaultValue("Al día");
                  
            entity.Property(e => e.Activa)
                  .IsRequired()
                  .HasDefaultValue(true);
        });
        
        // Configure AnuncioPublicidad entity
        modelBuilder.Entity<AnuncioPublicidad>(entity =>
        {
            entity.HasKey(a => a.Id);
            
            entity.HasIndex(a => a.EmpresaPublicidadId);
            entity.HasIndex(a => a.Activo);
            entity.HasIndex(a => a.FechaPublicacion);
            
            entity.Property(a => a.UrlImagen)
                  .IsRequired()
                  .HasMaxLength(500);
                  
            entity.Property(a => a.FechaPublicacion)
                  .IsRequired();
                  
            entity.Property(a => a.NumeroVistas)
                  .HasDefaultValue(0);
                  
            entity.Property(a => a.NumeroClics)
                  .HasDefaultValue(0);
                  
            entity.Property(a => a.Activo)
                  .IsRequired()
                  .HasDefaultValue(true);
            
            // Relación con EmpresaPublicidad
            entity.HasOne(a => a.EmpresaPublicidad)
                  .WithMany(e => e.Anuncios)
                  .HasForeignKey(a => a.EmpresaPublicidadId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure your other entity relationships here
    }
}