using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Indicators.Queries;

public sealed record GetGeneralIndicatorsQuery : IRequest<GeneralIndicatorDto>;

internal sealed class GetGeneralIndicatorsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGeneralIndicatorsQuery, GeneralIndicatorDto>
{
    public async Task<GeneralIndicatorDto> Handle(GetGeneralIndicatorsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Indicators.GetGeneralAsync(cancellationToken)
                   ?? throw new KeyNotFoundException("Indicators not available.");

        return data.ToDto();
    }
}
