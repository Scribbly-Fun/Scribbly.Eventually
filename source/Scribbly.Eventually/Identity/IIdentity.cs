using System.Security.Principal;

namespace Scribbly.Eventually;

public interface IIdentity { }

public interface IIdentity<out TId> : IIdentity
{
    TId Value { get; }
}

public sealed record GuidId(Guid Value) : IIdentity<Guid>
{
    public static implicit operator GuidId(Guid value) => new(value);
    public static implicit operator Guid(GuidId id) => id.Value;
}

public sealed record IntId(int Value) : IIdentity<int>
{
    public static implicit operator IntId(int value) => new(value);
    public static implicit operator int(IntId id) => id.Value;
}


public sealed record LongId(long Value) : IIdentity<long>
{
    public static implicit operator LongId(long value) => new(value);
    public static implicit operator long(LongId id) => id.Value;
}


