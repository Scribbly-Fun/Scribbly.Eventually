using System.Diagnostics;
using System.Reflection;
using Scribbly.Eventually.Errors;

namespace Scribbly.Eventually;

public sealed class RoutingOptions
{
    public Action<IError> ErrorHandler { get; set; } = error =>
    {
        Debug.Write(error);
    };
}
public class CommandRouter
{
    private static readonly Dictionary<Type, (Type HandlerType, MethodInfo HandleMethod)> Handlers = new();
    
    static CommandRouter()
    {
        // TODO: Generate this collection using code generation.
        var handlerTypes = typeof(CommandRouter).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.BaseType?.Name == typeof(CommandHandler<,>).Name);
        
        foreach (var handlerType in handlerTypes)
        {
            var commandType = handlerType.BaseType?.GenericTypeArguments.First();
            var handleMethod = handlerType.GetMethod("Handle")!;
            Handlers.Add(commandType!, (handlerType, handleMethod));
        }
    }
    
    private readonly Func<IIdentity, IEnumerable<EventMessage>> _eventStream;
    private readonly Action<EventMessage> _publishEvent;
    private readonly RoutingOptions _options;

    public CommandRouter(
        Func<IIdentity, IEnumerable<EventMessage>> eventStream,
        Action<EventMessage> publishEvent,
        Action<RoutingOptions>? routingOptions = null)
    {
        _eventStream = eventStream;
        _publishEvent = publishEvent;

        _options = new RoutingOptions();
        routingOptions?.Invoke(_options);
    }

    public void Handle(CommandMessage command)
    {
        var filteredStream = _eventStream(command.AggregateId);
        var eventMessages = filteredStream as EventMessage[] ?? filteredStream.ToArray();
        var maxSequence = eventMessages.Select(e => e.Sequence).DefaultIfEmpty().Max();

        var commandType = command.Command.GetType();

        if (!Handlers.TryGetValue(commandType, out var handler))
        {
            _options.ErrorHandler(new HandlerNotFoundError($"Unable to find handler for command {commandType}"));
            return;
        }

        // TODO: Store the handlers in the dictionary and remove all reflection.
        var instance = Activator.CreateInstance(handler.HandlerType, eventMessages.Select(e => e.Event), (Action<IEvent>)Publish);

        handler.HandleMethod.Invoke(instance, [ command.Command ]);
        return;

        void Publish(IEvent ev) => _publishEvent(new EventMessage(command.AggregateId, ++maxSequence, ev));
    }
}

