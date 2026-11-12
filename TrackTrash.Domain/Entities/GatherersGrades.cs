using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class GatherersGrades
{
    public long? RecolectorId { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public decimal? PromedioCalificacion { get; set; }

    public long? TotalCalificaciones { get; set; }
}
