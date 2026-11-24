using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RecoveredMaterials.Commands;

public sealed record DeleteRecoveredMaterialCommand : IRequest<bool>
{
    public long RecoveredMaterialId { get; set; }
}

internal sealed class DeleteRecoveredMaterialCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRecoveredMaterialCommand, bool>
{
    public async Task<bool> Handle(DeleteRecoveredMaterialCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RecoveredMaterials.GetByIdAsync(request.RecoveredMaterialId, cancellationToken)
                   ?? throw new KeyNotFoundException("Recovered material not found.");

        unitOfWork.RecoveredMaterials.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
