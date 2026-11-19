using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.ResidueTypes.Queries;

public sealed record GetResidueTypesQuery : IRequest<IReadOnlyList<ResidueTypeDto>>;

internal sealed class GetResidueTypesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetResidueTypesQuery, IReadOnlyList<ResidueTypeDto>>
{
    public async Task<IReadOnlyList<ResidueTypeDto>> Handle(GetResidueTypesQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.ResidueTypes.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
