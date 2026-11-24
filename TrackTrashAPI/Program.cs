using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using TrackTrash.Application.Configuration;
using TrackTrash.Infraestructure.Configuration;
using TrackTrashAPI.Health;
using TrackTrashAPI.Swagger;
using TrackTrashAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TrackTrash API",
        Version = "v1",
        Description = "API de TrackTrash: recojo de residuos reciclables en Arequipa. Usa POST /api/Auth/Login y pega el accessToken en Authorize."
    });
    options.OperationFilter<AllowAnonymousOperationFilter>();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Ingresa solo el token JWT."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? string.Empty)
    .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment() && allowedOrigins.Length == 0)
            policy.AllowAnyOrigin();
        else
            policy.WithOrigins(allowedOrigins);

        policy.AllowAnyHeader().AllowAnyMethod();
    }));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "TrackTrash API";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TrackTrash API v1");
    options.EnablePersistAuthorization();
    options.DisplayRequestDuration();
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous().ExcludeFromDescription();
app.MapHealthEndpoint();

app.Run();
