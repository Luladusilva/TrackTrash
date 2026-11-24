using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Indicators.Queries;

public sealed record GetResiduesByTypeQuery : IRequest<IReadOnlyList<ResiduesByTypeDto>>;

internal sealed class GetResiduesByTypeQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetResiduesByTypeQuery, IReadOnlyList<ResiduesByTypeDto>>
{
    public async Task<IReadOnlyList<ResiduesByTypeDto>> Handle(GetResiduesByTypeQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Indicators.GetResiduesByTypeAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
