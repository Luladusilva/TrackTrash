using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class CollectRequest
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

    public virtual ICollection<DetailRequest> DetalleSolicituds { get; set; } = new List<DetailRequest>();

    public virtual ICollection<HistoryRequest> HistorialSolicituds { get; set; } = new List<HistoryRequest>();

    public virtual Organization? Organizacion { get; set; }

    public virtual PickupService? ServiciosRecojo { get; set; }

    public virtual Ubication Ubicacion { get; set; } = null!;

    public virtual User Usuario { get; set; } = null!;
}
