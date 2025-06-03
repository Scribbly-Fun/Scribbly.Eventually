namespace Scribbly.Eventually;

/// <summary>
/// A checpoint stored in the DB to track the last event read from the stream.
/// </summary>
public class ProjectionCheckpoint
{
    // TODO: Add mapping abstractions
    
    /// <summary>
    /// 
    /// </summary>
    public string ProjectionName { get; set; }
  
    /// <summary>
    /// 
    /// </summary>
    public ulong EventVersion { get; set; }
}