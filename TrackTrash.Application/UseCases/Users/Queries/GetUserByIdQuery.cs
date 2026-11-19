using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Users.Queries;

public sealed record GetUserByIdQuery : IRequest<UserDto>
{
    public long UserId { get; set; }
}

internal sealed class GetUserByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found.");

        return data.ToDto();
    }
}
