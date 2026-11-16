using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectionPoints.Commands;

public sealed record CreateCollectionPointCommand : IRequest<long>
{
    [Required, StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(120)]
    public string? ManagerName { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public long LocationId { get; set; }

    [StringLength(150)]
    public string? OpeningHours { get; set; }

    public string? Description { get; set; }
}

internal sealed class CreateCollectionPointCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCollectionPointCommand, long>
{
    public async Task<long> Handle(CreateCollectionPointCommand request, CancellationToken cancellationToken)
    {
        if (!await unitOfWork.Locations.ExistsAsync(x => x.LocationId == request.LocationId, cancellationToken))
        {
            throw new KeyNotFoundException("Location not found.");
        }

        var data = new CollectionPoint
        {
            Name = request.Name,
            ManagerName = request.ManagerName,
            Phone = request.Phone,
            LocationId = request.LocationId,
            OpeningHours = request.OpeningHours,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.CollectionPoints.AddAsync(data, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.CollectionPointId;
    }
}
