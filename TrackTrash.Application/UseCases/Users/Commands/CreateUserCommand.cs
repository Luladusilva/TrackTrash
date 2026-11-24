using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Users.Commands;

public sealed record CreateUserCommand : IRequest<long>
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, StringLength(100)]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Phone { get; set; }

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = null!;

    public UserRole RoleId { get; set; } = UserRole.Generator;

    public long? LocationId { get; set; }
}

internal sealed class CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHashingService passwordHashingService)
    : IRequestHandler<CreateUserCommand, long>
{
    public async Task<long> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await unitOfWork.Users.GetByEmailAsync(email, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        if (request.LocationId.HasValue &&
            !await unitOfWork.Locations.ExistsAsync(x => x.LocationId == request.LocationId, cancellationToken))
        {
            throw new KeyNotFoundException("Location not found.");
        }

        var now = DateTime.UtcNow;
        var data = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            Phone = request.Phone,
            RoleId = request.RoleId,
            LocationId = request.LocationId,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        data.PasswordHash = passwordHashingService.HashPassword(data, request.Password);

        await unitOfWork.Users.AddAsync(data, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.UserId;
    }
}
