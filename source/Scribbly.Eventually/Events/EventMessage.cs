namespace Scribbly.Eventually;

/// <summary>
/// 
/// </summary>
/// <param name="AggregateId"></param>
/// <param name="Sequence"></param>
/// <param name="Event"></param>
public record EventMessage(
    IIdentity AggregateId,
    int Sequence,
    IEvent Event);