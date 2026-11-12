using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class PuntosAcopio
{
    public long PuntoAcopioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Responsable { get; set; }

    public string? Telefono { get; set; }

    public long UbicacionId { get; set; }

    public string? Horario { get; set; }

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<MaterialRecuperado> MaterialRecuperados { get; set; } = new List<MaterialRecuperado>();

    public virtual Ubicacion Ubicacion { get; set; } = null!;

    public virtual ICollection<TiposResiduo> TipoResiduos { get; set; } = new List<TiposResiduo>();
}
