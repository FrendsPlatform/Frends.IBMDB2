# Changelog

## [2.0.0] - 2026-10-25
- **[Breaking Change]** Replaced `Result.ErrorMessage` with `Result.Error` object containing `Message` and `AdditionalInfo`.
- **[Breaking Change]** Error messages are no longer prefixed with transaction rollback details; use `Options.ErrorMessageOnFailure` to override the message.
- Added `ErrorMessageOnFailure` option for customizing the error message.
- Task now targets .NET 8.
- Added missing example values to the `QueryParameter` documentation.

## [1.1.0] - 2025-02-17
### Changed
- Update package Microsoft.CodeAnalysis.CSharp from 4.7.0 to 4.13.0
- Update package Newtonsoft.Json from 12.0.3 to 13.0.3

## [1.0.0] - 2023-10-24
### Added
- Initial implementation
