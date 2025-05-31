namespace Scribbly.Eventually.Errors;

/// <summary>
/// Defines an error in the eventually framework
/// </summary>
public interface IError
{
    /// <summary>
    /// An error code 
    /// </summary>
    int Code { get; }
    /// <summary>
    /// The severity of the error
    /// </summary>
    Severity Severity { get; }
    /// <summary>
    /// A message describing what went wrong.
    /// </summary>
    string Message { get; }
    /// <summary>
    /// An optional inner error
    /// </summary>
    IError? InnerError { get; }
}