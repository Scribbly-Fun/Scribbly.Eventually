using Microsoft.AspNetCore.Mvc;
using Scribbly.Eventually.Cookbook.Api.EventStream;

namespace Scribbly.Eventually.Cookbook.Api.Commands;

public static class CommandEndpoints
{
    public static IEndpointRouteBuilder MapCommandEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/commands", ([FromServices] CommandRouter router, [FromServices] EventService events, [FromBody] CommandMessage command) =>
        {
            router.Handle(command);
            events.Commit();
        });
        
        return endpoints;
    }
    
}