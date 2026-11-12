using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Grade
{
    public long CalificacionId { get; set; }

    public long ServicioId { get; set; }

    public long UsuarioId { get; set; }

    public short Puntuacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaCalificacion { get; set; }

    public virtual PickupService Servicio { get; set; } = null!;

    public virtual User Usuario { get; set; } = null!;
}
