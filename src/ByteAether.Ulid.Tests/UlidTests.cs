namespace ByteAether.Ulid.Tests;

public class UlidTests
{
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void New_GeneratesUniqueUlid(bool isMonotonic)
	{
		var ulid1 = Ulid.New(isMonotonic);
		var ulid2 = Ulid.New(isMonotonic);

		Assert.NotEqual(ulid1, ulid2);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void New_WithGivenDateTime_GeneratesUniqueUlid(bool isMonotonic)
	{
		var dateTimeOffset = DateTimeOffset.UtcNow;

		var ulid1 = Ulid.New(dateTimeOffset, isMonotonic);
		var ulid2 = Ulid.New(dateTimeOffset, isMonotonic);

		Assert.Equal(ulid1.Time, ulid2.Time);
		Assert.NotEqual(ulid1.Random.ToArray(), ulid2.Random.ToArray());

		Assert.NotEqual(ulid1, ulid2);
	}

	[Fact]
	public void New_WithGivenDateTimeAndRandom_GeneratesSameUlid()
	{
		var dateTimeOffset = DateTimeOffset.UtcNow;
		var random = new byte[10];

		var ulid1 = Ulid.New(dateTimeOffset, random);
		var ulid2 = Ulid.New(dateTimeOffset, random);

		Assert.Equal(ulid1.Time, ulid2.Time);
		Assert.Equal(ulid1.Random.ToArray(), ulid2.Random.ToArray());

		Assert.Equal(ulid1, ulid2);
	}

	[Fact]
	public void ToByteArray_ByteArrayAndBack()
	{
		var ulid = Ulid.New();
		var byteArray = ulid.ToByteArray();
		var ulid2 = Ulid.New(byteArray);

		Assert.Equal(16, byteArray.Length);
		Assert.Equal(ulid, ulid2);
	}

	[Fact]
	public void ToGuid_GuidAndBack()
	{
		var ulid = Ulid.New();
		var guid = ulid.ToGuid();
		var ulid2 = Ulid.New(guid);

		Assert.NotEqual(Guid.Empty, guid);
		Assert.Equal(ulid, ulid2);
	}

	[Fact]
	public void ToString_StringAndBack()
	{
		var ulid = Ulid.New();
		var ulidString = ulid.ToString();
		var ulid2 = Ulid.Parse(ulidString);

		Assert.Equal(26, ulidString.Length);
		Assert.Equal(ulid, ulid2);
	}
}
