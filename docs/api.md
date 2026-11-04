# API reference

Reference for the `Ulid` type in `ByteAether.Ulid`. For help choosing generation settings, see [generation options](generation-options.md). For database mappings, see [storage formats](storage-formats.md).

The `Ulid` implementation provides the following properties and methods:

## Creation

- `Ulid.New(GenerationOptions? options = null)`\
  Generates a new ULID. Without `options`, `Ulid.DefaultGenerationOptions` applies.
- `Ulid.New(DateTimeOffset dateTimeOffset, GenerationOptions? options = null)`\
  Generates a new ULID with the specified `DateTimeOffset` as the timestamp.
- `Ulid.New(long timestamp, GenerationOptions? options = null)`\
  Generates a new ULID with the specified Unix timestamp in milliseconds as the timestamp.
- `Ulid.New(DateTimeOffset dateTimeOffset, ReadOnlySpan<byte> random)`\
  Generates a new ULID from the specified `DateTimeOffset` and the first 10 bytes of `random`. Throws `ArgumentException` if `random` is shorter than 10 bytes.
- `Ulid.New(long timestamp, ReadOnlySpan<byte> random)`\
  Generates a new ULID from the specified Unix timestamp in milliseconds and the first 10 bytes of `random`. Throws `ArgumentException` if `random` is shorter than 10 bytes.
- `Ulid.New(ReadOnlySpan<byte> bytes)`\
  Creates a ULID from the first 16 bytes of `bytes`, in the order `AsByteSpan()` returns them. Throws `ArgumentException` if `bytes` is shorter than 16 bytes.
- `Ulid.New(Guid guid)`\
  Creates a ULID from a `Guid`. Inverse of `ToGuid()`; see [Conversions](#conversions).
- `Ulid.MinAt(DateTimeOffset datetime)`\
  Creates the minimum possible ULID value for the specified `DateTimeOffset`.
- `Ulid.MinAt(long timestamp)`\
  Creates the minimum possible ULID value for the specified Unix timestamp in milliseconds (`long`).
- `Ulid.MaxAt(DateTimeOffset datetime)`\
  Creates the maximum possible ULID value for the specified `DateTimeOffset`.
- `Ulid.MaxAt(long timestamp)`\
  Creates the maximum possible ULID value for the specified Unix timestamp in milliseconds (`long`).

## Validation

- `Ulid.IsValid(string ulidString)`\
  Validates whether the specified string represents a valid ULID.
- `Ulid.IsValid(ReadOnlySpan<char> ulidString)`\
  Validates whether the specified span of characters represents a valid ULID.
- `Ulid.IsValid(ReadOnlySpan<byte> utf8Text)`\
  Validates whether the specified UTF-8 encoded byte span represents a valid ULID string.

A valid ULID string is exactly 26 characters of [Crockford's Base32](https://www.crockford.com/base32.html) alphabet, read case-insensitively, with `I` and `L` accepted as `1`, and `O` as `0`. The first character must be between `0` and `7`, as larger values do not fit into 128 bits.

## Parsing

- `Ulid.Parse(ReadOnlySpan<char> chars, IFormatProvider? provider = null)`\
  Parses a ULID from a character span.
- `Ulid.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Ulid result)`\
  Tries to parse a ULID from a character span. Returns `true` if successful.
- `Ulid.Parse(string s, IFormatProvider? provider = null)`\
  Parses a ULID from a string.
- `Ulid.TryParse(string? s, IFormatProvider? provider, out Ulid result)`\
  Tries to parse a ULID from a string. Returns `true` if successful.
- `Ulid.Parse(ReadOnlySpan<byte> bytes, IFormatProvider? provider = null)`\
  Parses a ULID from a UTF-8 encoded byte span.
- `Ulid.TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Ulid result)`\
  Tries to parse a ULID from a UTF-8 encoded byte span. Returns `true` if successful.

Parsing applies the same rules as validation above. The `IFormatProvider` parameter is ignored. `Parse` throws a `FormatException` for input that `IsValid` rejects, while `TryParse` returns `false` without throwing.

## Properties

- `Ulid.MinValue`\
  Represents an empty ULID, equivalent to `default(Ulid)` and `Ulid.New(new byte[16])`.
- `Ulid.MaxValue`\
  Represents the maximum possible value for a ULID (all bytes set to `0xFF`).
- `Ulid.Empty`\
  Alias for `Ulid.MinValue`.
- `Ulid.DefaultGenerationOptions`\
  Gets or sets the global default `GenerationOptions` configuration for ULID generation when no options are provided by the `Ulid.New(...)` call.
- `.Time`\
  Gets the timestamp component of the ULID as a `DateTimeOffset`. A ULID stores a 48-bit millisecond timestamp (up to the year 10889), but `DateTimeOffset` ends at the year 9999, so `.Time` throws `ArgumentOutOfRangeException` for valid ULIDs from `76EZ91ZQ000000000000000000` upwards (including `Ulid.MaxValue`). Use `.TimeBytes` to read the raw timestamp without that limit.
- `.TimeBytes`\
  Gets the timestamp component of the ULID as a `ReadOnlySpan<byte>`.
- `.Random`\
  Gets the random component of the ULID as a `ReadOnlySpan<byte>`.

## Conversions

- `.AsByteSpan()`\
  Provides a `ReadOnlySpan<byte>` representing the contents of the ULID.
- `.ToByteArray()`\
  Converts the ULID to a byte array.
- `.ToGuid()`\
  Converts the ULID to a `Guid`. The conversion accounts for the mixed-endian layout of `Guid`, so `ToGuid().ToString()` shows the ULID's bytes in their original order (for example `01a0ed09-3a00-5d67-6247-7acbf9130587` for the bytes `01A0ED093A005D6762477ACBF9130587`). `Ulid.New(Guid)` reverses it.
- `.ToString(string? format = null, IFormatProvider? formatProvider = null)`\
  Converts the ULID to a canonical string representation. Format arguments are ignored.
- Provides implicit operators to and from `Guid` and `string`.

## Comparison

- Supports all comparison operators:\
  `==`, `!=`, `<`, `<=`, `>`, `>=`.
- Implements standard comparison and equality methods:\
  `CompareTo`, `Equals`, `GetHashCode`.
- Implements these .NET interfaces on every target framework:\
  `IEquatable<Ulid>`, `IEqualityComparer<Ulid>`, `IComparable`, `IComparable<Ulid>`, `IFormattable`.
- Additionally on .NET 6.0+: `ISpanFormattable`.
- Additionally on .NET 7.0+: `IMinMaxValue<Ulid>`, `IEqualityOperators<Ulid, Ulid, bool>`, `IComparisonOperators<Ulid, Ulid, bool>`, `IParsable<Ulid>`, `ISpanParsable<Ulid>`.
- Additionally on .NET 8.0+: `IUtf8SpanFormattable`, `IUtf8SpanParsable<Ulid>`.

## GenerationOptions

The `GenerationOptions` class holds the generation settings and the state of the monotonic sequence.

Configurable properties:

- `Monotonicity`\
  Defines the monotonic strategy when generating multiple ULIDs within the same millisecond. Each instance maintains its own state (the last generated ULID), guarded by a low-latency test-and-test-and-set (TTAS) lock with CAS acquisition, so that ordering is strictly sequential across threads. Options include:
  - `NonMonotonic`: Generates fully random 80-bit payloads without state tracking.
  - `MonotonicIncrement` (Default): Adds 1 to the previous random payload when a ULID is generated in the same millisecond as the previous one.
  - `MonotonicRandom1Byte`, `MonotonicRandom2Byte`, `MonotonicRandom3Byte`, `MonotonicRandom4Byte`: Adds a random increment to the previous random payload. The increment is a random 1 to 4 byte value (each byte `00` to `FF`) plus 1, so it ranges from 1 to 256, 65 536, 16 777 216, or 4 294 967 296. Less predictable than `MonotonicIncrement`, and still strictly ordered.

- `InitialRandomSource`\
  An `IRandomProvider` for generating the random bytes of a ULID. The default `CryptographicallySecureRandomProvider` uses a cryptographically secure generator.

- `IncrementRandomSource`\
  An `IRandomProvider` that provides randomness for monotonic random increments. The default `PseudoRandomProvider` is a faster, non-cryptographically secure source optimized for this specific purpose.

This library comes with two default `IRandomProvider` implementations:

- `CryptographicallySecureRandomProvider`\
  Uses `System.Security.Cryptography.RandomNumberGenerator`.
- `PseudoRandomProvider`\
  A faster, non-cryptographically secure option based on `System.Random`, suited to performance-critical code where cryptographic security is not needed for random increments.

Custom `IRandomProvider` implementations can also be created.
