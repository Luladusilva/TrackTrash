using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Gatherer
{
    public long RecolectorId { get; set; }

    public long UsuarioId { get; set; }

    public string Tipo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? DocumentoVerificacion { get; set; }

    public string EstadoVerificacion { get; set; } = null!;

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<PickupService> ServiciosRecojos { get; set; } = new List<PickupService>();

    public virtual User Usuario { get; set; } = null!;

    public virtual ICollection<ResidueType> TipoResiduos { get; set; } = new List<ResidueType>();
}
