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
	[CombinatorialData]
	public void New_ShouldGenerateUniqueUlids(Ulid.Monotonicity monotonicity)
	{
		// Act
		var ulid1 = Ulid.New(monotonicity);
		var ulid2 = Ulid.New(monotonicity);

		// Assert
		Assert.NotEqual(ulid1, ulid2);
	}

	[Theory]
	[CombinatorialData]
	public void New_WithDateTime_ShouldGenerateUniqueUlids(Ulid.Monotonicity monotonicity)
	{
		// Arrange
		var dateTimeOffset = DateTimeOffset.UtcNow;
		var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();

		// Act
		var ulid1 = Ulid.New(dateTimeOffset, monotonicity);
		var ulid2 = Ulid.New(dateTimeOffset, monotonicity);

		// Assert
		Assert.NotEqual(ulid1, ulid2);

		Assert.True(ulid1.Time.ToUnixTimeMilliseconds() <= ulid2.Time.ToUnixTimeMilliseconds());
		Assert.True(timestamp <= ulid1.Time.ToUnixTimeMilliseconds());
		Assert.True(timestamp <= ulid2.Time.ToUnixTimeMilliseconds());

		if (monotonicity != Ulid.Monotonicity.NonMonotonic)
		{
			Assert.True(MemoryExtensions.SequenceCompareTo(ulid1.AsByteSpan(), ulid2.AsByteSpan()) < 0);
			Assert.True(ulid1 < ulid2);
		}
	}

	[Theory]
	[CombinatorialData]
	public void New_WithTimestamp_ShouldGenerateUniqueUlids(Ulid.Monotonicity monotonicity)
	{
		// Arrange
		var dateTimeOffset = DateTimeOffset.UtcNow;
		var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();

		// Act
		var ulid1 = Ulid.New(timestamp, monotonicity);
		var ulid2 = Ulid.New(timestamp, monotonicity);

		// Assert
		Assert.NotEqual(ulid1, ulid2);

		Assert.True(ulid1.Time.ToUnixTimeMilliseconds() <= ulid2.Time.ToUnixTimeMilliseconds());
		Assert.True(timestamp <= ulid1.Time.ToUnixTimeMilliseconds());
		Assert.True(timestamp <= ulid2.Time.ToUnixTimeMilliseconds());

		if (monotonicity != Ulid.Monotonicity.NonMonotonic)
		{
			Assert.True(MemoryExtensions.SequenceCompareTo(ulid1.AsByteSpan(), ulid2.AsByteSpan()) < 0);
			Assert.True(ulid1 < ulid2);
		}
	}

	[Fact]
	public void New_WithDateTimeAndRandom_ShouldGenerateSameUlid()
	{
		// Arrange
		var dateTimeOffset = DateTimeOffset.UtcNow;
		var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
		var random = new byte[10];

		// Act
		var ulid1 = Ulid.New(dateTimeOffset, random);
		var ulid2 = Ulid.New(dateTimeOffset, random);

		// Assert
		Assert.Equal(ulid1, ulid2);

		Assert.Equal(timestamp, ulid1.Time.ToUnixTimeMilliseconds());
		Assert.Equal(timestamp, ulid2.Time.ToUnixTimeMilliseconds());

		Assert.Equal(ulid1.Time, ulid2.Time);
		Assert.Equal(ulid1.TimeBytes.ToArray(), ulid2.TimeBytes.ToArray());

		Assert.Equal(ulid1.Random.ToArray(), ulid2.Random.ToArray());
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
		Assert.Equal(ulid1, ulid2);

		Assert.Equal(timestamp, ulid1.Time.ToUnixTimeMilliseconds());
		Assert.Equal(timestamp, ulid2.Time.ToUnixTimeMilliseconds());

		Assert.Equal(ulid1.Time, ulid2.Time);
		Assert.Equal(ulid1.TimeBytes.ToArray(), ulid2.TimeBytes.ToArray());

		Assert.Equal(ulid1.Random.ToArray(), ulid2.Random.ToArray());
	}

	[Theory]
	[CombinatorialData]
	public void New_WithTimestampAndMonotonicSet_ShouldGenerateUniqueUlids(Ulid.Monotonicity? monotonicity, Ulid.Monotonicity defaultMonotonicity)
	{
		// Arrange
		Ulid.DefaultMonotonicity = defaultMonotonicity;
		var dateTimeOffset = DateTimeOffset.UtcNow;
		var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();

		// Act
		var ulid1 = Ulid.New(timestamp, monotonicity);
		var ulid2 = Ulid.New(timestamp, monotonicity);

		// Assert
		Assert.NotEqual(ulid1, ulid2);

		Assert.True(ulid1.Time.ToUnixTimeMilliseconds() <= ulid2.Time.ToUnixTimeMilliseconds());
		Assert.True(timestamp <= ulid1.Time.ToUnixTimeMilliseconds());
		Assert.True(timestamp <= ulid2.Time.ToUnixTimeMilliseconds());

		if ((monotonicity ?? defaultMonotonicity) != Ulid.Monotonicity.NonMonotonic)
		{
			Assert.True(MemoryExtensions.SequenceCompareTo(ulid1.AsByteSpan(), ulid2.AsByteSpan()) < 0);
			Assert.True(ulid1 < ulid2);
		}
	}
}