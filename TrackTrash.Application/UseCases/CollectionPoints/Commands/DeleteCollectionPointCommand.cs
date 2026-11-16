using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectionPoints.Commands;

public sealed record DeleteCollectionPointCommand : IRequest<bool>
{
    public long CollectionPointId { get; set; }
}

internal sealed class DeleteCollectionPointCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCollectionPointCommand, bool>
{
    public async Task<bool> Handle(DeleteCollectionPointCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectionPoints.GetByIdAsync(request.CollectionPointId, cancellationToken)
                   ?? throw new KeyNotFoundException("Collection point not found.");

        unitOfWork.CollectionPoints.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
