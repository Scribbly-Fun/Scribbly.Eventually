namespace Scribbly.Eventually;

public record CommandMessage(
    IIdentity AggregateId,
    ICommand Command);