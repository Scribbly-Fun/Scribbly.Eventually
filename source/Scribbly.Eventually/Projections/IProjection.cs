namespace Scribbly.Eventually;

/// <summary>
/// A projection takes a stream of events and creates new state used to display data
/// </summary>
public interface IProjection
{
    /// <summary>
    /// The number of events to process at once
    /// </summary>
    int BatchSize { get; }
    
    /// <summary>
    /// How long between each batch the projection should wait before running the next batch
    /// </summary>
    TimeSpan WaitTime { get; }
    
    /// <summary>
    /// The events the the projection should process
    /// Todo: Move this to code gen
    /// </summary>
    Type[] EventTypes { get; }
    
    /// <summary>
    /// Handles a batch of event messages
    /// </summary>
    /// <param name="batch"></param>
    void HandleBatch(IEnumerable<EventMessage> batch);
}