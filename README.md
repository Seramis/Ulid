# ULID
*from **ByteAether***

[![NuGet Version](https://img.shields.io/nuget/v/ByteAether.Ulid)](https://www.nuget.org/packages/ByteAether.Ulid/)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ByteAether/Ulid/build-and-test.yml)](https://github.com/ByteAether/Ulid/actions/workflows/build-and-test.yml)

A .NET implementation of ULIDs (Universally Unique Lexicographically Sortable Identifiers) that is fully compatible with the [official ULID specification](https://github.com/ulid/spec).

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [API](#api)
- [Contributing](#contributing)
- [License](#license)

## Introduction

ULIDs are a type of identifier that are designed to be universally unique and lexicographically sortable. They are useful for generating unique identifiers that can be easily sorted and compared. This repository contains a .NET implementation of ULIDs that is fully compatible with the official ULID specification. Unlike many other implementations that deviate from the specification, sometimes on crucial points, this implementation adheres strictly to the official guidelines.

Additionally, this implementation addresses a potential issue in the official specification where generating multiple ULIDs within the same millisecond can cause the "random" part of the ULID to overflow, leading to an overflow exception being thrown. To ensure dependability and guarantee the generation of unique ULIDs, this implementation allows overflow to increment the "timestamp" part of the ULID, thereby eliminating the possibility of randomly occuring exception.

Relevant issue with same suggestion is opened on official ULID specification: [Guarantee a minimum number of IDs before overflow of the random component #39](https://github.com/ulid/spec/issues/39#issuecomment-2252145597)

For almost all systems in the world, both GUID and integer IDs should be abandoned in favor of ULIDs. GUIDs, while unique, lack sortability and readability, making them less efficient for indexing and querying. Integer IDs, on the other hand, are sortable but not universally unique, leading to potential conflicts in distributed systems. ULIDs combine the best of both worlds, offering both uniqueness and sortability, making them an ideal choice for modern applications that require scalable and efficient identifier generation. This library provides a robust and reliable implementation of ULIDs, ensuring that your application can benefit from these advantages without compromising on performance or compliance with the official specification.

## Features

- **Universally Unique**: ULIDs are designed to be globally unique.
- **Lexicographically Sortable**: ULIDs can be sorted lexicographically, making them useful for time-based sorting.
- **Efficient**: The implementation is designed to be efficient and performant.
- **Compatible**: The implementation provides methods to convert ULIDs to and from GUIDs, Crockford's Base32 strings (canonical form), and byte arrays.
- **Fully Compliant**: This implementation adheres strictly to the official ULID specification.
- **Enhanced Reliability**: Eliminates the possibility of throwing an exception when the "random" part gets overflown.

## Benchmarking
To ensure the performance and efficiency of this ULID implementation, benchmarking was conducted using BenchmarkDotNet.

For comparison, [NetUlid](https://github.com/ultimicro/netulid) 2.1.0, [Ulid](https://github.com/Cysharp/Ulid) 1.3.4 and [NUlid](https://github.com/RobThree/NUlid) 1.7.2 implementations were benchmarked alongside.

The following benchmarks were performed:
```
BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5131/22H2/2022Update)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=DefaultJob

| Type            | Method         | Mean        | Error     | Gen0   | Allocated |
|---------------- |--------------- |------------:|----------:|-------:|----------:|
| Generate        | ByteAetherUlid |  54.4466 ns | 0.2331 ns |      - |         - |
| Generate        | NetUlid *(1)   | 160.6932 ns | 1.7455 ns | 0.0095 |      80 B |
| Generate        | NUlid *(2)     |  74.2981 ns | 0.6811 ns | 0.0124 |     104 B |

| GenerateNonMono | ByteAetherUlid |  97.3747 ns | 0.3449 ns |      - |         - |
| GenerateNonMono | Ulid *(3,4)    |  44.0165 ns | 0.1488 ns |      - |         - |
| GenerateNonMono | NUlid          | 114.2276 ns | 0.5517 ns | 0.0124 |     104 B |
| GenerateNonMono | Guid           |  47.3825 ns | 0.1112 ns |      - |         - |

| FromByteArray   | ByteAetherUlid |   0.0000 ns | 0.0000 ns |      - |         - |
| FromByteArray   | NetUlid        |   0.6843 ns | 0.0024 ns |      - |         - |
| FromByteArray   | Ulid           |   6.9964 ns | 0.0190 ns |      - |         - |
| FromByteArray   | NUlid          |  10.7442 ns | 0.0240 ns |      - |         - |
| FromByteArray   | Guid           |   0.2575 ns | 0.0049 ns |      - |         - |

| FromGuid        | ByteAetherUlid |   1.6311 ns | 0.0034 ns |      - |         - |
| FromGuid        | NetUlid        |   5.2728 ns | 0.0759 ns | 0.0048 |      40 B |
| FromGuid        | Ulid           |   1.6940 ns | 0.0023 ns |      - |         - |
| FromGuid        | NUlid          |  14.3225 ns | 0.0379 ns | 0.0048 |      40 B |

| FromString      | ByteAetherUlid |  15.5433 ns | 0.0376 ns |      - |         - |
| FromString      | NetUlid        |  27.1990 ns | 0.0730 ns |      - |         - |
| FromString      | Ulid           |  15.0990 ns | 0.0869 ns |      - |         - |
| FromString      | NUlid          |  81.3847 ns | 1.0069 ns | 0.0324 |     272 B |
| FromString      | Guid           |  22.7008 ns | 0.0807 ns |      - |         - |

| ToByteArray     | ByteAetherUlid |   3.9814 ns | 0.0872 ns | 0.0048 |      40 B |
| ToByteArray     | NetUlid        |   9.9516 ns | 0.1120 ns | 0.0048 |      40 B |
| ToByteArray     | Ulid           |   3.7345 ns | 0.0778 ns | 0.0048 |      40 B |
| ToByteArray     | NUlid          |   7.4857 ns | 0.1334 ns | 0.0048 |      40 B |

| ToGuid          | ByteAetherUlid |   0.7166 ns | 0.0009 ns |      - |         - |
| ToGuid          | NetUlid        |  12.3059 ns | 0.1163 ns | 0.0048 |      40 B |
| ToGuid          | Ulid           |   0.7384 ns | 0.0079 ns |      - |         - |
| ToGuid          | NUlid          |  12.2188 ns | 0.1052 ns | 0.0048 |      40 B |

| ToString        | ByteAetherUlid |  21.0633 ns | 0.3459 ns | 0.0095 |      80 B |
| ToString        | NetUlid        |  22.5619 ns | 0.3377 ns | 0.0095 |      80 B |
| ToString        | Ulid           |  19.7406 ns | 0.2671 ns | 0.0095 |      80 B |
| ToString        | NUlid          |  53.6629 ns | 0.1617 ns | 0.0430 |     360 B |
| ToString        | Guid           |  12.0498 ns | 0.0899 ns | 0.0115 |      96 B |

| CompareTo       | ByteAetherUlid |   0.4954 ns | 0.0048 ns |      - |         - |
| CompareTo       | NetUlid        |   3.9930 ns | 0.0270 ns |      - |         - |
| CompareTo       | Ulid           |   2.0029 ns | 0.0092 ns |      - |         - |
| CompareTo       | NUlid          |   9.1245 ns | 0.1161 ns | 0.0048 |      40 B |

| Equals          | ByteAetherUlid |   0.4534 ns | 0.0025 ns |      - |         - |
| Equals          | NetUlid        |   1.1755 ns | 0.0154 ns |      - |         - |
| Equals          | Ulid           |   0.0163 ns | 0.0024 ns |      - |         - |
| Equals          | NUlid          |  17.6854 ns | 0.3190 ns | 0.0095 |      80 B |
| Equals          | Guid           |   0.0000 ns | 0.0000 ns |      - |         - |

| GetHashCode     | ByteAetherUlid |   0.0000 ns | 0.0000 ns |      - |         - |
| GetHashCode     | NetUlid        |   9.7133 ns | 0.0129 ns |      - |         - |
| GetHashCode     | Ulid           |   0.0000 ns | 0.0000 ns |      - |         - |
| GetHashCode     | NUlid          |  13.5635 ns | 0.1188 ns | 0.0048 |      40 B |
| GetHashCode     | Guid           |   0.0138 ns | 0.0036 ns |      - |         - |
```
All competitive libraries deviate from the official ULID specification in various ways or have other drawbacks:
  1. `NetUlid`: Can only maintain monotonicity in the scope of a single thread.
  2. `NUlid`: Requires special configuration to enable monotonic generation. You have to write your own wrapper with state.
  3. `Ulid`: Does not implement monotonicity.
  4. `Ulid`: This library uses a cryptographically non-secure `XOR-Shift` random value generation. Only the initial seed is generated by a cryptographically secure generator.

Both `NetUlid` and `NUlid`, which do provide monotonicity, may randomly throw `OverflowException`, when stars align against you. (Random-part overflow)

As such, it can be concluded that this implementation is either the fastest or very close to the fastest ones, while also adhering most completely to the official ULID specification and can be relied on.

## Installation

You can install the package via NuGet:

```sh
dotnet add package ByteAether.Ulid
```

## Usage

Here is a basic example of how to use the ULID implementation:

```csharp
using System;
using YourNamespace; // Replace with the actual namespace

class Program
{
    static void Main()
    {
        // Create a new ULID
        var ulid = Ulid.New();

        // Convert to byte array & back
        byte[] byteArray = ulid.ToByteArray();
        Console.WriteLine($"Byte Array: {BitConverter.ToString(byteArray)}");
        var ulidFromByteArray = Ulid.New(byteArray);

        // Convert to GUID & back
        Guid guid = ulid.ToGuid();
        Console.WriteLine($"GUID: {guid}");
        var ulidFromGuid = Ulid.New(guid);

        // Convert to string & back
        string str = ulid.ToString();
        Console.WriteLine($"String: {str}");
        var ulidFromStr = Ulid.Parse(str);
    }
}
```

## API

The `Ulid` implementation provides the following methods:

- `Ulid.New(bool isMonotonic = true)`: Generates new ULID
- `Ulid.New(ReadOnlySpan<byte> bytes)`: Create from existing array of bytes.
- `Ulid.New(Guid guid)`: Create from existing `Guid`.
- `Ulid.New(DateTimeOffset dateTimeOffset, bool isMonotonic = true)`: Generates new ULID using given `DateTimeOffset`
- `Ulid.New(long timestamp, bool isMonotonic = true)`: Generates new ULID using given `long` unix timestamp.
- `Ulid.Parse(ReadOnlySpan<char> chars, IFormatProvider? provider = null)`: Creates from existing array of `char`s of canonical form. `IFormatProvider` is irrelevant.
- `Ulid.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Ulid result)`: Same as previous using `Try*()` pattern.
- `Ulid.Parse(string s, IFormatProvider? provider = null)`: Creates from existing `string` of canonical form. `IFormatProvider` is irrelevant.
- `Ulid.TryParse(string? s, IFormatProvider? provider, out Ulid result)`: Same as previous using `Try*()` pattern.
- `.ToByteArray()`: Converts the ULID to a byte array.
- `.ToGuid()`: Converts the ULID to a GUID.
- `.ToString(string? format = null, IFormatProvider? formatProvider = null)`: Converts the ULID to a canonical string representation. (Formatting arguments are irrelevant)
- `.Copy()`: Allocates another ULID with identical value
- All comparison operators: `GetHashCode`, `Equals`, `CompareTo`, `==`, `!=`, `<`, `<=`, `>`, `>=`.
- Explicit operators to and from `Guid`.

## Integration with other libraries

### ASP.NET Core
The `Ulid` structure has `TypeConverter` applied so ULIDs can be used as an argument on the action (e.g. query string, route parameter, etc.) without any additional works. The input will be accepted as canonical form.

### System.Text.Json
The `Ulid` structure has `JsonConverterAttribute` applied so that it can be used with `System.Text.Json` without any additional works.

## Prior Art

Much of this implementation is either based on or inspired by existing implementations. This library is standing on the shoulders of giants.

  * [NetUlid](https://github.com/ultimicro/netulid)
  * [Ulid](https://github.com/Cysharp/Ulid)
  * [NUlid](https://github.com/RobThree/NUlid)
  * [Official ULID specification](https://github.com/ulid/spec)

## Contributing

Contributions are welcome! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes and commit them with descriptive commit messages.
4. Push your changes to your fork.
5. Open a pull request against the `main` branch of this repository.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.