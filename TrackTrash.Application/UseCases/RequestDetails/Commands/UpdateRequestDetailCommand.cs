using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RequestDetails.Commands;

public sealed record UpdateRequestDetailCommand : IRequest<long>
{
    public long RequestDetailId { get; set; }

    public long ResidueTypeId { get; set; }

    [Range(typeof(decimal), "0.01", "100000")]
    public decimal EstimatedQuantity { get; set; }

    public MeasurementUnit UnitId { get; set; }

    public string? Observations { get; set; }
}

internal sealed class UpdateRequestDetailCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateRequestDetailCommand, long>
{
    public async Task<long> Handle(UpdateRequestDetailCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RequestDetails.GetByIdAsync(request.RequestDetailId, cancellationToken)
                   ?? throw new KeyNotFoundException("Request detail not found.");

        var collectRequest = await unitOfWork.CollectRequests.GetByIdAsync(data.CollectRequestId, cancellationToken)
                             ?? throw new KeyNotFoundException("Collect request not found.");

        if (collectRequest.StatusId != CollectRequestStatus.Pending)
        {
            throw new InvalidOperationException("Details can only be changed while the request is pending.");
        }

        if (!await unitOfWork.ResidueTypes.ExistsAsync(x => x.ResidueTypeId == request.ResidueTypeId, cancellationToken))
        {
            throw new KeyNotFoundException("Residue type not found.");
        }

        data.ResidueTypeId = request.ResidueTypeId;
        data.EstimatedQuantity = request.EstimatedQuantity;
        data.UnitId = request.UnitId;
        data.Observations = request.Observations;

        unitOfWork.RequestDetails.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.RequestDetailId;
    }
}
