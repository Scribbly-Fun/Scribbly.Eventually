namespace Scribbly.Eventually;

/// <summary>
/// 
/// </summary>
public abstract class Aggregate
{
    /// <summary>
    /// Applies an event to the state of the aggregate
    /// </summary>
    /// <param name="event"></param>
    /// <exception cref="Exception"></exception>
    public void Apply(object @event)
    {
        throw new Exception($"Event type {@event.GetType()} not implemented for {GetType()}.");
    }
}