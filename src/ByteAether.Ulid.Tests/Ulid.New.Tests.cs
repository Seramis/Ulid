namespace ByteAether.Ulid.Tests;

public class UlidNewTests
{
	[Fact]
	public void ToByteArray_ShouldConvertToByteArrayAndBack()
	{
		// Arrange
		var ulid = Ulid.New();

		// Act
		var byteArray = ulid.ToByteArray();
		var ulidFromBytes = Ulid.New(byteArray);

		// Assert
		Assert.Equal(16, byteArray.Length);
		Assert.Equal(ulid, ulidFromBytes);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void New_ShouldGenerateUniqueUlids(bool isMonotonic)
	{
		// Act
		var ulid1 = Ulid.New(isMonotonic);
		var ulid2 = Ulid.New(isMonotonic);

		// Assert
		Assert.NotEqual(ulid1, ulid2);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void New_WithDateTime_ShouldGenerateUniqueUlids(bool isMonotonic)
	{
		// Arrange
		var dateTimeOffset = DateTimeOffset.UtcNow;

		// Act
		var ulid1 = Ulid.New(dateTimeOffset, isMonotonic);
		var ulid2 = Ulid.New(dateTimeOffset, isMonotonic);

		// Assert
		Assert.Equal(dateTimeOffset.ToUnixTimeMilliseconds(), ulid1.Time.ToUnixTimeMilliseconds());
		Assert.Equal(ulid1.Time, ulid2.Time);
		Assert.Equal(ulid1.TimeBytes.ToArray(), ulid2.TimeBytes.ToArray());
		Assert.NotEqual(ulid1.Random.ToArray(), ulid2.Random.ToArray());
		Assert.NotEqual(ulid1, ulid2);
	}

	[Fact]
	public void New_WithDateTimeAndRandom_ShouldGenerateSameUlid()
	{
		// Arrange
		var dateTimeOffset = DateTimeOffset.UtcNow;
		var random = new byte[10];

		// Act
		var ulid1 = Ulid.New(dateTimeOffset, random);
		var ulid2 = Ulid.New(dateTimeOffset, random);

		// Assert
		Assert.Equal(dateTimeOffset.ToUnixTimeMilliseconds(), ulid1.Time.ToUnixTimeMilliseconds());
		Assert.Equal(ulid1.Time, ulid2.Time);
		Assert.Equal(ulid1.TimeBytes.ToArray(), ulid2.TimeBytes.ToArray());
		Assert.Equal(ulid1.Random.ToArray(), ulid2.Random.ToArray());
		Assert.Equal(ulid1, ulid2);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void New_WithTimestamp_ShouldGenerateUniqueUlids(bool isMonotonic)
	{
		// Arrange
		var dateTimeOffset = DateTimeOffset.UtcNow;
		var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();

		// Act
		var ulid1 = Ulid.New(timestamp, isMonotonic);
		var ulid2 = Ulid.New(timestamp, isMonotonic);

		// Assert
		Assert.Equal(timestamp, ulid1.Time.ToUnixTimeMilliseconds());
		Assert.Equal(ulid1.Time, ulid2.Time);
		Assert.Equal(ulid1.TimeBytes.ToArray(), ulid2.TimeBytes.ToArray());
		Assert.NotEqual(ulid1.Random.ToArray(), ulid2.Random.ToArray());
		Assert.NotEqual(ulid1, ulid2);
	}

	[Fact]
	public void New_WithTimestampAndRandom_ShouldGenerateSameUlid()
	{
		// Arrange
		var dateTimeOffset = DateTimeOffset.UtcNow;
		var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
		var random = new byte[10];

		// Act
		var ulid1 = Ulid.New(timestamp, random);
		var ulid2 = Ulid.New(timestamp, random);

		// Assert
		Assert.Equal(timestamp, ulid1.Time.ToUnixTimeMilliseconds());
		Assert.Equal(ulid1.Time, ulid2.Time);
		Assert.Equal(ulid1.TimeBytes.ToArray(), ulid2.TimeBytes.ToArray());
		Assert.Equal(ulid1.Random.ToArray(), ulid2.Random.ToArray());
		Assert.Equal(ulid1, ulid2);
	}
}
