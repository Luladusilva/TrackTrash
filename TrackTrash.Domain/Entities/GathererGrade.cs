using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class GathererGrade
{
    public long GathererGradeId { get; set; }

    public long GathererId { get; set; }

    public long GradeId { get; set; }

    public virtual Gatherer Gatherer { get; set; } = null!;

    public virtual Grade Grade { get; set; } = null!;
}
