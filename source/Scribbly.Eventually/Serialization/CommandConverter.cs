using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scribbly.Eventually.Serialization;

public class CommandConverter : JsonConverter<CommandMessage>
{
    private static readonly Dictionary<string, Type> TypeLookup = new();

    static CommandConverter()
    {
        var commandTypes = typeof(ICommand)
            .Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(ICommand).IsAssignableFrom(type));

        foreach (var commandType in commandTypes)
        {
            TypeLookup[commandType.Name] = commandType;
        }
    }

    /// <inheritdoc />
    public override bool CanConvert(Type type)
    {
        return typeof(CommandMessage).IsAssignableFrom(type);
    }

    /// <inheritdoc />
    public override CommandMessage? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        if (!reader.Read()
            || reader.TokenType != JsonTokenType.PropertyName
            || reader.GetString()?.ToLower() != "$type")
        {
            throw new JsonException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var typeDiscriminator = reader.GetString();
        var commandType = TypeLookup[typeDiscriminator!];

        if (!reader.Read()
            || reader.TokenType != JsonTokenType.PropertyName
            || reader.GetString()?.ToLower() != "aggregate_id")
        {
            throw new JsonException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var aggregateIdString = reader.GetString();
        Guid.TryParse(aggregateIdString, out Guid aggregateId);

        if (!reader.Read() || reader.GetString()?.ToLower() != "command")
        {
            throw new JsonException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var command = (ICommand)JsonSerializer.Deserialize(ref reader, commandType)!;

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
        {
            throw new JsonException();
        }

        return new CommandMessage(new GuidId(aggregateId), command);
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        CommandMessage value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("$type", value.GetType().Name);
        writer.WriteString("aggregate_id", value.AggregateId.ToString());
        writer.WritePropertyName("command");
        JsonSerializer.Serialize(writer, value, value.GetType());

        writer.WriteEndObject();
    }
}