using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.ResidueTypes.Commands;

public sealed record UpdateResidueTypeCommand : IRequest<long>
{
    public long ResidueTypeId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [EnumDataType(typeof(ResidueCategory))]
    public ResidueCategory ResidueCategoryId { get; set; }

    public string? Description { get; set; }

    public bool IsRecyclable { get; set; }

    public string? ClassificationInstructions { get; set; }

    public bool IsActive { get; set; }
}

internal sealed class UpdateResidueTypeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateResidueTypeCommand, long>
{
    public async Task<long> Handle(UpdateResidueTypeCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.ResidueTypes.GetByIdAsync(request.ResidueTypeId, cancellationToken)
                   ?? throw new KeyNotFoundException("Residue type not found.");

        if (await unitOfWork.ResidueTypes.ExistsAsync(x => x.Name == request.Name && x.ResidueTypeId != request.ResidueTypeId, cancellationToken))
        {
            throw new InvalidOperationException("A residue type with the same name already exists.");
        }

        data.Name = request.Name;
        data.ResidueCategoryId = request.ResidueCategoryId;
        data.Description = request.Description;
        data.IsRecyclable = request.IsRecyclable;
        data.ClassificationInstructions = request.ClassificationInstructions;
        data.IsActive = request.IsActive;

        unitOfWork.ResidueTypes.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.ResidueTypeId;
    }
}
