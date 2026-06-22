# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

OpenShock Common .NET library — shared utilities for the OpenShock ecosystem. Contains two main libraries: **Common** (utilities, validation, crypto, geolocation) and **DynamicLinq** (dynamic LINQ query builder for PostgreSQL/EF Core).

## Build Commands

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run a single test project
dotnet test Common.Tests/Common.Tests.csproj
dotnet test DynamicLinq.Tests/DynamicLinq.Tests.csproj

# CI-style build + test
dotnet build --configuration Release && dotnet test --configuration Release --no-build
```

No separate lint command — warnings are treated as errors in Debug configuration.

## Solution Structure

- **Common/** — Main utility library (NuGet package output)
- **Common.Tests/** — Unit tests for Common
- **DynamicLinq/** — Dynamic LINQ expression builder targeting PostgreSQL via EF Core
- **DynamicLinq.Tests/** — Tests for DynamicLinq (uses Testcontainers with PostgreSQL)

Solution file: `Common.slnx`

## Build Configuration

- .NET 10.0, latest C# language version
- Nullable reference types enabled globally
- Central package management via `Directory.Packages.props`
- `Directory.Build.props` enables implicit usings, XML docs, and treats warnings as errors in Debug
- GitVersion used in CI for semantic versioning (requires full git history)

## Testing

Uses **TUnit** (not xUnit/NUnit). Test methods use TUnit attributes and async patterns. DynamicLinq tests use **Testcontainers** for real PostgreSQL instances and **Bogus** for test data generation.

## Architecture & Key Patterns

### Common Library Namespaces
- `Constants` — Domain hard limits (username/password lengths, shocker limits, durations, intensities)
- `Utils` — `HashingUtils` (BCrypt + legacy PBKDF2, SHA-256 token hashing), `CryptoUtils` (cryptographic RNG), `MathUtils` (Haversine), `LatencyEmulator` (timing-safe operations with Gaussian noise)
- `Geo` — `Alpha2CountryCode` (value-type struct wrapping ushort), `DistanceLookup` (pre-computed frozen dictionary of country-to-country distances)
- `Validation` — `CharsetMatchers` for detecting emojis, zalgo text, zero-width spaces, control chars
- `JsonSerialization` — Custom converters (Unix milliseconds, flag-guarded enum strings)

### DynamicLinq Library
- `QueryStringTokenizer` — Parses filter strings with quoted values and escape sequences
- `DBExpressionBuilder` — Converts string filter queries into LINQ expressions (supports `eq`, `neq`, `lt`, `gt`, `lte`, `gte`, `ilike`; multiple filters joined with `and`)
- `OrderByQueryBuilder` — Dynamic OrderBy/ThenBy from string input
- Properties marked with `[IgnoreDataMember]` are excluded from query building for security

### Performance Conventions
- `stackalloc` / `Span<T>` for short-lived buffers
- `ArrayPool<byte>` for temporary allocations
- `FrozenDictionary` for immutable lookup tables
- `CollectionsMarshal` for optimized dictionary operations
- `GeneratedRegex` for compiled regex patterns

### Security Conventions
- Password hashing uses BCrypt with SHA-512; PBKDF2 is legacy-only
- Token hashing uses SHA-256
- Timing-safe verification via `LatencyEmulator` sliding window statistics
- Expression builder hides internal structure in error messages

## License

AGPL-3.0-or-later
