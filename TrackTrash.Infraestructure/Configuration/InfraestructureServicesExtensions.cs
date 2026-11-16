using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Infraestructure.Adapters.UnitOfWork;
//using TrackTrash.Infraestructure.Context;
using TrackTrash.Infraestructure.Adapters.Repositories;
using TrackTrash.Infraestructure.Adapters.Services;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Domain.Ports.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrackTrash.Infraestructure.Configuration;

public static class InfraestructureServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices
        (
        this IServiceCollection services,
        IConfiguration configuration
        //JwtTokenOptions jwtOptions
        )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
        }

        var jwtOptions = configuration
            .GetSection("Jwt")
            .Get<JwtOptions>() ?? throw new InvalidOperationException(
                "JWT configuration is not configured.");

        if (string.IsNullOrWhiteSpace(jwtOptions.Key))
        {
            throw new InvalidOperationException("JWT key is not configured.");
        }

        services.AddSingleton(jwtOptions);
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

        services.AddAuthorization();

        //services.AddDbContext<TrackTrashDBContext>(options =>
        //    options.UseNpgsql(connectionString));
        // services.AddScoped<IUnitOfWork, UnitOfWork>();
        // services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        // services.AddScoped<IUserRepository, UserRepository>();
        // services.AddScoped<ICollectionPointRepository, CollectionPointRepository>();
        // services.AddScoped<ICollectRequestRepository, CollectRequestRepository>();
        // services.AddScoped<IDetailRequestRepository, DetailRequestRepository>();
        // services.AddScoped<IGatherersGradesRepository, GatherersGradesRepository>();
        // services.AddScoped<IGatherersRepository, GatherersRepository>();
        // services.AddScoped<IGenericRepository, GenericRepository>();
        // services.AddScoped<IGradeRepository, GradeRepository>();
        // services.AddScoped<IHistoryRequestRepository, HistoryRequestRepository>();
        // services.AddScoped<IIndicatorsRepository, IndicatorsRepository>();
        // services.AddScoped<ILocationRepository, LocationRepository>();
        // services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        // services.AddScoped<IPickupServiceRepository, PickupServiceRepository>();
        // services.AddScoped<IRecoveredMaterialRepository, RecoveredMaterialRepository>();
        // services.AddScoped<IResidueTypeRepository, ResidueTypeRepository>();
        // services.AddScoped<IUserRepository, UserRepository>();
        //services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        //services.AddSingleton(jwtOptions);
        //services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
