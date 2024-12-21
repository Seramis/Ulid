#if NETCOREAPP3_0_OR_GREATER
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ByteAether.Ulid;

/// <summary>
/// A custom JSON converter for the <see cref="Ulid"/> type.
/// </summary>
public class UlidJsonConverter : JsonConverter<Ulid>
{
	/// <inheritdoc/>
	public override Ulid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		try
		{
			if (reader.TokenType is not JsonTokenType.String and not JsonTokenType.PropertyName)
			{
				throw new JsonException("Expected string or property name");
			}

			if (reader.HasValueSequence)
			{
				var byteSequence = reader.ValueSequence;
				if (byteSequence.Length != Ulid.UlidStringLength)
				{
					throw new JsonException($"Ulid invalid: length must be {Ulid.UlidStringLength}");
				}

				Span<byte> byteSpan = stackalloc byte[Ulid.UlidStringLength];
				byteSequence.CopyTo(byteSpan);
				Ulid.TryParse(byteSpan, null, out var ulid);
				return ulid;
			}
			else
			{
				var byteSpan = reader.ValueSpan;
				if (byteSpan.Length != Ulid.UlidStringLength)
				{
					throw new JsonException($"Ulid invalid: length must be {Ulid.UlidStringLength}");
				}

				Ulid.TryParse(byteSpan, null, out var ulid);
				return ulid;
			}
		}
		catch (IndexOutOfRangeException ex)
		{
			throw new JsonException($"Ulid invalid: length must be {Ulid.UlidStringLength}", ex);
		}
		catch (OverflowException ex)
		{
			throw new JsonException("Ulid invalid: invalid character", ex);
		}
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Ulid ulid, JsonSerializerOptions options)
	{
		Span<byte> ulidString = stackalloc byte[Ulid.UlidStringLength];
		ulid.TryFormat(ulidString, out var _, []);
		writer.WriteStringValue(ulidString);
	}

	/// <inheritdoc/>
	public override void WriteAsPropertyName(Utf8JsonWriter writer, Ulid ulid, JsonSerializerOptions options)
	{
		Span<byte> ulidString = stackalloc byte[Ulid.UlidStringLength];
		ulid.TryFormat(ulidString, out var _, []);
		writer.WritePropertyName(ulidString);
	}

	/// <inheritdoc/>
	public override Ulid ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> Read(ref reader, typeToConvert, options);
}
#endif