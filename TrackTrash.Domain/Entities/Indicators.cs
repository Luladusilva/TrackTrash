using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Indicators
{
    public long? TotalUsuarios { get; set; }

    public long? TotalOrganizaciones { get; set; }

    public long? TotalRecolectoresVerificados { get; set; }

    public long? TotalSolicitudes { get; set; }

    public long? SolicitudesCompletadas { get; set; }

    public long? ServiciosCompletados { get; set; }

    public decimal? KilogramosRecuperados { get; set; }
}
