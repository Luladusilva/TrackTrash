using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TrackTrash.Domain.Entities;

namespace TrackTrash.Infraestructure.Context;

public partial class TrackTrashDBContext : DbContext
{
    public TrackTrashDBContext()
    {
    }

    public TrackTrashDBContext(DbContextOptions<TrackTrashDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<DetailRequest> DetailRequests { get; set; }

    public virtual DbSet<HistoryRequest> HistoryRequests { get; set; }

    public virtual DbSet<RecoveredMaterial> RecoveredMaterials { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<CollectionPoint> CollectionPoints { get; set; }

    public virtual DbSet<Gatherer> Gatherers { get; set; }

    public virtual DbSet<PickupService> PickupServices { get; set; }

    public virtual DbSet<CollectRequest> CollectRequests { get; set; }

    public virtual DbSet<ResidueType> ResidueTypes { get; set; }

    public virtual DbSet<Ubication> Ubications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<GatherersGrades> GatherersGrades { get; set; }

    public virtual DbSet<Indicators> Indicators { get; set; }

    public virtual DbSet<ResiduesByType> ResiduesByType { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.CalificacionId).HasName("calificaciones_pkey");

            entity.ToTable("calificaciones");

            entity.HasIndex(e => new { e.ServicioId, e.UsuarioId }, "uq_calificacion_servicio_usuario").IsUnique();

            entity.Property(e => e.CalificacionId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("calificacion_id");
            entity.Property(e => e.Comentario).HasColumnName("comentario");
            entity.Property(e => e.FechaCalificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_calificacion");
            entity.Property(e => e.Puntuacion).HasColumnName("puntuacion");
            entity.Property(e => e.ServicioId).HasColumnName("servicio_id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Servicio).WithMany(p => p.Calificaciones)
                .HasForeignKey(d => d.ServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("calificaciones_servicio_id_fkey");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Calificaciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("calificaciones_usuario_id_fkey");
        });

        modelBuilder.Entity<DetailRequest>(entity =>
        {
            entity.HasKey(e => e.DetalleId).HasName("detalle_solicitud_pkey");

            entity.ToTable("detalle_solicitud");

            entity.HasIndex(e => new { e.SolicitudId, e.TipoResiduoId }, "uq_solicitud_residuo").IsUnique();

            entity.Property(e => e.DetalleId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("detalle_id");
            entity.Property(e => e.CantidadEstimada)
                .HasPrecision(12, 2)
                .HasColumnName("cantidad_estimada");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.SolicitudId).HasColumnName("solicitud_id");
            entity.Property(e => e.TipoResiduoId).HasColumnName("tipo_residuo_id");
            entity.Property(e => e.Unidad)
                .HasMaxLength(15)
                .HasDefaultValueSql("'KG'::character varying")
                .HasColumnName("unidad");

            entity.HasOne(d => d.Solicitud).WithMany(p => p.DetalleSolicituds)
                .HasForeignKey(d => d.SolicitudId)
                .HasConstraintName("detalle_solicitud_solicitud_id_fkey");

            entity.HasOne(d => d.TipoResiduo).WithMany(p => p.DetalleSolicituds)
                .HasForeignKey(d => d.TipoResiduoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_solicitud_tipo_residuo_id_fkey");
        });

        modelBuilder.Entity<HistoryRequest>(entity =>
        {
            entity.HasKey(e => e.HistorialId).HasName("historial_solicitud_pkey");

            entity.ToTable("historial_solicitud");

            entity.HasIndex(e => new { e.SolicitudId, e.FechaHora }, "idx_historial_solicitud");

            entity.Property(e => e.HistorialId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("historial_id");
            entity.Property(e => e.EstadoAnterior)
                .HasMaxLength(20)
                .HasColumnName("estado_anterior");
            entity.Property(e => e.EstadoNuevo)
                .HasMaxLength(20)
                .HasColumnName("estado_nuevo");
            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_hora");
            entity.Property(e => e.Observacion).HasColumnName("observacion");
            entity.Property(e => e.SolicitudId).HasColumnName("solicitud_id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Solicitud).WithMany(p => p.HistorialSolicituds)
                .HasForeignKey(d => d.SolicitudId)
                .HasConstraintName("historial_solicitud_solicitud_id_fkey");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialSolicituds)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("historial_solicitud_usuario_id_fkey");
        });

        modelBuilder.Entity<RecoveredMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("material_recuperado_pkey");

            entity.ToTable("material_recuperado");

            entity.HasIndex(e => e.ServicioId, "idx_material_servicio");

            entity.Property(e => e.MaterialId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("material_id");
            entity.Property(e => e.Cantidad)
                .HasPrecision(12, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.PuntoAcopioId).HasColumnName("punto_acopio_id");
            entity.Property(e => e.ServicioId).HasColumnName("servicio_id");
            entity.Property(e => e.TipoResiduoId).HasColumnName("tipo_residuo_id");
            entity.Property(e => e.Unidad)
                .HasMaxLength(15)
                .HasDefaultValueSql("'KG'::character varying")
                .HasColumnName("unidad");

            entity.HasOne(d => d.PuntoAcopio).WithMany(p => p.MaterialRecuperados)
                .HasForeignKey(d => d.PuntoAcopioId)
                .HasConstraintName("material_recuperado_punto_acopio_id_fkey");

            entity.HasOne(d => d.Servicio).WithMany(p => p.MaterialRecuperados)
                .HasForeignKey(d => d.ServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("material_recuperado_servicio_id_fkey");

            entity.HasOne(d => d.TipoResiduo).WithMany(p => p.MaterialRecuperados)
                .HasForeignKey(d => d.TipoResiduoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("material_recuperado_tipo_residuo_id_fkey");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizacionId).HasName("organizaciones_pkey");

            entity.ToTable("organizaciones");

            entity.HasIndex(e => e.Ruc, "organizaciones_ruc_key").IsUnique();

            entity.HasIndex(e => e.UsuarioId, "organizaciones_usuario_id_key").IsUnique();

            entity.Property(e => e.OrganizacionId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("organizacion_id");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Plan)
                .HasMaxLength(20)
                .HasDefaultValueSql("'BASICO'::character varying")
                .HasColumnName("plan");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(180)
                .HasColumnName("razon_social");
            entity.Property(e => e.Ruc)
                .HasMaxLength(11)
                .HasColumnName("ruc");
            entity.Property(e => e.Tipo)
                .HasMaxLength(40)
                .HasColumnName("tipo");
            entity.Property(e => e.UbicacionId).HasColumnName("ubicacion_id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Ubicacion).WithMany(p => p.Organizaciones)
                .HasForeignKey(d => d.UbicacionId)
                .HasConstraintName("organizaciones_ubicacion_id_fkey");

            entity.HasOne(d => d.Usuario).WithOne(p => p.Organizacione)
                .HasForeignKey<Organization>(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organizaciones_usuario_id_fkey");
        });

        modelBuilder.Entity<CollectionPoint>(entity =>
        {
            entity.HasKey(e => e.PuntoAcopioId).HasName("puntos_acopio_pkey");

            entity.ToTable("puntos_acopio");

            entity.Property(e => e.PuntoAcopioId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("punto_acopio_id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Horario)
                .HasMaxLength(150)
                .HasColumnName("horario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Responsable)
                .HasMaxLength(120)
                .HasColumnName("responsable");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.UbicacionId).HasColumnName("ubicacion_id");

            entity.HasOne(d => d.Ubicacion).WithMany(p => p.PuntosAcopios)
                .HasForeignKey(d => d.UbicacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("puntos_acopio_ubicacion_id_fkey");

            entity.HasMany(d => d.TipoResiduos).WithMany(p => p.PuntoAcopios)
                .UsingEntity<Dictionary<string, object>>(
                    "AcopioTipoResiduo",
                    r => r.HasOne<ResidueType>().WithMany()
                        .HasForeignKey("TipoResiduoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("acopio_tipo_residuo_tipo_residuo_id_fkey"),
                    l => l.HasOne<CollectionPoint>().WithMany()
                        .HasForeignKey("PuntoAcopioId")
                        .HasConstraintName("acopio_tipo_residuo_punto_acopio_id_fkey"),
                    j =>
                    {
                        j.HasKey("PuntoAcopioId", "TipoResiduoId").HasName("acopio_tipo_residuo_pkey");
                        j.ToTable("acopio_tipo_residuo");
                        j.IndexerProperty<long>("PuntoAcopioId").HasColumnName("punto_acopio_id");
                        j.IndexerProperty<long>("TipoResiduoId").HasColumnName("tipo_residuo_id");
                    });
        });

        modelBuilder.Entity<Gatherer>(entity =>
        {
            entity.HasKey(e => e.RecolectorId).HasName("recolectores_pkey");

            entity.ToTable("recolectores");

            entity.HasIndex(e => e.UsuarioId, "recolectores_usuario_id_key").IsUnique();

            entity.Property(e => e.RecolectorId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("recolector_id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.DocumentoVerificacion)
                .HasMaxLength(50)
                .HasColumnName("documento_verificacion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.EstadoVerificacion)
                .HasMaxLength(20)
                .HasDefaultValueSql("'PENDIENTE'::character varying")
                .HasColumnName("estado_verificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Tipo)
                .HasMaxLength(30)
                .HasColumnName("tipo");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithOne(p => p.Recolectore)
                .HasForeignKey<Gatherer>(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recolectores_usuario_id_fkey");

            entity.HasMany(d => d.TipoResiduos).WithMany(p => p.Recolectors)
                .UsingEntity<Dictionary<string, object>>(
                    "RecolectorResiduo",
                    r => r.HasOne<ResidueType>().WithMany()
                        .HasForeignKey("TipoResiduoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("recolector_residuos_tipo_residuo_id_fkey"),
                    l => l.HasOne<Gatherer>().WithMany()
                        .HasForeignKey("RecolectorId")
                        .HasConstraintName("recolector_residuos_recolector_id_fkey"),
                    j =>
                    {
                        j.HasKey("RecolectorId", "TipoResiduoId").HasName("recolector_residuos_pkey");
                        j.ToTable("recolector_residuos");
                        j.IndexerProperty<long>("RecolectorId").HasColumnName("recolector_id");
                        j.IndexerProperty<long>("TipoResiduoId").HasColumnName("tipo_residuo_id");
                    });
        });

        modelBuilder.Entity<PickupService>(entity =>
        {
            entity.HasKey(e => e.ServicioId).HasName("servicios_recojo_pkey");

            entity.ToTable("servicios_recojo");

            entity.HasIndex(e => e.Estado, "idx_servicios_estado");

            entity.HasIndex(e => e.RecolectorId, "idx_servicios_recolector");

            entity.HasIndex(e => e.SolicitudId, "servicios_recojo_solicitud_id_key").IsUnique();

            entity.Property(e => e.ServicioId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("servicio_id");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'ASIGNADO'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_asignacion");
            entity.Property(e => e.FechaFinalizacion).HasColumnName("fecha_finalizacion");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.PorcentajeComision)
                .HasPrecision(5, 2)
                .HasDefaultValue(10.00m)
                .HasColumnName("porcentaje_comision");
            entity.Property(e => e.RecolectorId).HasColumnName("recolector_id");
            entity.Property(e => e.SolicitudId).HasColumnName("solicitud_id");

            entity.HasOne(d => d.Recolector).WithMany(p => p.ServiciosRecojos)
                .HasForeignKey(d => d.RecolectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("servicios_recojo_recolector_id_fkey");

            entity.HasOne(d => d.Solicitud).WithOne(p => p.ServiciosRecojo)
                .HasForeignKey<PickupService>(d => d.SolicitudId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("servicios_recojo_solicitud_id_fkey");
        });

        modelBuilder.Entity<CollectRequest>(entity =>
        {
            entity.HasKey(e => e.SolicitudId).HasName("solicitudes_recojo_pkey");

            entity.ToTable("solicitudes_recojo");

            entity.HasIndex(e => e.Estado, "idx_solicitudes_estado");

            entity.HasIndex(e => e.FechaProgramada, "idx_solicitudes_fecha");

            entity.HasIndex(e => e.OrganizacionId, "idx_solicitudes_organizacion");

            entity.HasIndex(e => e.UsuarioId, "idx_solicitudes_usuario");

            entity.Property(e => e.SolicitudId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("solicitud_id");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'PENDIENTE'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaProgramada).HasColumnName("fecha_programada");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_solicitud");
            entity.Property(e => e.HoraProgramada).HasColumnName("hora_programada");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.OrganizacionId).HasColumnName("organizacion_id");
            entity.Property(e => e.UbicacionId).HasColumnName("ubicacion_id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Organizacion).WithMany(p => p.SolicitudesRecojos)
                .HasForeignKey(d => d.OrganizacionId)
                .HasConstraintName("solicitudes_recojo_organizacion_id_fkey");

            entity.HasOne(d => d.Ubicacion).WithMany(p => p.SolicitudesRecojos)
                .HasForeignKey(d => d.UbicacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("solicitudes_recojo_ubicacion_id_fkey");

            entity.HasOne(d => d.Usuario).WithMany(p => p.SolicitudesRecojos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("solicitudes_recojo_usuario_id_fkey");
        });

        modelBuilder.Entity<ResidueType>(entity =>
        {
            entity.HasKey(e => e.TipoResiduoId).HasName("tipos_residuo_pkey");

            entity.ToTable("tipos_residuo");

            entity.HasIndex(e => e.Nombre, "tipos_residuo_nombre_key").IsUnique();

            entity.Property(e => e.TipoResiduoId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("tipo_residuo_id");
            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .HasColumnName("categoria");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.EsReciclable).HasColumnName("es_reciclable");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.InstruccionesClasificacion).HasColumnName("instrucciones_clasificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Ubication>(entity =>
        {
            entity.HasKey(e => e.UbicacionId).HasName("ubicaciones_pkey");

            entity.ToTable("ubicaciones");

            entity.HasIndex(e => e.Distrito, "idx_ubicaciones_distrito");

            entity.Property(e => e.UbicacionId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("ubicacion_id");
            entity.Property(e => e.Departamento)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Arequipa'::character varying")
                .HasColumnName("departamento");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .HasColumnName("direccion");
            entity.Property(e => e.Distrito)
                .HasMaxLength(100)
                .HasColumnName("distrito");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Latitud)
                .HasPrecision(10, 7)
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasPrecision(10, 7)
                .HasColumnName("longitud");
            entity.Property(e => e.Provincia)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Arequipa'::character varying")
                .HasColumnName("provincia");
            entity.Property(e => e.Referencia)
                .HasMaxLength(250)
                .HasColumnName("referencia");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("usuarios_pkey");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Rol, "idx_usuarios_rol");

            entity.HasIndex(e => e.Correo, "usuarios_correo_key").IsUnique();

            entity.Property(e => e.UsuarioId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("usuario_id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(120)
                .HasColumnName("apellido");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(120)
                .HasColumnName("nombre");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .HasDefaultValueSql("'GENERADOR'::character varying")
                .HasColumnName("rol");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.UbicacionId).HasColumnName("ubicacion_id");

            entity.HasOne(d => d.Ubicacion).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.UbicacionId)
                .HasConstraintName("usuarios_ubicacion_id_fkey");
        });

        modelBuilder.Entity<GatherersGrades>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_calificaciones_recolectores");

            entity.Property(e => e.Apellido)
                .HasMaxLength(120)
                .HasColumnName("apellido");
            entity.Property(e => e.Nombre)
                .HasMaxLength(120)
                .HasColumnName("nombre");
            entity.Property(e => e.PromedioCalificacion).HasColumnName("promedio_calificacion");
            entity.Property(e => e.RecolectorId).HasColumnName("recolector_id");
            entity.Property(e => e.TotalCalificaciones).HasColumnName("total_calificaciones");
        });

        modelBuilder.Entity<Indicators>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_indicadores_generales");

            entity.Property(e => e.KilogramosRecuperados).HasColumnName("kilogramos_recuperados");
            entity.Property(e => e.ServiciosCompletados).HasColumnName("servicios_completados");
            entity.Property(e => e.SolicitudesCompletadas).HasColumnName("solicitudes_completadas");
            entity.Property(e => e.TotalOrganizaciones).HasColumnName("total_organizaciones");
            entity.Property(e => e.TotalRecolectoresVerificados).HasColumnName("total_recolectores_verificados");
            entity.Property(e => e.TotalSolicitudes).HasColumnName("total_solicitudes");
            entity.Property(e => e.TotalUsuarios).HasColumnName("total_usuarios");
        });

        modelBuilder.Entity<ResiduesByType>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_residuos_por_tipo");

            entity.Property(e => e.CantidadRecuperada).HasColumnName("cantidad_recuperada");
            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .HasColumnName("categoria");
            entity.Property(e => e.TipoResiduo)
                .HasMaxLength(100)
                .HasColumnName("tipo_residuo");
            entity.Property(e => e.TipoResiduoId).HasColumnName("tipo_residuo_id");
            entity.Property(e => e.Unidad)
                .HasMaxLength(15)
                .HasColumnName("unidad");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
