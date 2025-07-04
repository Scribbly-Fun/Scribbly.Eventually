namespace Scribbly.Eventually.Setup.Aggregates;


class Box: Aggregate
{
    public Capacity? Capacity { get; private set; }
    public Shipping_label? Shipping_label { get; private set; }
    public List<Bottle> Contents { get; } = new();
    public bool Closed { get; private set; }
    public bool Shipped { get; private set; }

    // Get box
    public void Apply(Box_created @event)
    {
        Capacity = @event.Capacity;
    }

    // Add beer
    public void Apply(Beer_added @event)
    {
        Contents.Add(@event.Beer);
    }
    public void Apply(Beer_failed_to_add @event) { }

    // Apply shipping label
    public void Apply(Label_applied @event)
    {
        Shipping_label = @event.Label;
    }
    public void Apply(Label_was_invalid @event) { }

    // Close box
    public void Apply(Box_closed @event)
    {
        Closed = true;
    }
    public void Apply(Box_failed_to_close @event) { }

    // Ship box
    public void Apply(Box_shipped @event)
    {
        Shipped = true;
    }
    public void Apply(Box_was_not_ready @event) { }
}

// Get box
public record Box_created(Capacity Capacity) : IEvent;

// Add beer
public record Beer_added(Bottle Beer) : IEvent;
public record Beer_failed_to_add(Beer_failed_to_add.Fail_reason Reason) : IEvent
{
    public enum Fail_reason
    {
        Box_was_full
    }
}

// Close Box
public record Box_closed : IEvent;
public record Box_failed_to_close(Box_failed_to_close.Fail_reason Reason) : IEvent
{
    public enum Fail_reason
    {
        Box_was_empty
    }
}

// Add label
public record Label_was_invalid : IEvent;
public record Label_applied(Shipping_label Label) : IEvent;

// Ship box
public record Box_shipped : IEvent;
public record Box_was_not_ready(Box_was_not_ready.Fail_reason Reason) : IEvent
{
    public enum Fail_reason
    {
        Box_was_not_closed,
        Box_has_no_shipping_label
    }
}

public record Capacity(int Number_of_spots)
{
    public static Capacity Create(int desired_number_of_spots)
    {
        return desired_number_of_spots switch
        {
            <= 6 => new Capacity(6),
            <= 12 => new Capacity(12),
            <= 24 => new Capacity(24),
            _ => new Capacity(24)
        };
    }
}

public enum Shipping_carrier
{
    Fedex,
    Ups,
    Bpost
}

public record Shipping_label(
    Shipping_carrier Carrier,
    string Tracking_code)
{
    public bool Is_valid()
    {
        switch(Carrier)
        {
            case Shipping_carrier.Fedex:
                return Tracking_code.StartsWith("ABC");
            case Shipping_carrier.Ups:
                return Tracking_code.StartsWith("DEF");
            case Shipping_carrier.Bpost:
                return Tracking_code.StartsWith("GHI");
            default:
                return false;
        }
    }
}


public record Bottle(
    string Brewery,
    string Name,
    decimal Alcohol_percentage,
    int Volume_in_ml);
    
public record Get_box(int Desired_number_of_spots) : ICommand;
public record Add_beer(Bottle Beer) : ICommand;
public record Apply_shipping_label(Shipping_label Label) : ICommand;
public record Close_box : ICommand;
public record Ship_box : ICommand;