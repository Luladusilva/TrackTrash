using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Grades.Commands;

public sealed record UpdateGradeCommand : IRequest<long>
{
    public long GradeId { get; set; }

    [Range(1, 5)]
    public short Score { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }
}

internal sealed class UpdateGradeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateGradeCommand, long>
{
    public async Task<long> Handle(UpdateGradeCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Grades.GetByIdAsync(request.GradeId, cancellationToken)
                   ?? throw new KeyNotFoundException("Grade not found.");

        data.Score = request.Score;
        data.Comment = request.Comment;

        unitOfWork.Grades.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.GradeId;
    }
}
