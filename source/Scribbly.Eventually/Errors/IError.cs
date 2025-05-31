namespace Scribbly.Eventually.Errors;

public enum Severity
{
    Fatal,
    Critical,
    Warning
}

public interface IError
{
    int Code { get; }
    
    Severity Severity { get; }
    
    string Message { get; }
    
    IError? InnerError { get; }
}