using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RequestDetails.Commands;

public sealed record CreateRequestDetailCommand : IRequest<long>
{
    public long CollectRequestId { get; set; }

    public long ResidueTypeId { get; set; }

    [Range(typeof(decimal), "0.01", "100000")]
    public decimal EstimatedQuantity { get; set; }

    public MeasurementUnit UnitId { get; set; } = MeasurementUnit.Kg;

    public string? Observations { get; set; }
}

internal sealed class CreateRequestDetailCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateRequestDetailCommand, long>
{
    public async Task<long> Handle(CreateRequestDetailCommand request, CancellationToken cancellationToken)
    {
        var collectRequest = await unitOfWork.CollectRequests.GetByIdAsync(request.CollectRequestId, cancellationToken)
                             ?? throw new KeyNotFoundException("Collect request not found.");

        if (collectRequest.StatusId != CollectRequestStatus.Pending)
        {
            throw new InvalidOperationException("Details can only be added while the request is pending.");
        }

        if (!await unitOfWork.ResidueTypes.ExistsAsync(x => x.ResidueTypeId == request.ResidueTypeId, cancellationToken))
        {
            throw new KeyNotFoundException("Residue type not found.");
        }

        var data = new RequestDetail
        {
            CollectRequestId = request.CollectRequestId,
            ResidueTypeId = request.ResidueTypeId,
            EstimatedQuantity = request.EstimatedQuantity,
            UnitId = request.UnitId,
            Observations = request.Observations
        };

        await unitOfWork.RequestDetails.AddAsync(data, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.RequestDetailId;
    }
}
