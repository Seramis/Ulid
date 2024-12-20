using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ByteAether.Ulid;

public readonly partial struct Ulid
{
	private static readonly byte[] _lastUlid = new byte[_ulidSize];

#if NET9_0_OR_GREATER
	private static readonly Lock _lock = new();
#else
	private static readonly object _lock = new();
#endif

	/// <summary>
	/// Initializes a new instance of the <see cref="ByteAether.Ulid"/> struct using the specified byte array.
	/// </summary>
	/// <param name="bytes">The byte array to initialize the ULID with.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Ulid New(ReadOnlySpan<byte> bytes)
		=> MemoryMarshal.Read<Ulid>(bytes);

	/// <summary>
	/// Creates a new ULID with the current timestamp.
	/// </summary>
	/// <param name="isMonotonic">If true, ensures the ULID is monotonically increasing.</param>
	/// <returns>A new ULID instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Ulid New(bool isMonotonic = true)
		=> New(DateTimeOffset.UtcNow, isMonotonic);

	/// <summary>
	/// Creates a new ULID with the specified timestamp.
	/// </summary>
	/// <param name="dateTimeOffset">The timestamp to use for the ULID.</param>
	/// <param name="isMonotonic">If true, ensures the ULID is monotonically increasing.</param>
	/// <returns>A new ULID instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Ulid New(DateTimeOffset dateTimeOffset, bool isMonotonic = true)
		=> New(dateTimeOffset.ToUnixTimeMilliseconds(), isMonotonic);

	/// <summary>
	/// Creates a new ULID with the specified timestamp.
	/// </summary>
	/// <param name="dateTimeOffset">The timestamp to use for the ULID.</param>
	/// <param name="random" >
	/// A span containing the random component of the ULID. 
	/// It must be at least 10 bytes long, as the last 10 bytes of the ULID are derived from this span.
	/// </param>
	/// <returns>A new ULID instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Ulid New(DateTimeOffset dateTimeOffset, Span<byte> random)
		=> New(dateTimeOffset.ToUnixTimeMilliseconds(), random);

	/// <summary>
	/// Creates a new ULID with the specified timestamp in milliseconds.
	/// </summary>
	/// <param name="timestamp">The timestamp in milliseconds to use for the ULID.</param>
	/// <param name="isMonotonic">If true, ensures the ULID is monotonically increasing.</param>
	/// <returns>A new ULID instance.</returns>
#if NET5_0_OR_GREATER
	[SkipLocalsInit]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Ulid New(long timestamp, bool isMonotonic = true)
	{
		Ulid ulid = default;

		var ulidBytes = MemoryMarshal.CreateSpan(ref Unsafe.As<Ulid, byte>(ref ulid), _ulidSize);

		FillTime(ulidBytes, timestamp);
		FillRandom(ulidBytes, isMonotonic);

		return ulid;
	}

	/// <summary>
	/// Creates a new instance of the <see cref="Ulid"/> struct using the specified timestamp and random byte sequence.
	/// </summary>
	/// <param name="timestamp">
	/// A 64-bit integer representing the timestamp in milliseconds since the Unix epoch (1970-01-01T00:00:00Z).
	/// This value will be encoded into the first 6 bytes of the ULID.
	/// </param>
	/// <param name="random">
	/// A span containing the random component of the ULID. 
	/// It must be at least 10 bytes long, as the last 10 bytes of the ULID are derived from this span.
	/// </param>
	/// <returns>
	/// A new <see cref="Ulid"/> instance composed of the given timestamp and random byte sequence.
	/// </returns>
#if NET5_0_OR_GREATER
	[SkipLocalsInit]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Ulid New(long timestamp, Span<byte> random)
	{
		Ulid ulid = default;

		var ulidBytes = MemoryMarshal.CreateSpan(ref Unsafe.As<Ulid, byte>(ref ulid), _ulidSize);

		FillTime(ulidBytes, timestamp);
		random.CopyTo(ulidBytes[6..]);

		return ulid;
	}

#if NET5_0_OR_GREATER
	[SkipLocalsInit]
#endif
	private static void FillTime(Span<byte> bytes, long timestamp)
	{
		unsafe
		{
			// Get a pointer to the timestamp and convert it to a reference to the first byte.
			ref var firstByte = ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref timestamp));

			if (BitConverter.IsLittleEndian)
			{
				// If the system is little-endian, reverse the bytes to store in big-endian format.
				bytes[5] = Unsafe.Add(ref firstByte, 0);

				bytes[0] = Unsafe.Add(ref firstByte, 5);
				bytes[1] = Unsafe.Add(ref firstByte, 4);
				bytes[2] = Unsafe.Add(ref firstByte, 3);
				bytes[3] = Unsafe.Add(ref firstByte, 2);
				bytes[4] = Unsafe.Add(ref firstByte, 1);
			}
			else
			{
				// If the system is big-endian, just copy the bytes directly.
				bytes[5] = Unsafe.Add(ref firstByte, 7);

				bytes[0] = Unsafe.Add(ref firstByte, 2);
				bytes[1] = Unsafe.Add(ref firstByte, 3);
				bytes[2] = Unsafe.Add(ref firstByte, 4);
				bytes[3] = Unsafe.Add(ref firstByte, 5);
				bytes[4] = Unsafe.Add(ref firstByte, 6);
			}
		}
	}

#if NET5_0_OR_GREATER
	[SkipLocalsInit]
#endif
	private static void FillRandom(Span<byte> bytes, bool isMonotonic)
	{
		if (!isMonotonic)
		{
			RandomNumberGenerator.Fill(bytes[6..]);
			return;
		}

		var lastUlidSpan = _lastUlid.AsSpan();

		lock (_lock)
		{
			// If the timestamp is the same or lesser than the last one, increment the last ULID by one
			if (bytes[..6].SequenceCompareTo(lastUlidSpan[..6]) <= 0)
			{
				var i = _ulidSize;
				while (i > 0)
				{
					if (++_lastUlid[--i] != 0)
					{
						break;
					}
				}
			}
			// Otherwise, generate a new ULID
			else
			{
				bytes[..6].CopyTo(lastUlidSpan);
				RandomNumberGenerator.Fill(lastUlidSpan[6..]);
			}

			_lastUlid.CopyTo(bytes);
		}
	}
}
