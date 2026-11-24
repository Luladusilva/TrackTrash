using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Grades.Commands;

/// <summary>
/// Rates a completed pickup service. Only the user who made the request can rate it, once.
/// The grade is also linked to the gatherer so it counts toward their rating.
/// </summary>
public sealed record CreateGradeCommand : IRequest<long>
{
    public long PickupServiceId { get; set; }

    public long UserId { get; set; }

    [Range(1, 5)]
    public short Score { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }
}

internal sealed class CreateGradeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateGradeCommand, long>
{
    public async Task<long> Handle(CreateGradeCommand request, CancellationToken cancellationToken)
    {
        var service = await unitOfWork.PickupServices.GetByIdAsync(request.PickupServiceId, cancellationToken)
                      ?? throw new KeyNotFoundException("Pickup service not found.");

        if (service.StatusId != PickupServiceStatus.Completed)
        {
            throw new InvalidOperationException("Only completed pickup services can be rated.");
        }

        var collectRequest = await unitOfWork.CollectRequests.GetByIdAsync(service.CollectRequestId, cancellationToken)
                             ?? throw new KeyNotFoundException("Collect request not found.");

        if (collectRequest.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("Only the user who made the request can rate this service.");
        }

        if (await unitOfWork.Grades.GetByServiceAndUserAsync(request.PickupServiceId, request.UserId, cancellationToken) is not null)
        {
            throw new InvalidOperationException("This service has already been rated.");
        }

        var grade = new Grade
        {
            PickupServiceId = request.PickupServiceId,
            UserId = request.UserId,
            Score = request.Score,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Grades.AddAsync(grade, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await unitOfWork.GathererGrades.AddAsync(new GathererGrade
        {
            GathererId = service.GathererId,
            GradeId = grade.GradeId
        }, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return grade.GradeId;
    }
}
