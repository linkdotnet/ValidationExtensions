using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests;

public class RequiredIfNotTests
{
    [Fact]
    public void ShouldBeValidIfIsDependentIsNotSameValue()
    {
        var model = new Model(null, false, "Test");
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldBeInvalidWhenRequiredNotConditionNotMet()
    {
        var model = new Model(null, true, "test");
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeFalse();
        results.Should().HaveCount(1);
        results.Single().ErrorMessage.Should().Be("Property 'SomeProperty' is required when 'IsDependent' is not 'False'");
    }

    [Fact]
    public void ShouldCheckForNullValues()
    {
        var model = new Model("Test", false, null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldCheckNullValues()
    {
        var model = new Model(null, false, null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldThrowExceptionWhenPropertyNotFound()
    {
        var model = new InvalidModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var act = () => Validator.TryValidateObject(model, context, results, true);

        act.Should().Throw<NotSupportedException>();
    }

    public class Model
    {
        public Model(string? someProperty, bool isDependent, string? otherPropertyDependingOnString)
        {
            SomeProperty = someProperty;
            IsDependent = isDependent;
            OtherPropertyDependingOnString = otherPropertyDependingOnString;
        }

        [RequiredIf(nameof(IsDependent), false, true)]
        public string? SomeProperty { get; set; }

        [Required]
        public bool IsDependent { get; set; }

        [RequiredIf(nameof(SomeProperty), "Test", true)]
        public string? OtherPropertyDependingOnString { get; set; }
    }

    public class InvalidModel
    {
        [RequiredIf("not-existing property", false)]
        public string? Id { get; set; }
    }
}