# Changelog

All notable changes to the **ValidationExtensions** will be documented in this file. The project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- net9.0 as target framework.

## [2.6.0] - 2023-07-07

### Fixed

-   Fixed issue with not indicating validation errors with a border around the control. By [Robelind](https://github.com/Robelind)
-   Fixed an issue where `RequiredIf` did not work with `null` values. By [Robelind](https://github.com/Robelind)

## [2.5.1] - 2023-04-16

### Added

-   Respect optional Error Message

### Fixed

-   Use wrong error sentence

## [2.5.0] - 2023-03-27

### Added

-   New attributes to check if a date is valid in the future or past.

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

[Unreleased]: https://github.com/linkdotnet/ValidationExtensions/compare/2.6.0...HEAD

[2.6.0]: https://github.com/linkdotnet/ValidationExtensions/compare/2.5.1...2.6.0

[2.5.1]: https://github.com/linkdotnet/ValidationExtensions/compare/2.5.0...2.5.1

[2.5.0]: https://github.com/linkdotnet/ValidationExtensions/compare/2.4.0...2.5.0

[2.4.0]: https://github.com/linkdotnet/ValidationExtensions/compare/2.3.2...2.4.0

[2.3.2]: https://github.com/linkdotnet/ValidationExtensions/compare/2.3.1...2.3.2

[2.3.1]: https://github.com/linkdotnet/ValidationExtensions/compare/138e2951b2d42584ed66e41e6f31e203e509b8ef...2.3.1
