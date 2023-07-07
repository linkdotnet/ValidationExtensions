using System;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests.DateAttributes;

public class FutureDateValidationAttributeTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsValid_WithPastDate_ReturnsValidationError(bool useUtc)
    {
        // Arrange
        var validationAttribute = new FutureDateValidationAttribute { UseUtc = useUtc };
        var pastDate = (useUtc ? DateTime.UtcNow : DateTime.Now).AddMinutes(-1);

        // Act
        var validationResult = validationAttribute.IsValid(pastDate);

        // Assert
        validationResult.Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsValid_WithFutureDate_ReturnsSuccess(bool useUtc)
    {
        // Arrange
        var validationAttribute = new FutureDateValidationAttribute { UseUtc = useUtc };
        var futureDate = (useUtc ? DateTime.UtcNow : DateTime.Now).AddMinutes(1);

        // Act
        var validationResult = validationAttribute.IsValid(futureDate);

        // Assert
        validationResult.Should().BeTrue();
    }
}