using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Locations.Commands;

public sealed record DeleteLocationCommand : IRequest<bool>
{
    public long LocationId { get; set; }
}

internal sealed class DeleteLocationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteLocationCommand, bool>
{
    public async Task<bool> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Locations.GetByIdAsync(request.LocationId, cancellationToken)
                   ?? throw new KeyNotFoundException("Location not found.");

        unitOfWork.Locations.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
