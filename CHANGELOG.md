# Changelog

All notable changes to the **ValidationExtensions** will be documented in this file. The project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
 * Added `RequiredDynamicAttribute` which can accept method to validate complicated requirement(s) and more readable code.

## [Unreleased]

### Added
 * Removed all `*IfNotAttribute` in favor of newly introducded `IsInverse` parameter. For example: `[RequiredIfNot(nameof(OtherProperty), false)]` --> `[RequiredIf(nameof(OtherProperty), false, isInverse: true)]`
 * Added `MinAttribute` which indicates that a number field has to have at least this value
 * Added `MinIfAttribute` which used `MinAttribute` if the condition is (not) met
 * Added `MaxAttribute` which indicates that a number field has to have at most this value
 * Added `MaxIfAttribute` which used `MaxAttribute` if the condition is (not) met

## [1.1.0]

### Added
 * Added `MinLengthRequiredIfAttribute` and `MinLengthRequiredIfNotAttribute`
 * Added `MaxLengthRequiredIfAttribute` and `MaxLengthRequiredIfNotAttribute`

## [1.0.0]
 * Initial Release
