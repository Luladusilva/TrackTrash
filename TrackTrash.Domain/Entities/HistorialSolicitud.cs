using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class HistorialSolicitud
{
    public long HistorialId { get; set; }

    public long SolicitudId { get; set; }

    public long? UsuarioId { get; set; }

    public string? EstadoAnterior { get; set; }

    public string EstadoNuevo { get; set; } = null!;

    public string? Observacion { get; set; }

    public DateTime FechaHora { get; set; }

    public virtual SolicitudesRecojo Solicitud { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
