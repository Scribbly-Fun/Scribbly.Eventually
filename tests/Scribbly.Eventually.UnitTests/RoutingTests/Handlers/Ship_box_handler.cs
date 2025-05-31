using Scribbly.Eventually.UnitTests.Setup.Aggregates;

namespace Scribbly.Eventually.UnitTests.RoutingTests.Handlers;

internal class ShipBoxHandler(
    IEnumerable<IEvent> eventStream,
    Action<IEvent> publishEvent)
    : CommandHandler<Ship_box, Box>(eventStream, publishEvent)
{
    protected override IEnumerable<IEvent> HandleCommand(
        Box aggregate,
        Ship_box command)
    {
        if (aggregate is { Closed: true, Shipping_label: not null })
        {
            yield return new Box_shipped();
            yield break;
        }

        if (!aggregate.Closed)
            yield return new Box_was_not_ready(
                Box_was_not_ready.Fail_reason.Box_was_not_closed);

        if (aggregate.Shipping_label is null)
            yield return new Box_was_not_ready(
                Box_was_not_ready.Fail_reason.Box_has_no_shipping_label);
    }
}