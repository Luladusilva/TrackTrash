using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class DetailRequest
{
    public long DetalleId { get; set; }

    public long SolicitudId { get; set; }

    public long TipoResiduoId { get; set; }

    public decimal CantidadEstimada { get; set; }

    public string Unidad { get; set; } = null!;

    public string? Observaciones { get; set; }

    public virtual CollectRequest Solicitud { get; set; } = null!;

    public virtual ResidueType TipoResiduo { get; set; } = null!;
}
