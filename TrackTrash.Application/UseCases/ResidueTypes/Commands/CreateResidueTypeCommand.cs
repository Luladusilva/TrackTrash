using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.ResidueTypes.Commands;

public sealed record CreateResidueTypeCommand : IRequest<long>
{
    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [EnumDataType(typeof(ResidueCategory))]
    public ResidueCategory ResidueCategoryId { get; set; }

    public string? Description { get; set; }

    public bool IsRecyclable { get; set; }

    public string? ClassificationInstructions { get; set; }
}

internal sealed class CreateResidueTypeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateResidueTypeCommand, long>
{
    public async Task<long> Handle(CreateResidueTypeCommand request, CancellationToken cancellationToken)
    {
        if (await unitOfWork.ResidueTypes.ExistsAsync(x => x.Name == request.Name, cancellationToken))
        {
            throw new InvalidOperationException("A residue type with the same name already exists.");
        }

        var data = new ResidueType
        {
            Name = request.Name,
            ResidueCategoryId = request.ResidueCategoryId,
            Description = request.Description,
            IsRecyclable = request.IsRecyclable,
            ClassificationInstructions = request.ClassificationInstructions,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.ResidueTypes.AddAsync(data, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.ResidueTypeId;
    }
}
