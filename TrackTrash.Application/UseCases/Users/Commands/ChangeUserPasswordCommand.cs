using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Users.Commands;

public sealed record ChangeUserPasswordCommand : IRequest<bool>
{
    public long UserId { get; set; }

    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required, StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; } = null!;
}

internal sealed class ChangeUserPasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHashingService passwordHashingService)
    : IRequestHandler<ChangeUserPasswordCommand, bool>
{
    public async Task<bool> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found.");

        if (!passwordHashingService.VerifyPassword(data, request.CurrentPassword, data.PasswordHash))
        {
            throw new InvalidOperationException("Current password is incorrect.");
        }

        data.PasswordHash = passwordHashingService.HashPassword(data, request.NewPassword);
        data.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Users.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
