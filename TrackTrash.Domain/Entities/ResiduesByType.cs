using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class ResiduesByType
{
    public long? TipoResiduoId { get; set; }

    public string? TipoResiduo { get; set; }

    public string? Categoria { get; set; }

    public string? Unidad { get; set; }

    public decimal? CantidadRecuperada { get; set; }
}
