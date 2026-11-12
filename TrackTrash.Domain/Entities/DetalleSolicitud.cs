using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class DetalleSolicitud
{
    public long DetalleId { get; set; }

    public long SolicitudId { get; set; }

    public long TipoResiduoId { get; set; }

    public decimal CantidadEstimada { get; set; }

    public string Unidad { get; set; } = null!;

    public string? Observaciones { get; set; }

    public virtual SolicitudesRecojo Solicitud { get; set; } = null!;

    public virtual TiposResiduo TipoResiduo { get; set; } = null!;
}
