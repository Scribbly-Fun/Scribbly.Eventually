using Scribbly.Eventually.UnitTests.Setup.Aggregates;

namespace Scribbly.Eventually.UnitTests.RoutingTests.Handlers;

class Add_beer_handler : CommandHandler<Add_beer, Box>
{
    public Add_beer_handler(
        IEnumerable<IEvent> event_stream,
        Action<IEvent> publish_event)
        : base(event_stream, publish_event)
    { }

    protected override IEnumerable<IEvent> HandleCommand(
        Box aggregate,
        Add_beer command)
    {
        if ((aggregate.Capacity?.Number_of_spots ?? 0) 
            > aggregate.Contents.Count)
            yield return new Beer_added(command.Beer);
        else
            yield return new Beer_failed_to_add(
                Beer_failed_to_add.Fail_reason.Box_was_full);
    }
}