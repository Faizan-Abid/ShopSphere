using Catalog.API.Middleware;
using Catalog.Application;
using Catalog.Infrastructure;
using Serilog;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Catalog API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, config) => config
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "Catalog.API"));

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("CatalogDb")!);
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "Catalog API"));
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            db.Database.Migrate();
        }
    }

    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Catalog API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }