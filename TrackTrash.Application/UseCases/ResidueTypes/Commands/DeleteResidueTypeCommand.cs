using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.ResidueTypes.Commands;

public sealed record DeleteResidueTypeCommand : IRequest<bool>
{
    public long ResidueTypeId { get; set; }
}

internal sealed class DeleteResidueTypeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteResidueTypeCommand, bool>
{
    public async Task<bool> Handle(DeleteResidueTypeCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.ResidueTypes.GetByIdAsync(request.ResidueTypeId, cancellationToken)
                   ?? throw new KeyNotFoundException("Residue type not found.");

        unitOfWork.ResidueTypes.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
