using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Locations.Commands;

public sealed record UpdateLocationCommand : IRequest<long>
{
    public long LocationId { get; set; }

    [Required, StringLength(250)]
    public string Address { get; set; } = null!;

    [StringLength(250)]
    public string? Reference { get; set; }

    [Required, StringLength(100)]
    public string District { get; set; } = null!;

    [StringLength(100)]
    public string? Province { get; set; }

    [StringLength(100)]
    public string? Department { get; set; }

    [Range(-90, 90)]
    public decimal? Latitude { get; set; }

    [Range(-180, 180)]
    public decimal? Longitude { get; set; }
}

internal sealed class UpdateLocationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateLocationCommand, long>
{
    public async Task<long> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Locations.GetByIdAsync(request.LocationId, cancellationToken)
                   ?? throw new KeyNotFoundException("Location not found.");

        data.Address = request.Address;
        data.Reference = request.Reference;
        data.District = request.District;
        data.Province = request.Province;
        data.Department = request.Department;
        data.Latitude = request.Latitude;
        data.Longitude = request.Longitude;

        unitOfWork.Locations.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.LocationId;
    }
}
