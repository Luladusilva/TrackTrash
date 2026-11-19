using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Users.Queries;

public sealed record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;

internal sealed class GetUsersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Users.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
