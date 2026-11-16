using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Gatherers.Commands;

public sealed record UpdateGathererCommand : IRequest<long>
{
    public long GathererId { get; set; }

    [EnumDataType(typeof(GathererType))]
    public GathererType GathererTypeId { get; set; }

    public string? Description { get; set; }

    [StringLength(50)]
    public string? VerificationDocument { get; set; }

    [EnumDataType(typeof(VerificationStatus))]
    public VerificationStatus VerificationStatusId { get; set; }

    public bool IsActive { get; set; }
}

internal sealed class UpdateGathererCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateGathererCommand, long>
{
    public async Task<long> Handle(UpdateGathererCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Gatherers.GetByIdAsync(request.GathererId, cancellationToken)
                   ?? throw new KeyNotFoundException("Gatherer not found.");

        data.GathererTypeId = request.GathererTypeId;
        data.Description = request.Description;
        data.VerificationDocument = request.VerificationDocument;
        data.VerificationStatusId = request.VerificationStatusId;
        data.IsActive = request.IsActive;

        unitOfWork.Gatherers.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.GathererId;
    }
}
