using System.Text.Json;

namespace ByteAether.Ulid.Tests;

public class UlidJsonConverterTests
{
	private class TestDto
	{
		public Ulid UlidProperty { get; set; }
	}

	private static JsonSerializerOptions _jsonOptions
		=> new()
		{
			Converters = { new UlidJsonConverter() }
		};

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Serialize_UlidToJsonString(bool withOptions)
	{
		var ulid = Ulid.New();
		var jsonString = JsonSerializer.Serialize(ulid, withOptions ? _jsonOptions : null);

		Assert.Equal($"\"{ulid.ToString()}\"", jsonString);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Deserialize_JsonStringToUlid(bool withOptions)
	{
		var ulid = Ulid.New();
		var jsonString = $"\"{ulid.ToString()}\"";

		var deserializedUlid = JsonSerializer.Deserialize<Ulid>(jsonString, withOptions ? _jsonOptions : null);

		Assert.Equal(ulid, deserializedUlid);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Deserialize_BadUlidString(bool withOptions)
	{
		var ulid = Ulid.New();
		var jsonString = $"\"{ulid.ToString()[1..]}\"";

		try
		{
			var deserializedUlid = JsonSerializer.Deserialize<Ulid>(jsonString, withOptions ? _jsonOptions : null);
			throw new Exception("Test should fail here: no exception were thrown");
		}
		catch (JsonException)
		{
			// This is success
		}
		catch (Exception e)
		{
			throw new Exception($"Test should fail here: Got exception {e}");
		}
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Serialize_DtoToJson(bool withOptions)
	{
		var dto = new TestDto { UlidProperty = Ulid.New() };
		var jsonString = $"{{\"UlidProperty\":\"{dto.UlidProperty.ToString()}\"}}";

		var result = JsonSerializer.Serialize(dto, withOptions ? _jsonOptions : null);

		Assert.Equal(jsonString, result);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Deserialize_GoodJsonToDto(bool withOptions)
	{
		var ulid = Ulid.New();
		var jsonString = $"{{\"UlidProperty\": \"{ulid.ToString()}\"}}";

		var result = JsonSerializer.Deserialize<TestDto>(jsonString, withOptions ? _jsonOptions : null);

		Assert.Equal(ulid, result!.UlidProperty);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Serialize_UlidAsPropertyName(bool withOptions)
	{
		var ulid = Ulid.New();
		var dictionary = new Dictionary<Ulid, string>
		{
			{ ulid, "value" }
		};

		var jsonString = JsonSerializer.Serialize(dictionary, withOptions ? _jsonOptions : null);

		Assert.Contains($"\"{ulid.ToString()}\":\"value\"", jsonString);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Deserialize_UlidAsPropertyName(bool withOptions)
	{
		var ulid = Ulid.New();
		var jsonString = $"{{\"{ulid.ToString()}\":\"value\"}}";

		var deserializedDictionary = JsonSerializer.Deserialize<Dictionary<Ulid, string>>(jsonString, withOptions ? _jsonOptions : null);

		Assert.Equal("value", deserializedDictionary![ulid]);
	}
}