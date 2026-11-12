using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class TiposResiduo
{
    public long TipoResiduoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool EsReciclable { get; set; }

    public string? InstruccionesClasificacion { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<DetalleSolicitud> DetalleSolicituds { get; set; } = new List<DetalleSolicitud>();

    public virtual ICollection<MaterialRecuperado> MaterialRecuperados { get; set; } = new List<MaterialRecuperado>();

    public virtual ICollection<PuntosAcopio> PuntoAcopios { get; set; } = new List<PuntosAcopio>();

    public virtual ICollection<Recolector> Recolectors { get; set; } = new List<Recolector>();
}
