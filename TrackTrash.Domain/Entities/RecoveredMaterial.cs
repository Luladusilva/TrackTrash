using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class RecoveredMaterial
{
    public long MaterialId { get; set; }

    public long ServicioId { get; set; }

    public long TipoResiduoId { get; set; }

    public decimal Cantidad { get; set; }

    public string Unidad { get; set; } = null!;

    public long? PuntoAcopioId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string? Observaciones { get; set; }

    public virtual CollectionPoint? PuntoAcopio { get; set; }

    public virtual PickupService Servicio { get; set; } = null!;

    public virtual ResidueType TipoResiduo { get; set; } = null!;
}
