using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Organizations.Commands;

public sealed record DeleteOrganizationCommand : IRequest<bool>
{
    public long OrganizationId { get; set; }
}

internal sealed class DeleteOrganizationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteOrganizationCommand, bool>
{
    public async Task<bool> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Organizations.GetByIdAsync(request.OrganizationId, cancellationToken)
                   ?? throw new KeyNotFoundException("Organization not found.");

        unitOfWork.Organizations.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
