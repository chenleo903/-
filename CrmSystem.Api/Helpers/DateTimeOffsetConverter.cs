using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrmSystem.Api.Helpers;

/// <summary>
/// Custom JSON converter for DateTimeOffset to ensure ISO 8601 format with UTC timezone
/// Format: yyyy-MM-ddTHH:mm:ss.fffZ
/// </summary>
public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
        {
            throw new JsonException("DateTimeOffset value cannot be null or empty");
        }

        if (DateTimeOffset.TryParse(dateString, out var result))
        {
            return result.ToUniversalTime();
        }

        throw new JsonException($"Unable to parse '{dateString}' as DateTimeOffset");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        // Always output in UTC with ISO 8601 format
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
    }
}

/// <summary>
/// Custom JSON converter for nullable DateTimeOffset
/// </summary>
public class NullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
        {
            return null;
        }

        if (DateTimeOffset.TryParse(dateString, out var result))
        {
            return result.ToUniversalTime();
        }

        throw new JsonException($"Unable to parse '{dateString}' as DateTimeOffset");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            // Always output in UTC with ISO 8601 format
            writer.WriteStringValue(value.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
