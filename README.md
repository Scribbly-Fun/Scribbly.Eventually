![scribbly_banner.png](./docs/scribbly_banner.png)

# Eventually
An Event sourcing framework used by Scribbly.Fun free for use and contributions.

# 🏃‍♂️ Aggregate

Aggregates are the thing, yup the thing your commands and events effect.  Aggregates should extend the ``Aggregate`` abstract class.

Aggregates should include an apply method for each type of event it's expected to receive.

```csharp

class Box: Aggregate
{
    public Capacity? Capacity { get; private set; }
    public Shipping_label? Shipping_label { get; private set; }
    public List<Bottle> Contents { get; } = new();
    public bool Closed { get; private set; }
    public bool Shipped { get; private set; }

    public void Apply(Box_created @event)
    {
        Capacity = @event.Capacity;
    }

    public void Apply(Beer_added @event)
    {
        Contents.Add(@event.Beer);
    }
    
    public void Apply(Label_applied @event)
    {
        Shipping_label = @event.Label;
    }

    public void Apply(Box_shipped @event)
    {
        Shipped = true;
    }
}

```

# 💪 Commands

Commands command the system to act on an aggregate.  A command must implement the ``ICommand`` interface

```csharp
public record ShipBox : ICommand;
```

# 🛒 Handlers

A handle accepts the command and applies business logic.  Command handlers should extend the ``CommandHandler<TCommand, TAggregate>`` base class.

A handlers purpose is to encapsulate all the business logic.

```csharp
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
```

# 📆 Events

A event is the result of the applied command.  An event must implement the ``IEvent`` interface

```csharp
public record BoxShipped : IEvent;

public record BoxNotReadyToShip(Reason Reason) : IEvent;
```

