using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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

        var jwtOptions = configuration
            .GetSection("Jwt")
            .Get<JwtOptions>() ?? throw new InvalidOperationException(
                "JWT configuration is not configured.");

        if (string.IsNullOrWhiteSpace(jwtOptions.Key))
        {
            throw new InvalidOperationException("JWT key is not configured.");
        }

        if (Encoding.UTF8.GetByteCount(jwtOptions.Key) < 32)
        {
            throw new InvalidOperationException("JWT key must contain at least 32 UTF-8 bytes.");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience) ||
            jwtOptions.AccessTokenExpirationHours <= 0 || jwtOptions.RefreshTokenExpirationDays <= 0)
        {
            throw new InvalidOperationException("JWT issuer, audience and token lifetimes must be configured with valid values.");
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
        services.AddScoped<ICollectRequestApplicationRepository, CollectRequestApplicationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IXMLService, XMLService>();
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton(jwtOptions);

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
                    RequireExpirationTime = true,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var subject = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        var familyValue = context.Principal?.FindFirst("session_family")?.Value;
                        if (!long.TryParse(subject, out var userId) || !Guid.TryParse(familyValue, out var familyId))
                        {
                            context.Fail("The access token is missing required session claims.");
                            return;
                        }

                        var db = context.HttpContext.RequestServices.GetRequiredService<TrackTrashDBContext>();
                        var now = DateTime.UtcNow;
                        var user = await db.Users.AsNoTracking()
                            .Where(x => x.UserId == userId)
                            .Select(x => new { x.IsActive, x.RoleId })
                            .SingleOrDefaultAsync(context.HttpContext.RequestAborted);
                        var sessionIsActive = await db.RefreshTokens.AsNoTracking()
                            .AnyAsync(x => x.FamilyId == familyId && x.RevokedAt == null && x.ExpiresAt > now,
                                context.HttpContext.RequestAborted);

                        var tokenRole = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                        if (user is null || !user.IsActive || !sessionIsActive || tokenRole != user.RoleId.ToString())
                            context.Fail("The access token session is no longer active.");
                    },
                    OnChallenge = async context =>
                    {
                        if (context.AuthenticateFailure is not SecurityTokenExpiredException)
                            return;

                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            code = "ACCESS_TOKEN_EXPIRED",
                            error = "The access token has expired."
                        });
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
