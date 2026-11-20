using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Users.Commands;

/// <summary>
/// Deactivates a user instead of removing the row, so their requests, services and grades keep their history.
/// </summary>
public sealed record DeleteUserCommand : IRequest<bool>
{
    public long UserId { get; set; }
}

internal sealed class DeleteUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found.");

        data.IsActive = false;
        data.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Users.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
