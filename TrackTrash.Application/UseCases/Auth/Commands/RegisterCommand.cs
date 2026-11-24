using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Auth.Commands;

public sealed record RegisterCommand(RegisterDto Registration) : IRequest<long>;

internal sealed class RegisterCommandHandler(IUnitOfWork unitOfWork, IPasswordHashingService hashing)
    : IRequestHandler<RegisterCommand, long>
{
    public async Task<long> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var input = command.Registration;
        var validProfile = input.RoleId switch
        {
            UserRole.Generator => input.Organization is null && input.Gatherer is null,
            UserRole.Organization => input.Organization is not null && input.Gatherer is null,
            UserRole.Gatherer => input.Gatherer is not null && input.Organization is null,
            _ => false
        };
        if (!validProfile)
            throw new InvalidOperationException("Invalid role or registration profile.");

        var email = input.Email.Trim().ToLowerInvariant();
        if (await unitOfWork.Users.GetByEmailAsync(email, cancellationToken) is not null)
            throw new InvalidOperationException("Email is already registered.");

        var now = DateTime.UtcNow;
        var location = new Location
        {
            Address = input.Location.Address.Trim(),
            District = input.Location.District.Trim(),
            Reference = input.Location.Reference,
            Province = input.Location.Province,
            Department = input.Location.Department,
            Latitude = input.Location.Latitude,
            Longitude = input.Location.Longitude,
            CreatedAt = now
        };
        var user = new User
        {
            FirstName = input.FirstName.Trim(),
            LastName = input.LastName.Trim(),
            Email = email,
            Phone = input.Phone,
            RoleId = input.RoleId,
            Location = location,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        user.PasswordHash = hashing.HashPassword(user, input.Password);

        if (input.Organization is { } organization)
        {
            if (organization.TaxId is { } taxId &&
                await unitOfWork.Organizations.ExistsAsync(x => x.TaxId == taxId, cancellationToken))
                throw new InvalidOperationException("An organization with the same tax id already exists.");

            user.Organization = new Organization
            {
                BusinessName = organization.BusinessName.Trim(),
                TaxId = organization.TaxId,
                OrganizationTypeId = organization.OrganizationTypeId,
                SubscriptionPlanId = SubscriptionPlan.Basic,
                Location = location,
                IsActive = true,
                CreatedAt = now
            };
        }

        if (input.Gatherer is { } gatherer)
            user.Gatherer = new Gatherer
            {
                GathererTypeId = gatherer.GathererTypeId,
                Description = gatherer.Description,
                VerificationDocument = gatherer.VerificationDocument,
                VerificationStatusId = VerificationStatus.Pending,
                IsActive = true,
                CreatedAt = now
            };

        await unitOfWork.Users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.UserId;
    }
}
