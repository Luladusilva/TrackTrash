using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RequestDetails.Queries;

public sealed record GetRequestDetailsQuery : IRequest<IReadOnlyList<RequestDetailDto>>;

internal sealed class GetRequestDetailsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRequestDetailsQuery, IReadOnlyList<RequestDetailDto>>
{
    public async Task<IReadOnlyList<RequestDetailDto>> Handle(GetRequestDetailsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RequestDetails.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
