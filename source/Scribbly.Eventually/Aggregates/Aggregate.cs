namespace Scribbly.Eventually;

/// <summary>
/// 
/// </summary>
abstract class Aggregate
{
    public void Apply(object @event)
    {
        throw new Exception($"Event type {@event.GetType()} not implemented for {GetType()}.");
    }
}