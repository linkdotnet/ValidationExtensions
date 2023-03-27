using System;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests.DateAttributes;

public class PastDateValidationAttributeTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsValid_WithPastDate_ReturnsSuccess(bool useUtc)
    {
        // Arrange
        var validationAttribute = new PastDateValidationAttribute { UseUtc = useUtc };
        var pastDate = (useUtc ? DateTime.UtcNow : DateTime.Now).AddMinutes(-1);

        // Act
        var validationResult = validationAttribute.IsValid(pastDate);

        // Assert
        validationResult.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsValid_WithFutureDate_ReturnsValidationError(bool useUtc)
    {
        // Arrange
        var validationAttribute = new PastDateValidationAttribute { UseUtc = useUtc };
        var futureDate = (useUtc ? DateTime.UtcNow : DateTime.Now).AddMinutes(1);

        // Act
        var validationResult = validationAttribute.IsValid(futureDate);

        // Assert
        validationResult.Should().BeFalse();
    }
}