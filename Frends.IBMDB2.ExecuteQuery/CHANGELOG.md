# Changelog

## [2.0.0] - 2026-10-22
### Changed
- [Breaking] Result.ErrorMessage has been replaced with Result.Error object containing Message and AdditionalInfo.
- Upgraded the Task to target .NET 8.
- Error message is now customizable with the new Options.ErrorMessageOnFailure setting.
- Options.ThrowErrorOnFailure now defaults to true.
- When an error is thrown, the original exception is now rethrown unless a custom error message is set.

## [1.1.0] - 2025-02-17
### Changed
- Update package Microsoft.CodeAnalysis.CSharp from 4.7.0 to 4.13.0
- Update package Newtonsoft.Json from 12.0.3 to 13.0.3

## [1.0.0] - 2023-10-24
### Added
- Initial implementation
