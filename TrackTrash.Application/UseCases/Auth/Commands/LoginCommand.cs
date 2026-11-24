using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.Ports.Services;

namespace TrackTrash.Application.UseCases.Auth.Commands;

public sealed record LoginCommand(LoginDto Login) : IRequest<AuthResponseDto?>;

internal sealed class LoginCommandHandler(IAuthService authService)
    : IRequestHandler<LoginCommand, AuthResponseDto?>
{
    public Task<AuthResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return authService.LoginAsync(request.Login, cancellationToken);
    }
}
