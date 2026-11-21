using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RequestDetails.Commands;

public sealed record DeleteRequestDetailCommand : IRequest<bool>
{
    public long RequestDetailId { get; set; }
}

internal sealed class DeleteRequestDetailCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteRequestDetailCommand, bool>
{
    public async Task<bool> Handle(DeleteRequestDetailCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RequestDetails.GetByIdAsync(request.RequestDetailId, cancellationToken)
                   ?? throw new KeyNotFoundException("Request detail not found.");

        var collectRequest = await unitOfWork.CollectRequests.GetByIdAsync(data.CollectRequestId, cancellationToken);
        if (collectRequest is not null && collectRequest.StatusId != CollectRequestStatus.Pending)
        {
            throw new InvalidOperationException("Details can only be removed while the request is pending.");
        }

        unitOfWork.RequestDetails.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
