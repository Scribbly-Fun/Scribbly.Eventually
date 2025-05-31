namespace Scribbly.Eventually.Errors;

public sealed class HandlerNotFoundError : IError
{
    public int Code { get; } = 100;
    
    public Severity Severity { get; } = Severity.Critical;
    
    public string Message { get; } = "Handler not found";
    
    public IError? InnerError { get; }
    
    public HandlerNotFoundError()
    {
        
    }

    public HandlerNotFoundError(string message)
        :this()
    {
        Message = message;
    }
    
    public HandlerNotFoundError(string message, IError? innerError) 
        : this(message)
    {
        InnerError = innerError;
    }
};