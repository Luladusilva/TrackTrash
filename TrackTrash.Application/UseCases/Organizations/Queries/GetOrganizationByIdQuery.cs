using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Organizations.Queries;

public sealed record GetOrganizationByIdQuery : IRequest<OrganizationDto>
{
    public long OrganizationId { get; set; }
}

internal sealed class GetOrganizationByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto>
{
    public async Task<OrganizationDto> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Organizations.GetByIdAsync(request.OrganizationId, cancellationToken)
                   ?? throw new KeyNotFoundException("Organization not found.");

        return data.ToDto();
    }
}
