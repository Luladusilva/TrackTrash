using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Grades.Commands;

/// <summary>Deletes a grade together with its gatherer link.</summary>
public sealed record DeleteGradeCommand : IRequest<bool>
{
    public long GradeId { get; set; }
}

internal sealed class DeleteGradeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteGradeCommand, bool>
{
    public async Task<bool> Handle(DeleteGradeCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Grades.GetByIdAsync(request.GradeId, cancellationToken)
                   ?? throw new KeyNotFoundException("Grade not found.");

        foreach (var link in await unitOfWork.GathererGrades.FindAsync(x => x.GradeId == request.GradeId, cancellationToken))
        {
            unitOfWork.GathererGrades.Remove(link);
        }

        unitOfWork.Grades.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
