using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.ResidueTypes.Queries;

public sealed record GetResidueTypeByIdQuery : IRequest<ResidueTypeDto>
{
    public long ResidueTypeId { get; set; }
}

internal sealed class GetResidueTypeByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetResidueTypeByIdQuery, ResidueTypeDto>
{
    public async Task<ResidueTypeDto> Handle(GetResidueTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.ResidueTypes.GetByIdAsync(request.ResidueTypeId, cancellationToken)
                   ?? throw new KeyNotFoundException("Residue type not found.");

        return data.ToDto();
    }
}
