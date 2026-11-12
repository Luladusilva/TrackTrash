using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class MaterialRecuperado
{
    public long MaterialId { get; set; }

    public long ServicioId { get; set; }

    public long TipoResiduoId { get; set; }

    public decimal Cantidad { get; set; }

    public string Unidad { get; set; } = null!;

    public long? PuntoAcopioId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string? Observaciones { get; set; }

    public virtual PuntosAcopio? PuntoAcopio { get; set; }

    public virtual ServiciosRecojo Servicio { get; set; } = null!;

    public virtual TiposResiduo TipoResiduo { get; set; } = null!;
}
