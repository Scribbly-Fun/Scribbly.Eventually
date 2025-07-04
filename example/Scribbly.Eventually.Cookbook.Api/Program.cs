using Microsoft.EntityFrameworkCore;
using Scribbly.Eventually;
using Scribbly.Eventually.Cookbook.Api.Commands;
using Scribbly.Eventually.Cookbook.Api.EventStream;
using Scribbly.Eventually.Cookbook.Api.Projections;
using Scribbly.Eventually.Cookbook.Api.ReadDatabase;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

// ------------------------------------------------------------------> Register Services

builder.Services.AddDbContext<ReadContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("scrb-db-read");
    options.UseNpgsql(connection);
});

builder.Services.AddDbContext<EventContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("scrb-db-events");
    options.UseNpgsql(connection);
});

builder.Services.AddScoped<EventService>();

builder.Services.AddScoped<CommandRouter>(sp =>
{
    var eventService = sp.GetRequiredService<EventService>();
    var router = new CommandRouter(
        eventService.GetEvents,
        eventService.AddEvent
    );
    return router;
});
// ------------------------------------------------------------------> Register Services

// ------------------------------------------------------------------> Register Projections
builder.Services.AddScoped<BoxStatusProjection>();
builder.Services.AddHostedService<ProjectionService<BoxStatusProjection>>();

// ------------------------------------------------------------------> Register Projections

// ------------------------------------------------------------------> Configure and Start App

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await using var scope = app.Services.CreateAsyncScope();

    var configurationDbContext = scope.ServiceProvider.GetRequiredService<ReadContext>();
    await configurationDbContext.Database.EnsureCreatedAsync();

    var proModelDbContext = scope.ServiceProvider.GetRequiredService<EventContext>();
    await proModelDbContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

app.MapCommandEndpoints();

app.Run();
