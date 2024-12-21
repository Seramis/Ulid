namespace ByteAether.Ulid.Tests;

public class UlidUtf8SpanFormattableTests
{
	[Theory]
	[InlineData(26)]
	[InlineData(27)]
	public void TryFormat_ValidSpan_ReturnsTrue(int spanSize)
	{
		var ulid = Ulid.New();
		Span<byte> span = new byte[spanSize];

		var result = ulid.TryFormat(span, out var bytesWritten, []);

		Assert.True(result);
		Assert.Equal(26, bytesWritten);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(25)]
	public void TryFormat_InvalidSpan_ReturnsFalse(int spanSize)
	{
		var ulid = Ulid.New();
		Span<byte> span = new byte[spanSize];

		var result = ulid.TryFormat(span, out var bytesWritten, []);

		Assert.False(result);
		Assert.Equal(0, bytesWritten);
	}
}
