using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class SolicitudesRecojo
{
    public long SolicitudId { get; set; }

    public long UsuarioId { get; set; }

    public long? OrganizacionId { get; set; }

    public long UbicacionId { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public DateOnly FechaProgramada { get; set; }

    public TimeOnly HoraProgramada { get; set; }

    public string Estado { get; set; } = null!;

    public string? Observaciones { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public virtual ICollection<DetalleSolicitud> DetalleSolicituds { get; set; } = new List<DetalleSolicitud>();

    public virtual ICollection<HistorialSolicitud> HistorialSolicituds { get; set; } = new List<HistorialSolicitud>();

    public virtual Organizacion? Organizacion { get; set; }

    public virtual ServiciosRecojo? ServiciosRecojo { get; set; }

    public virtual Ubicacion Ubicacion { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
