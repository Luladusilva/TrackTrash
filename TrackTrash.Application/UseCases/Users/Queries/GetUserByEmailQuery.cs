using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Users.Queries;

public sealed record GetUserByEmailQuery : IRequest<UserDto>
{
    public string Email { get; set; } = null!;
}

internal sealed class GetUserByEmailQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserByEmailQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found.");

        return data.ToDto();
    }
}
