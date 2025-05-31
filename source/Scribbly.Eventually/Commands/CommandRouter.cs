using System.Diagnostics;
using System.Reflection;
using Scribbly.Eventually.Errors;

namespace Scribbly.Eventually;

/// <summary>
/// A configuration POCO used to manage options for the routing of commands and events.
/// </summary>
public sealed class RoutingOptions
{
    /// <summary>
    /// A list of assemblies to scan for commands and handlers.
    /// </summary>
    public List<Assembly> HandleAssemblies { get; set; } = [];
    
    /// <summary>
    /// A Error outlet, all errors will be sent to this outlet if provided.
    /// </summary>
    public Action<IError> ErrorHandler { get; set; } = error =>
    {
        Debug.Write(error);
    };
}

/// <summary>
/// A router to processor commands and direct them to the correct command handler.
/// </summary>
public class CommandRouter
{
    private static readonly Dictionary<Type, (Type HandlerType, MethodInfo HandleMethod)> Handlers = new();
    
    static CommandRouter()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes()) 
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.BaseType?.Name == typeof(CommandHandler<,>).Name);
            
        // TODO: Generate this collection using code generation.
        // var handlerTypes = typeof(CommandRouter).Assembly.GetTypes()
        //     .Where(t => t is { IsClass: true, IsAbstract: false })
        //     .Where(t => t.BaseType?.Name == typeof(CommandHandler<,>).Name);
        //
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

    /// <summary>
    /// Creates a new command router
    /// </summary>
    /// <param name="eventStream">The event stream containing all the events for the given handler.</param>
    /// <param name="publishEvent">An outlet for the new events as the handlers create them</param>
    /// <param name="routingOptions">Options to configure behavior</param>
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

    /// <summary>
    /// Handles a command message and routes the command to the correct handler.
    /// </summary>
    /// <param name="command">The command to process</param>
    public void Handle(CommandMessage command)
    {
        var filteredStream = _eventStream(command.AggregateId);
        var eventMessages = filteredStream as EventMessage[] ?? filteredStream.ToArray();
        var maxSequence = eventMessages.Select(e => e.Sequence).DefaultIfEmpty().Max();

        var commandType = command.Command.GetType();

        if (!Handlers.TryGetValue(commandType, out var handler))
        {
            _options.ErrorHandler(new HandlerNotFoundError(commandType));
            return;
        }

        // TODO: Store the handlers in the dictionary and remove all reflection.
        var instance = Activator.CreateInstance(handler.HandlerType, eventMessages.Select(e => e.Event), (Action<IEvent>)Publish);

        handler.HandleMethod.Invoke(instance, [ command.Command ]);
        return;

        void Publish(IEvent ev) => _publishEvent(new EventMessage(command.AggregateId, ++maxSequence, ev));
    }
}

