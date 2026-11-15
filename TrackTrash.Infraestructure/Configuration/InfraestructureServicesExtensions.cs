using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Domain.Ports.UnitOfWork;
using TrackTrash.Infraestructure.Adapters.Repositories;
using TrackTrash.Infraestructure.Adapters.Services;
using TrackTrash.Infraestructure.Adapters.UnitOfWork;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Configuration;

public static class InfraestructureServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string DefaultConnection is not configured.");
        }

        services.AddDbContext<TrackTrashDBContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IGathererRepository, GathererRepository>();
        services.AddScoped<IResidueTypeRepository, ResidueTypeRepository>();
        services.AddScoped<ICollectionPointRepository, CollectionPointRepository>();
        services.AddScoped<ICollectRequestRepository, CollectRequestRepository>();
        services.AddScoped<IRequestDetailRepository, RequestDetailRepository>();
        services.AddScoped<IRequestHistoryRepository, RequestHistoryRepository>();
        services.AddScoped<IPickupServiceRepository, PickupServiceRepository>();
        services.AddScoped<IRecoveredMaterialRepository, RecoveredMaterialRepository>();
        services.AddScoped<IGradeRepository, GradeRepository>();
        services.AddScoped<IGathererGradeRepository, GathererGradeRepository>();
        services.AddScoped<IIndicatorRepository, IndicatorRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IXMLService, XMLService>();

        return services;
    }
}
