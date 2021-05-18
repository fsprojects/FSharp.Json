# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.5.0-rc.2]
### Added
- Support for Set and ResizeArray

### Changed
- Modernize tests
- Dependency FSharp.Core 4.3.4 (F# 4.1 - https://github.com/dotnet/fsharp/blob/main/docs/fsharp-core-notes.md - https://en.wikipedia.org/wiki/F_Sharp_(programming_language))

### Fixed
- Null in untyped deserializing

## [0.4.0]
- Fixed no fields union case (de)serialization - not backward compatible.

## [0.3.7]
- Added support for numeric types: byte, sbyte, int16, uint16, uint, uint64, bigint
- Added support for floating point single type

## [0.3.6]
- Documentation cleanup

## [0.3.5]
- Moved to Release build

## [0.3.4]
- Moved to .NET Standard

## [0.3.3]
- Added .NET Core support

## [0.3.2]
- Added Transform for Uri type

## [0.3.1]
- Fixed FSharp.Core dependency to allow newer versions

## [0.3]
- Fix for tuples containing option types
- Support for char type
- Support for enums based on byte and char types
- Configurable enum mode
- Configurable unformatted setting

## [0.2] 
- Single case union as wrapped type 

## [0.1]
- Initial release
