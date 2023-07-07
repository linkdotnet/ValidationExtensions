using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests;

public class RequiredIfTests
{
    [Fact]
    public void ShouldBeValidIfIsDependentIsSameValue()
    {
        var model = new Model(null, false, "test");
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotBeValidWhenIsSameValueIsDifferent()
    {
        var model = new Model(null, true, "test");
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeFalse();
        results.Should().HaveCount(1);
        results.Single().ErrorMessage.Should().Be("Property 'SomeProperty' is required when 'IsDependent' is 'True'");
        results.Single().MemberNames.Should().HaveCount(1).And.Contain("SomeProperty");
    }

    [Fact]
    public void ShouldCheckForNullValues()
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

    [Fact]
    public void ShouldBeValidWhenNullAndDependentIsNotNull()
    {
        var model = new OtherModel(null, "Test");
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldBeInvalidWhenNullAndDependentIsNull()
    {
        var model = new OtherModel(null, null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ShouldBeValidWhenNotNullAndDependentIsNotNull()
    {
        var model = new OtherModel("Test", "Test");
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldBeValidWhenNotNullAndDependentIsNull()
    {
        var model = new OtherModel("Test", null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeTrue();
    }

    public class Model
    {
        public Model(string? someProperty, bool isDependent, string? otherPropertyDependingOnString)
        {
            SomeProperty = someProperty;
            IsDependent = isDependent;
            OtherPropertyDependingOnString = otherPropertyDependingOnString;
        }

        [RequiredIf(nameof(IsDependent), true)]
        public string? SomeProperty { get; set; }

        [Required]
        public bool IsDependent { get; set; }

        [RequiredIf(nameof(SomeProperty), "Test")]
        public string? OtherPropertyDependingOnString { get; set; }
    }

    public class OtherModel
    {
        public OtherModel(string? someOtherProperty, string? isDependentNullable)
        {
            SomeOtherProperty = someOtherProperty;
            IsDependentNullable = isDependentNullable;
        }

        public string? IsDependentNullable { get; set; }

        [RequiredIf(nameof(IsDependentNullable), null)]
        public string? SomeOtherProperty { get; set; }

    }

    public class InvalidModel
    {
        [RequiredIf("not-existing property", false)]
        public string? Id { get; set; }
    }
}