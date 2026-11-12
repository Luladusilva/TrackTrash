using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Ubicacion
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

    public virtual ICollection<Organizacion> Organizaciones { get; set; } = new List<Organizacion>();

    public virtual ICollection<PuntosAcopio> PuntosAcopios { get; set; } = new List<PuntosAcopio>();

    public virtual ICollection<SolicitudesRecojo> SolicitudesRecojos { get; set; } = new List<SolicitudesRecojo>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
