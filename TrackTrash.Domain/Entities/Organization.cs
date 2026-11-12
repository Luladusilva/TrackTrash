using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Organization
{
    public long OrganizacionId { get; set; }

    public long UsuarioId { get; set; }

    public string RazonSocial { get; set; } = null!;

    public string? Ruc { get; set; }

    public string Tipo { get; set; } = null!;

    public long? UbicacionId { get; set; }

    public string Plan { get; set; } = null!;

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<CollectRequest> SolicitudesRecojos { get; set; } = new List<CollectRequest>();

    public virtual Ubication? Ubicacion { get; set; }

    public virtual User Usuario { get; set; } = null!;
}
