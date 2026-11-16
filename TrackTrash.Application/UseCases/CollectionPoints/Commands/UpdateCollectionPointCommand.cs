using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectionPoints.Commands;

public sealed record UpdateCollectionPointCommand : IRequest<long>
{
    public long CollectionPointId { get; set; }

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

    public bool IsActive { get; set; }
}

internal sealed class UpdateCollectionPointCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCollectionPointCommand, long>
{
    public async Task<long> Handle(UpdateCollectionPointCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectionPoints.GetByIdAsync(request.CollectionPointId, cancellationToken)
                   ?? throw new KeyNotFoundException("Collection point not found.");

        if (!await unitOfWork.Locations.ExistsAsync(x => x.LocationId == request.LocationId, cancellationToken))
        {
            throw new KeyNotFoundException("Location not found.");
        }

        data.Name = request.Name;
        data.ManagerName = request.ManagerName;
        data.Phone = request.Phone;
        data.LocationId = request.LocationId;
        data.OpeningHours = request.OpeningHours;
        data.Description = request.Description;
        data.IsActive = request.IsActive;

        unitOfWork.CollectionPoints.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.CollectionPointId;
    }
}
