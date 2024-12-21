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

	[Fact]
	public void ToString_WrongLettersReplaced()
	{
		// Crockford's Base32 subtitution test
		var inputChars = new char[] { 'O', 'o', 'I', 'i', 'L', 'l', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
		var outputChars = new char[] { '0', '0', '1', '1', '1', '1', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z' };

		for (var i = 0; i < inputChars.Length; i++)
		{
			var inputString = $"2222222{inputChars[i]}222222222222222222";
			var outputString = $"2222222{outputChars[i]}222222222222222222";

			var ulid = Ulid.Parse(inputString);
			var resultString = ulid.ToString();

			Assert.Equal(outputString, resultString);
		}
	}
}
