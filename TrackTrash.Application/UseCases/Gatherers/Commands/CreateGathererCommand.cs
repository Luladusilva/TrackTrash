using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Gatherers.Commands;

public sealed record CreateGathererCommand : IRequest<long>
{
    public long UserId { get; set; }

    [EnumDataType(typeof(GathererType))]
    public GathererType GathererTypeId { get; set; }

    public string? Description { get; set; }

    [StringLength(50)]
    public string? VerificationDocument { get; set; }
}

internal sealed class CreateGathererCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateGathererCommand, long>
{
    public async Task<long> Handle(CreateGathererCommand request, CancellationToken cancellationToken)
    {
        if (!await unitOfWork.Users.ExistsAsync(x => x.UserId == request.UserId, cancellationToken))
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (await unitOfWork.Gatherers.ExistsAsync(x => x.UserId == request.UserId, cancellationToken))
        {
            throw new InvalidOperationException("The user is already registered as a gatherer.");
        }

        var data = new Gatherer
        {
            UserId = request.UserId,
            GathererTypeId = request.GathererTypeId,
            Description = request.Description,
            VerificationDocument = request.VerificationDocument,
            VerificationStatusId = VerificationStatus.Pending,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Gatherers.AddAsync(data, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.GathererId;
    }
}
