using Microsoft.AspNetCore.Mvc;
using Scribbly.Eventually.Example.Api.EventStream;

namespace Scribbly.Eventually.Example.Api.Commands;

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