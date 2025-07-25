#if NETSTANDARD2_0
using System.Buffers;
using System.Security.Cryptography;

namespace ByteAether.Ulid;

internal static class RandomNumberGeneratorExtensions
{
	// In NetStandard 2.0, RandomNumberGenerator.GetBytes() does not support Span<byte> overloads.
	public static void GetBytes(this RandomNumberGenerator rng, Span<byte> buffer)
	{
		var rndInc = ArrayPool<byte>.Shared.Rent(buffer.Length);
		rng.GetBytes(rndInc, 0, buffer.Length);
		new ReadOnlySpan<byte>(rndInc, 0, buffer.Length).CopyTo(buffer);
	}
}
#endif