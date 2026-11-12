using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Calificacion
{
    public long CalificacionId { get; set; }

    public long ServicioId { get; set; }

    public long UsuarioId { get; set; }

    public short Puntuacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaCalificacion { get; set; }

    public virtual ServiciosRecojo Servicio { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
