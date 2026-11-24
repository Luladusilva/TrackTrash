using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Users.Commands;

public sealed record UpdateUserCommand : IRequest<long>
{
    public long UserId { get; set; }

    [Required, StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(20)]
    public string? Phone { get; set; }

    public UserRole RoleId { get; set; }

    public long? LocationId { get; set; }

    public bool IsActive { get; set; }
}

internal sealed class UpdateUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand, long>
{
    public async Task<long> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found.");

        if (request.LocationId.HasValue &&
            !await unitOfWork.Locations.ExistsAsync(x => x.LocationId == request.LocationId, cancellationToken))
        {
            throw new KeyNotFoundException("Location not found.");
        }

        data.FirstName = request.FirstName.Trim();
        data.LastName = request.LastName.Trim();
        data.Phone = request.Phone;
        data.RoleId = request.RoleId;
        data.LocationId = request.LocationId;
        data.IsActive = request.IsActive;
        data.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Users.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.UserId;
    }
}
