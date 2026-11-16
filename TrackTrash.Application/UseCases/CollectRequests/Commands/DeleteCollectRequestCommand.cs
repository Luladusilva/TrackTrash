using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Commands;

public sealed record DeleteCollectRequestCommand : IRequest<bool>
{
    public long CollectRequestId { get; set; }
}

internal sealed class DeleteCollectRequestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCollectRequestCommand, bool>
{
    public async Task<bool> Handle(DeleteCollectRequestCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectRequests.GetByIdAsync(request.CollectRequestId, cancellationToken)
                   ?? throw new KeyNotFoundException("Collect request not found.");

        if (data.StatusId != CollectRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending collect requests can be deleted.");
        }

        unitOfWork.CollectRequests.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
