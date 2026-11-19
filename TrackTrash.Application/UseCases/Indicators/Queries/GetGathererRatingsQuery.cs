using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Indicators.Queries;

public sealed record GetGathererRatingsQuery : IRequest<IReadOnlyList<GathererRatingDto>>;

internal sealed class GetGathererRatingsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGathererRatingsQuery, IReadOnlyList<GathererRatingDto>>
{
    public async Task<IReadOnlyList<GathererRatingDto>> Handle(GetGathererRatingsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Indicators.GetGathererGradesAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
