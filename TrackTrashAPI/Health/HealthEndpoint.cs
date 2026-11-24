using System.Diagnostics;
using System.Reflection;
using TrackTrash.Infraestructure.Context;

namespace TrackTrashAPI.Health;

public static class HealthEndpoint
{
    private static readonly DateTime StartedAt = DateTime.UtcNow;

    public static IEndpointRouteBuilder MapHealthEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", async (TrackTrashDBContext db, IHostEnvironment environment, CancellationToken cancellationToken) =>
        {
            var stopwatch = Stopwatch.StartNew();
            bool databaseUp;
            try
            {
                databaseUp = await db.Database.CanConnectAsync(cancellationToken);
            }
            catch (Exception)
            {
                databaseUp = false;
            }
            stopwatch.Stop();

            var commit = Environment.GetEnvironmentVariable("RENDER_GIT_COMMIT");
            var uptime = DateTime.UtcNow - StartedAt;

            return Results.Ok(new
            {
                status = databaseUp ? "ok" : "degraded",
                service = "TrackTrash API",
                environment = environment.EnvironmentName,
                version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                commit = string.IsNullOrEmpty(commit) ? null : commit[..Math.Min(7, commit.Length)],
                startedAt = StartedAt,
                uptime = $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m",
                database = new
                {
                    status = databaseUp ? "up" : "down",
                    latencyMs = stopwatch.ElapsedMilliseconds
                },
                documentation = "/swagger"
            });
        }).AllowAnonymous();

        app.MapGet("/api/health/live", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

        return app;
    }
}
