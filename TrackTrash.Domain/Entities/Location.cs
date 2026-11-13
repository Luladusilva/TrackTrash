using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Location
{
    public long LocationId { get; set; }

    public string Address { get; set; } = null!;

    public string? Reference { get; set; }

    public string District { get; set; } = null!;

    public string? Province { get; set; }

    public string? Department { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CollectRequest> CollectRequests { get; set; } = new List<CollectRequest>();

    public virtual ICollection<CollectionPoint> CollectionPoints { get; set; } = new List<CollectionPoint>();

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
