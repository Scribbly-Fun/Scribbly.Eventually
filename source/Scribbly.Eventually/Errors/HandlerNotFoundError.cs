namespace Scribbly.Eventually.Errors;

/// <summary>
/// An error to represent an unknown command handler
/// </summary>
public sealed class HandlerNotFoundError : IError
{
    /// <inheritdoc />
    public int Code { get; } = 100;
    
    /// <inheritdoc />
    public Severity Severity { get; } = Severity.Critical;
    
    /// <inheritdoc />
    public string Message { get; } = "Handler not found";
    
    /// <inheritdoc />
    public IError? InnerError { get; }
    
    /// <summary>
    /// Default constructor using the default codes and messages
    /// </summary>
    public HandlerNotFoundError()
    {
        
    }

    /// <summary>
    /// Constructor to override the message in the error
    /// </summary>
    /// <param name="commandType">The command type that has no handler</param>
    public HandlerNotFoundError(Type commandType)
        :this()
    {
        Message = $"Not handle could be located for the {commandType.Name}";
    }
    
    /// <summary>
    /// Constructor to override the message in the error
    /// </summary>
    /// <param name="commandType">The command type that has no handler</param>
    /// <param name="innerError">Adds an inner error</param>
    public HandlerNotFoundError(Type commandType, IError? innerError) 
        : this(commandType)
    {
        InnerError = innerError;
    }
};