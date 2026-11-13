using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class VwGathererGrade
{
    public long? GathererId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public decimal? AverageScore { get; set; }

    public long? TotalGrades { get; set; }
}
