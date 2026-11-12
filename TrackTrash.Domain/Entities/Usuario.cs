using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Usuario
{
    public long UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public long? UbicacionId { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public virtual ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();

    public virtual ICollection<HistorialSolicitud> HistorialSolicituds { get; set; } = new List<HistorialSolicitud>();

    public virtual Organizacion? Organizacione { get; set; }

    public virtual Recolector? Recolectore { get; set; }

    public virtual ICollection<SolicitudesRecojo> SolicitudesRecojos { get; set; } = new List<SolicitudesRecojo>();

    public virtual Ubicacion? Ubicacion { get; set; }
}
