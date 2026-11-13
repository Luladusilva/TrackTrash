using System;
using System.Collections.Generic;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class User
{
    public long UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    public UserRole RoleId { get; set; }

    public long? LocationId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CollectRequest> CollectRequests { get; set; } = new List<CollectRequest>();

    public virtual Gatherer? Gatherer { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Location? Location { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<RequestHistory> RequestHistories { get; set; } = new List<RequestHistory>();
}
