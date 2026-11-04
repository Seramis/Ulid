# ByteAether.Ulid

[![License](https://img.shields.io/github/license/ByteAether/Ulid?logo=github&label=License)](https://github.com/ByteAether/Ulid/blob/main/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/ByteAether.Ulid?logo=nuget&label=Version)](https://www.nuget.org/packages/ByteAether.Ulid/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ByteAether.Ulid?logo=nuget&label=Downloads)](https://www.nuget.org/packages/ByteAether.Ulid/)

A .NET implementation of ULIDs (Universally Unique Lexicographically Sortable Identifiers) that follows the [official ULID specification](https://github.com/ulid/spec). Monotonic generation is thread-safe and never throws on overflow. Companion packages map ULIDs to Entity Framework Core, LinqToDB, Dapper, and ASP.NET Core.

Guides, benchmarks, and integration examples are in the [GitHub repository](https://github.com/ByteAether/Ulid).

## ✨ Features

Each runtime below is a separate build target, and each binary contains optimizations specific to that target.

![.NET AOT Ready](https://img.shields.io/badge/.NET-AOT_Ready-blue)
![.NET 10.0](https://img.shields.io/badge/.NET-10.0-brightgreen)
![.NET 9.0](https://img.shields.io/badge/.NET-9.0-brightgreen)
![.NET 8.0](https://img.shields.io/badge/.NET-8.0-brightgreen)
![.NET 7.0](https://img.shields.io/badge/.NET-7.0-brightgreen)
![.NET 6.0](https://img.shields.io/badge/.NET-6.0-brightgreen)
![.NET 5.0](https://img.shields.io/badge/.NET-5.0-brightgreen)
![.NET Standard 2.1](https://img.shields.io/badge/.NET-Standard_2.1-brightgreen)
![.NET Standard 2.0](https://img.shields.io/badge/.NET-Standard_2.0-brightgreen)

- Monotonic generation is **thread-safe**: IDs generated in sequence strictly increase, including across threads.
- The string and binary formats, ordering, and parsing follow the **ULID specification**.
- **Never throws** on random-part overflow. The timestamp advances by one millisecond instead, following the workaround from [spec issue #39](https://github.com/ulid/spec/issues/39#issuecomment-2252145597). This is the one deliberate deviation from the specification, which requires an error.
- Converts to and from `Guid`, [Crockford's Base32](https://www.crockford.com/base32.html) strings, and byte arrays. `AsByteSpan()` gives **zero-allocation** access to the bytes (on .NET Standard 2.0 it returns a copy).
- `Ulid.MinAt()` and `Ulid.MaxAt()` build the smallest and largest ULID for a timestamp, for **time-range** filtering.
- **EF Core, LinqToDB, and Dapper** mappings in four storage formats.
- Compatible with **Native AOT**.

## 📦 Packages

- [`ByteAether.Ulid`](https://www.nuget.org/packages/ByteAether.Ulid/) is the `Ulid` type with generation, parsing, conversion, and JSON serialization. Targets .NET 10.0 to .NET Standard 2.0.
- [`ByteAether.Ulid.EntityFrameworkCore`](https://www.nuget.org/packages/ByteAether.Ulid.EntityFrameworkCore/) maps ULIDs in Entity Framework Core. Requires EF Core 6.0.0+.
- [`ByteAether.Ulid.linq2db`](https://www.nuget.org/packages/ByteAether.Ulid.linq2db/) maps ULIDs in LinqToDB. Requires LinqToDB 6.0.0+.
- [`ByteAether.Ulid.Dapper`](https://www.nuget.org/packages/ByteAether.Ulid.Dapper/) maps ULIDs in Dapper. Requires Dapper 2.0.4+.
- [`ByteAether.Ulid.AspNetCore`](https://www.nuget.org/packages/ByteAether.Ulid.AspNetCore/) adds a `:ulid` route constraint, link generation, and OpenAPI schemas. Requires ASP.NET Core 6.0+.
- [`ByteAether.Ulid.Cli`](https://www.nuget.org/packages/ByteAether.Ulid.Cli/) generates and inspects ULIDs in a terminal or script. Requires .NET 6.0+.

## 💾 Installation

Install the latest stable package via NuGet:
```sh
dotnet add package ByteAether.Ulid
```
To install a specific [preview version](https://www.nuget.org/packages/ByteAether.Ulid/absoluteLatest), use the `--version` option:
```sh
dotnet add package ByteAether.Ulid --version <VERSION_NUMBER>
```

## 🚀 Usage

Generate, convert, and parse a ULID:
```csharp
using System;
using ByteAether.Ulid;

// Create a new ULID
var ulid = Ulid.New();

// Convert to byte array and back
byte[] byteArray = ulid.ToByteArray();
var ulidFromByteArray = Ulid.New(byteArray);

// Convert to GUID and back
Guid guid = ulid.ToGuid();
var ulidFromGuid = Ulid.New(guid);

// Convert to string and back
string ulidString = ulid.ToString();
var ulidFromString = Ulid.Parse(ulidString);

Console.WriteLine($"ULID: {ulid}, GUID: {guid}, String: {ulidString}");
```

### Time-range filtering

`Ulid.MinAt()` and `Ulid.MaxAt()` produce the smallest and largest possible ULIDs for a time window. The same boundaries work for in-memory collections, queryable data sources, and parameterized queries:

```csharp
Ulid minBoundary = Ulid.MinAt(DateTimeOffset.UtcNow.AddDays(-7));
Ulid maxBoundary = Ulid.MaxAt(DateTimeOffset.UtcNow);

var filteredItems = items
	.Where(item => item.Id >= minBoundary && item.Id <= maxBoundary);
```

Database-side range comparisons and `ORDER BY` follow the ULID order only if the storage format and the database engine preserve its byte order. See [storage formats](https://github.com/ByteAether/Ulid/blob/main/docs/storage-formats.md).

### Generation options

Pass a `GenerationOptions` instance to set the monotonicity and the source of randomness. The instance owns the monotonic sequence, so create it once and reuse it:

```csharp
using ByteAether.Ulid;
using static ByteAether.Ulid.Ulid.GenerationOptions;

var options = new Ulid.GenerationOptions
{
	Monotonicity = MonotonicityOptions.MonotonicRandom2Byte
};

var first = Ulid.New(options);
var second = Ulid.New(options);
```

Application-wide defaults and random sources are described in [generation options](https://github.com/ByteAether/Ulid/blob/main/docs/generation-options.md).

## ⚙️ API

The `Ulid` implementation provides the following properties and methods:

### Creation

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

### Validation

- `Ulid.IsValid(string ulidString)`\
  Validates whether the specified string represents a valid ULID.
- `Ulid.IsValid(ReadOnlySpan<char> ulidString)`\
  Validates whether the specified span of characters represents a valid ULID.
- `Ulid.IsValid(ReadOnlySpan<byte> utf8Text)`\
  Validates whether the specified UTF-8 encoded byte span represents a valid ULID string.

A valid ULID string is exactly 26 characters of [Crockford's Base32](https://www.crockford.com/base32.html) alphabet, read case-insensitively, with `I` and `L` accepted as `1`, and `O` as `0`. The first character must be between `0` and `7`, as larger values do not fit into 128 bits.

### Parsing

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

### Properties

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

### Conversions

- `.AsByteSpan()`\
  Provides a `ReadOnlySpan<byte>` representing the contents of the ULID.
- `.ToByteArray()`\
  Converts the ULID to a byte array.
- `.ToGuid()`\
  Converts the ULID to a `Guid`. The conversion accounts for the mixed-endian layout of `Guid`, so `ToGuid().ToString()` shows the ULID's bytes in their original order (for example `01a0ed09-3a00-5d67-6247-7acbf9130587` for the bytes `01A0ED093A005D6762477ACBF9130587`). `Ulid.New(Guid)` reverses it.
- `.ToString(string? format = null, IFormatProvider? formatProvider = null)`\
  Converts the ULID to a canonical string representation. Format arguments are ignored.
- Provides implicit operators to and from `Guid` and `string`.

### Comparison

- Supports all comparison operators:\
  `==`, `!=`, `<`, `<=`, `>`, `>=`.
- Implements standard comparison and equality methods:\
  `CompareTo`, `Equals`, `GetHashCode`.
- Implements these .NET interfaces on every target framework:\
  `IEquatable<Ulid>`, `IEqualityComparer<Ulid>`, `IComparable`, `IComparable<Ulid>`, `IFormattable`.
- Additionally on .NET 6.0+: `ISpanFormattable`.
- Additionally on .NET 7.0+: `IMinMaxValue<Ulid>`, `IEqualityOperators<Ulid, Ulid, bool>`, `IComparisonOperators<Ulid, Ulid, bool>`, `IParsable<Ulid>`, `ISpanParsable<Ulid>`.
- Additionally on .NET 8.0+: `IUtf8SpanFormattable`, `IUtf8SpanParsable<Ulid>`.

### GenerationOptions

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

## 🔌 Integration with other libraries

### ASP.NET Core

The optional `ByteAether.Ulid.AspNetCore` companion library adds ULID route constraints, OpenAPI schema metadata, and typed link generation. ASP.NET Core usage works without it, because the core `Ulid` type already supports model binding and JSON serialization.

### System.Text.Json

Includes a `JsonConverter` for serialization and deserialization.

### Other libraries

The companion packages for [Entity Framework Core](https://www.nuget.org/packages/ByteAether.Ulid.EntityFrameworkCore/), [LinqToDB](https://www.nuget.org/packages/ByteAether.Ulid.linq2db/), [Dapper](https://www.nuget.org/packages/ByteAether.Ulid.Dapper/), and the [command-line tool](https://www.nuget.org/packages/ByteAether.Ulid.Cli/) have their own documentation. Recipes for Newtonsoft.Json and MessagePack are in [serialization](https://github.com/ByteAether/Ulid/blob/main/docs/serialization.md).

## 🏛️ Acknowledgements

This implementation is based on or inspired by the following projects and specifications:

- [NetUlid](https://github.com/ultimicro/netulid)
- [Ulid](https://github.com/Cysharp/Ulid)
- [NUlid](https://github.com/RobThree/NUlid)
- [Official ULID specification](https://github.com/ulid/spec)
- [Crockford's Base32](https://www.crockford.com/base32.html)

## 📜 License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/ByteAether/Ulid/blob/main/LICENSE) file for details.
