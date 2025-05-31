namespace Scribbly.Eventually.Errors;

/// <summary>
/// The severity of the error.
/// </summary>
public enum Severity
{
    /// <summary>
    /// The application should probably be stopped...
    /// </summary>
    Fatal,
    
    /// <summary>
    /// The error results if undefined behavior.
    /// </summary>
    Critical,
    
    /// <summary>
    /// Hmm, the error might be something silly
    /// </summary>
    Warning
}