using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Gatherers.Commands;

public sealed record DeleteGathererCommand : IRequest<bool>
{
    public long GathererId { get; set; }
}

internal sealed class DeleteGathererCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteGathererCommand, bool>
{
    public async Task<bool> Handle(DeleteGathererCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Gatherers.GetByIdAsync(request.GathererId, cancellationToken)
                   ?? throw new KeyNotFoundException("Gatherer not found.");

        unitOfWork.Gatherers.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
