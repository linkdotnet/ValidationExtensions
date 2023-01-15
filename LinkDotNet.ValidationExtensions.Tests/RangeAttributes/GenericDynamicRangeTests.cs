using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests.Generic;

public partial class GenericDynamicRangeTests
{
    [Fact(DisplayName = "Should not be valid if 'MinimumWeight' is greater than 'MaximumWeight'")]
    public static void ShouldNotBeValidIfMinimumIsGreaterThanMaximum()
    {
        var data = new Model(2, 1);
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(data, context, results, true);

        isValid.Should().BeFalse();
        results.Should().HaveCount(2);
        results[0].ErrorMessage.Should().Be("The field MinimumWeight must be between 0.1 and 1.");
        results[1].ErrorMessage.Should().Be("The field MaximumWeight must be between 2 and 5.2.");
    }

    [Fact(DisplayName = "Should be valid if 'MaximumWeight' is greater than 'MinimumWeight'")]
    public static void ShouldBeValidIfMaximumIsGreaterThanMinimum()
    {
        var data = new Model(1, 2);
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(data, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'Minimum Property Value' is null")]
    public static void MustThrowsInvalidOperationExceptionWhenMinimumPropertyValueIsNull()
    {
        var data = new Model(null, 2);
        var context = new ValidationContext(data);
        var attribute = new DynamicRangeAttribute<double>("MinimumWeight", 5.2);

        var action = () => attribute.Validate("Any", context);

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The value of 'MinimumWeight' property cannot be null (introduced for 'Minimum' in range).");
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'Maximum Property Value' is null")]
    public static void MustThrowsInvalidOperationExceptionWhenMaximumPropertyValueIsNull()
    {
        var data = new Model(2, null);
        var context = new ValidationContext(data);
        var attribute = new DynamicRangeAttribute<double>(0.1, "MaximumWeight");

        var action = () => attribute.Validate("Any", context);

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The value of 'MaximumWeight' property cannot be null (introduced for 'Maximum' in range).");
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'Minimum PropertyName' is null or empty")]
    public static void MustThrowsInvalidOperationExceptionWhenMinimumPropertyNameIsNullOrEmpty()
    {
        var attribute = new DynamicRangeAttribute<double>(string.Empty, 5.2);

        var action = () => attribute.Validate("Any", new ValidationContext(new object()));

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The 'Minimum PropertyName' cannot be null or empty.");
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'Maximum PropertyName' is null or empty")]
    public static void MustThrowsInvalidOperationExceptionWhenMaximumPropertyNameIsNullOrEmpty()
    {
        var attribute = new DynamicRangeAttribute<double>(0.1, string.Empty);

        var action = () => attribute.Validate("Any", new ValidationContext(new object()));

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The 'Maximum PropertyName' cannot be null or empty.");
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'Minimum' PropertyName is wrong")]
    public static void MustThrowsInvalidOperationExceptionWhenMinimumPropertyNameIsWrong()
    {
        var data = new Model(1, 2);
        var context = new ValidationContext(data);
        var attribute = new DynamicRangeAttribute<double>("WRONG", 5.2);

        var action = () => attribute.Validate("Any", context);

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The 'WRONG' property not found (introduced for 'Minimum' in range).");
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'Maximum' PropertyName is wrong")]
    public static void MustThrowsInvalidOperationExceptionWhenMaximumPropertyNameIsWrong()
    {
        var data = new Model(1, 2);
        var context = new ValidationContext(data);
        var attribute = new DynamicRangeAttribute<double>(0.1, "WRONG");

        var action = () => attribute.Validate("Any", context);

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The 'WRONG' property not found (introduced for 'Maximum' in range).");
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'Minimum' PropertyType and OperandType not the same")]
    public static void MustThrowsInvalidOperationExceptionWhenMinimumPropertyTypeAndOperandTypeNotTheSame()
    {
        var data = new ModelWrongMinimumPropertyType(1, 5);
        var context = new ValidationContext(data);

        var action = () => Validator.ValidateObject(data, context, true);

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The 'MinimumWeight' type must be the same as the OperandType (introduced for 'Minimum' in range).");
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'Maximum' PropertyType and OperandType not the same")]
    public static void MustThrowsInvalidOperationExceptionWhenMaximumPropertyTypeAndOperandTypeNotTheSame()
    {
        var data = new ModelWrongMaximumPropertyType(1, 5);
        var context = new ValidationContext(data);

        var action = () => Validator.ValidateObject(data, context, true);

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The 'MaximumWeight' type must be the same as the OperandType (introduced for 'Maximum' in range).");
    }

    public class Model
    {
        public Model(double? minimumWeight, double? maximumWeight)
        {
            MinimumWeight = minimumWeight;
            MaximumWeight = maximumWeight;
        }

        [DynamicRange<double>(minimum: 0.1, maximumPropertyName: nameof(MaximumWeight))]
        public double? MinimumWeight { get; set; }

        [DynamicRange<double>(minimumPropertyName: nameof(MinimumWeight), maximum: 5.2)]
        public double? MaximumWeight { get; set; }
    }

    public class ModelWrongMaximumPropertyType
    {
        public ModelWrongMaximumPropertyType(double? minimumWeight, float? maximumWeight)
        {
            MinimumWeight = minimumWeight;
            MaximumWeight = maximumWeight;
        }

        [DynamicRange<double>(minimum: 0.1, maximumPropertyName: nameof(MaximumWeight))]
        public double? MinimumWeight { get; set; }

        public float? MaximumWeight { get; set; }
    }

    public class ModelWrongMinimumPropertyType
    {
        public ModelWrongMinimumPropertyType(float? minimumWeight, double? maximumWeight)
        {
            MinimumWeight = minimumWeight;
            MaximumWeight = maximumWeight;
        }

        public float? MinimumWeight { get; set; }

        [DynamicRange<double>(minimumPropertyName: nameof(MinimumWeight), maximum: 5.2)]
        public double? MaximumWeight { get; set; }
    }
}