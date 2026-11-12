using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class PickupService
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

    public virtual ICollection<Grade> Calificaciones { get; set; } = new List<Grade>();

    public virtual ICollection<RecoveredMaterial> MaterialRecuperados { get; set; } = new List<RecoveredMaterial>();

    public virtual Gatherer Recolector { get; set; } = null!;

    public virtual CollectRequest Solicitud { get; set; } = null!;
}
