using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Organizations.Queries;

public sealed record GetOrganizationsQuery : IRequest<IReadOnlyList<OrganizationDto>>;

internal sealed class GetOrganizationsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrganizationsQuery, IReadOnlyList<OrganizationDto>>
{
    public async Task<IReadOnlyList<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Organizations.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
