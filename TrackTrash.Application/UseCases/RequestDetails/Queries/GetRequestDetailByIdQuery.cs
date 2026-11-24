using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RequestDetails.Queries;

public sealed record GetRequestDetailByIdQuery : IRequest<RequestDetailDto>
{
    public long RequestDetailId { get; set; }
}

internal sealed class GetRequestDetailByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRequestDetailByIdQuery, RequestDetailDto>
{
    public async Task<RequestDetailDto> Handle(GetRequestDetailByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RequestDetails.GetByIdAsync(request.RequestDetailId, cancellationToken)
                   ?? throw new KeyNotFoundException("Request detail not found.");

        return data.ToDto();
    }
}
