# Changelog

All notable changes to the **ValidationExtensions** will be documented in this file. The project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

- Fixed issue with not indidating validation errors with a border around the control.

## [2.4.0] - 2023-01-16

-   Added `DynamicRangeAttribute<T>` which is the simplified and type-strong version of `DynamicRangeAttribute`. By [mhb164](https://github.com/mhb164)

## [2.3.2] - 2023-01-09

## [2.3.1] - 2023-01-09

### Added

-   Support for `netstandard2.0` and `net7.0`

### Added

## [2.1.0]

### Added

-   Added `RequiredDynamicAttribute` which can accept method to validate complicated requirement(s) and more readable code. By [mhb164](https://github.com/mhb164)

## [2.0.0]

### Added

-   Removed all `*IfNotAttribute` in favor of newly introducded `IsInverse` parameter. For example: `[RequiredIfNot(nameof(OtherProperty), false)]` --> `[RequiredIf(nameof(OtherProperty), false, isInverse: true)]`
-   Added `MinAttribute` which indicates that a number field has to have at least this value
-   Added `MinIfAttribute` which used `MinAttribute` if the condition is (not) met
-   Added `MaxAttribute` which indicates that a number field has to have at most this value
-   Added `MaxIfAttribute` which used `MaxAttribute` if the condition is (not) met

## [1.1.0]

### Added

-   Added `MinLengthRequiredIfAttribute` and `MinLengthRequiredIfNotAttribute`
-   Added `MaxLengthRequiredIfAttribute` and `MaxLengthRequiredIfNotAttribute`

## [1.0.0]

-   Initial Release

[Unreleased]: https://github.com/linkdotnet/ValidationExtensions/compare/2.4.0...HEAD

[2.4.0]: https://github.com/linkdotnet/ValidationExtensions/compare/2.3.2...2.4.0

[2.3.2]: https://github.com/linkdotnet/ValidationExtensions/compare/2.3.1...2.3.2

[2.3.1]: https://github.com/linkdotnet/ValidationExtensions/compare/138e2951b2d42584ed66e41e6f31e203e509b8ef...2.3.1
