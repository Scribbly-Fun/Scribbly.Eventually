
namespace Scribbly.Eventually;

/// <summary>
/// Represents an ID using for an aggregate
/// </summary>
public interface IIdentity { }

/// <summary>
/// A generic ID used for an aggregate containing a value
/// </summary>
/// <typeparam name="TId">The type of data stored in the ID</typeparam>
public interface IIdentity<out TId> : IIdentity
{
    /// <summary>
    /// Gets the data backing up the ID
    /// </summary>
    TId Value { get; }
}

/// <summary>
/// A guid ID uses a Guid as an UUID
/// </summary>
/// <param name="Value">The guid value</param>
public sealed record GuidId(Guid Value) : IIdentity<Guid>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator GuidId(Guid value) => new(value);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static implicit operator Guid(GuidId id) => id.Value;
}

/// <summary>
/// An int ID uses a Int as an UUID
/// </summary>
/// <param name="Value">The Int value</param>
public sealed record IntId(int Value) : IIdentity<int>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator IntId(int value) => new(value);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static implicit operator int(IntId id) => id.Value;
}

/// <summary>
/// A Long ID uses a Long as an UUID
/// </summary>
/// <param name="Value">The Long value</param>
public sealed record LongId(long Value) : IIdentity<long>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator LongId(long value) => new(value);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static implicit operator long(LongId id) => id.Value;
}


