using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class ResidueType
{
    public long TipoResiduoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool EsReciclable { get; set; }

    public string? InstruccionesClasificacion { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<DetailRequest> DetalleSolicituds { get; set; } = new List<DetailRequest>();

    public virtual ICollection<RecoveredMaterial> MaterialRecuperados { get; set; } = new List<RecoveredMaterial>();

    public virtual ICollection<CollectionPoint> PuntoAcopios { get; set; } = new List<CollectionPoint>();

    public virtual ICollection<Gatherer> Recolectors { get; set; } = new List<Gatherer>();
}
