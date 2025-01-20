using System.Runtime.CompilerServices;

namespace ByteAether.Ulid;

public readonly partial struct Ulid
{
	/// <summary>
	/// Validates if the given string is a valid ULID.
	/// </summary>
	/// <param name="ulidString">The ULID string to validate.</param>
	/// <returns>
	/// <c>true</c> if the string is a valid ULID, <c>false</c> otherwise.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValid(string ulidString) => IsValid(ulidString.AsSpan());

	/// <summary>
	/// Validates if the given span of characters is a valid ULID.
	/// </summary>
	/// <param name="ulidString">The ULID character span to validate.</param>
	/// <returns>
	/// <c>true</c> if the character span is a valid ULID, <c>false</c> otherwise.
	/// </returns>
#if NET5_0_OR_GREATER
	[SkipLocalsInit]
#endif
	public static bool IsValid(ReadOnlySpan<char> ulidString)
	{
		// Length check
		if (ulidString.Length != UlidStringLength)
		{
			return false;
		}

		// First character check
		var isValidFirstChar = false;
		for (var i = 7; i >= 0; --i)
		{
			if (ulidString[0] == _base32Chars[i])
			{
				isValidFirstChar = true;
				break;
			}
		}

		if (!isValidFirstChar)
		{
			return false;
		}

		// Rest of the characters check
		foreach (var c in ulidString[1..])
		{
			if (!_base32Chars.Contains(c))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Validates if the given byte array represents a valid ULID.
	/// </summary>
	/// <param name="ulidBytes">The byte array to validate.</param>
	/// <returns>
	/// <c>true</c> if the byte array is a valid ULID, <c>false</c> otherwise.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValid(ReadOnlySpan<byte> ulidBytes) => ulidBytes.Length == _ulidSize;
}
