# Changelog

All notable changes to the **ValidationExtensions** will be documented in this file. The project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
 * Removed all `*IfNotAttribute` in favor of newly introducded `IsInverse` parameter. For example: `[RequiredIfNot(nameof(OtherProperty), false)]` --> `[RequiredIf(nameof(OtherProperty), false, isInverse: true)]`

## [1.1.0]

### Added
 * Added `MinLengthRequiredIfAttribute` and `MinLengthRequiredIfNotAttribute`
 * Added `MaxLengthRequiredIfAttribute` and `MaxLengthRequiredIfNotAttribute`

## [1.0.0]
 * Initial Release