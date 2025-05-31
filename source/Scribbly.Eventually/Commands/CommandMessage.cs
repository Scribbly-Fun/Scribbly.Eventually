namespace Scribbly.Eventually;

/// <summary>
/// A command message is a message sent to the aggregate that is responsible for creating events.
/// Commands are invoked and routed to the handlers where business rules are applied.
/// <remarks>Commands should be considered present tense</remarks>
/// </summary>
/// <seealso cref="IIdentity"/>
/// <seealso cref="Aggregate"/>
/// <param name="AggregateId">The ID of the specific instance of the aggregate</param>
/// <param name="Command">The command containing the information required to act on the system.</param>
public record CommandMessage(
    IIdentity AggregateId,
    ICommand Command);