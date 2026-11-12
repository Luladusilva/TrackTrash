using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Ubication
{
    public long UbicacionId { get; set; }

    public string Direccion { get; set; } = null!;

    public string? Referencia { get; set; }

    public string Distrito { get; set; } = null!;

    public string? Provincia { get; set; }

    public string? Departamento { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Organization> Organizaciones { get; set; } = new List<Organization>();

    public virtual ICollection<CollectionPoint> PuntosAcopios { get; set; } = new List<CollectionPoint>();

    public virtual ICollection<CollectRequest> SolicitudesRecojos { get; set; } = new List<CollectRequest>();

    public virtual ICollection<User> Usuarios { get; set; } = new List<User>();
}
