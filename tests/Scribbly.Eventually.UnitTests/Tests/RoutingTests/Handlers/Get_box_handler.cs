using Scribbly.Eventually.Setup.Aggregates;

namespace Scribbly.Eventually.Tests.RoutingTests.Handlers;

class Get_box_handler : CommandHandler<Get_box, Box>
{
    public Get_box_handler(
        IEnumerable<IEvent> event_stream, 
        Action<IEvent> publish_event) 
        : base(event_stream, publish_event)
    { }

    protected override IEnumerable<IEvent> HandleCommand(
        Box aggregate,
        Get_box command)
    {
        var capacity = Capacity.Create(command.Desired_number_of_spots);
        yield return new Box_created(capacity);
    }
}