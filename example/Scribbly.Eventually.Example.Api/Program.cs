using Microsoft.EntityFrameworkCore;
using Scribbly.Eventually;
using Scribbly.Eventually.Example.Api.Commands;
using Scribbly.Eventually.Example.Api.EventStream;
using Scribbly.Eventually.Example.Api.Projections;
using Scribbly.Eventually.Example.Api.ReadDatabase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// ------------------------------------------------------------------> Register Services
builder.Services.AddDbContext<ReadContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ReadContext"));
});

builder.Services.AddDbContext<EventContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("EventContext"));
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
}

app.UseHttpsRedirection();

app.MapCommandEndpoints();

app.Run();
