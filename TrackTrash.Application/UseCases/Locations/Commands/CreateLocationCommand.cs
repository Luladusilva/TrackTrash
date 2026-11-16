using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Locations.Commands;

public sealed record CreateLocationCommand : IRequest<long>
{
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

internal sealed class CreateLocationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateLocationCommand, long>
{
    public async Task<long> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var data = new Location
        {
            Address = request.Address,
            Reference = request.Reference,
            District = request.District,
            Province = request.Province,
            Department = request.Department,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Locations.AddAsync(data, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.LocationId;
    }
}
