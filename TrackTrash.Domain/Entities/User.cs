using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class User
{
    public long UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public long? UbicacionId { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public virtual ICollection<Grade> Calificaciones { get; set; } = new List<Grade>();

    public virtual ICollection<HistoryRequest> HistorialSolicituds { get; set; } = new List<HistoryRequest>();

    public virtual Organization? Organizacione { get; set; }

    public virtual Gatherer? Recolectore { get; set; }

    public virtual ICollection<CollectRequest> SolicitudesRecojos { get; set; } = new List<CollectRequest>();

    public virtual Ubication? Ubicacion { get; set; }
}
