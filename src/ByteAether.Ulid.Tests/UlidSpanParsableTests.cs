using System.Text;

namespace ByteAether.Ulid.Tests;

public class UlidSpanParsableTests
{
	private static readonly Ulid _ulid = Ulid.New();

	[Fact]
	public void Parse_ValidString_ReturnsTrue()
	{
		var result = Ulid.Parse(_ulid.ToString());

		Assert.Equal(_ulid, result);
	}

	[Fact]
	public void Parse_ValidSpan_ReturnsTrue()
	{
		var result = Ulid.Parse(_ulid.ToString().AsSpan());

		Assert.Equal(_ulid, result);
	}

	[Fact]
	public void Parse_ValidByteArray_ReturnsTrue()
	{
		var result = Ulid.Parse(Encoding.UTF8.GetBytes(_ulid.ToString()));

		Assert.Equal(_ulid, result);
	}

	[Fact]
	public void TryParse_ValidString_ReturnsTrue()
	{
		var result = Ulid.TryParse(_ulid.ToString(), null, out var ulid);

		Assert.True(result);
		Assert.NotEqual(default, ulid);
		Assert.Equal(_ulid, ulid);
	}

	[Fact]
	public void TryParse_ValidSpan_ReturnsTrue()
	{
		var result = Ulid.TryParse(_ulid.ToString().AsSpan(), null, out var ulid);

		Assert.True(result);
		Assert.NotEqual(default, ulid);
		Assert.Equal(_ulid, ulid);
	}

	[Fact]
	public void TryParse_ValidByteArray_ReturnsTrue()
	{
		var result = Ulid.TryParse(Encoding.UTF8.GetBytes(_ulid.ToString()), null, out var ulid);

		Assert.True(result);
		Assert.NotEqual(default, ulid);
		Assert.Equal(_ulid, ulid);
	}

	[Fact]
	public void TryParse_NullString_ReturnsFalse()
	{
		string? ulidString = null;

		var result = Ulid.TryParse(ulidString, null, out var ulid);

		Assert.False(result);
		Assert.Equal(default, ulid);
	}

	[Theory]
	[InlineData("")]
	[InlineData("invalid")]
	[InlineData("asd")]
	[InlineData("UUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU")]
	[InlineData(",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,")]
	[InlineData("2222222222222222222222222222222222222")]
	public void Parse_InvalidString_ThrowsFormatException(string inputString)
	{
		Assert.Throws<FormatException>(() => Ulid.Parse(inputString));
		Assert.Throws<FormatException>(() => Ulid.Parse(Encoding.UTF8.GetBytes(inputString)));
		Assert.Throws<FormatException>(() => Ulid.Parse(inputString.AsSpan()));
	}

	[Theory]
	[InlineData("")]
	[InlineData("invalid")]
	[InlineData("asd")]
	[InlineData("UUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU")]
	[InlineData(",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,")]
	[InlineData("2222222222222222222222222222222222222")]
	public void TryParse_InvalidString_ReturnsFalse(string inputString)
	{
		var result = Ulid.TryParse(inputString, null, out var ulid);
		Assert.False(result);
		Assert.Equal(default, ulid);
	}
}
