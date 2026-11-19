using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Organizations.Queries;

public sealed record GetOrganizationByUserQuery : IRequest<OrganizationDto>
{
    public long UserId { get; set; }
}

internal sealed class GetOrganizationByUserQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrganizationByUserQuery, OrganizationDto>
{
    public async Task<OrganizationDto> Handle(GetOrganizationByUserQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Organizations.GetByUserIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("Organization not found.");

        return data.ToDto();
    }
}
