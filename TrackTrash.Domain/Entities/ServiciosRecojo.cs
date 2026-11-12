using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class ServiciosRecojo
{
    public long ServicioId { get; set; }

    public long SolicitudId { get; set; }

    public long RecolectorId { get; set; }

    public DateTime FechaAsignacion { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFinalizacion { get; set; }

    public string Estado { get; set; } = null!;

    public decimal PorcentajeComision { get; set; }

    public string? Observaciones { get; set; }

    public virtual ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();

    public virtual ICollection<MaterialRecuperado> MaterialRecuperados { get; set; } = new List<MaterialRecuperado>();

    public virtual Recolector Recolector { get; set; } = null!;

    public virtual SolicitudesRecojo Solicitud { get; set; } = null!;
}
