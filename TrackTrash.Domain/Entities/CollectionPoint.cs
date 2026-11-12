using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class CollectionPoint
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

    public virtual ICollection<RecoveredMaterial> MaterialRecuperados { get; set; } = new List<RecoveredMaterial>();

    public virtual Ubication Ubicacion { get; set; } = null!;

    public virtual ICollection<ResidueType> TipoResiduos { get; set; } = new List<ResidueType>();
}
