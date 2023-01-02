using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests;

public class DynamicRangeTests
{
    [Fact(DisplayName = "Should not be valid if 'MinimumWeight' is grather than 'MaximumWeight'")]
    public static void ShouldNotBeValidIfMinimumIsGratherThanMaximum()
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

    [Fact(DisplayName = "Should be valid if 'MaximumWeight' is grather than 'MinimumWeight'")]
    public static void ShouldBeValidIfMaximumIsGratherThanMinimum()
    {
        var data = new Model(1, 2);
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(data, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'MinimumWeight' is null")]
    public static void MustThrowsInvalidOperationExceptionWhenMinimumWeightIsNull()
    {
        var data = new Model(null, 2);
        var context = new ValidationContext(data);
        var attribute = new DynamicRangeAttribute(typeof(decimal), "MinimumWeight", "5.2");

        var action = () => attribute.Validate("Any", context);

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The value of 'MinimumWeight' property cannot be null (introduced for 'Minimum' in range).");
    }

    [Fact(DisplayName = "Must throws 'InvalidOperationException' when 'MaximumWeight' is null")]
    public static void MustThrowsInvalidOperationExceptionWhenMaximumWeightIsNull()
    {
        var data = new Model(2, null);
        var context = new ValidationContext(data);
        var attribute = new DynamicRangeAttribute(typeof(decimal), "0.1", "MaximumWeight");

        var action = () => attribute.Validate("Any", context);

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("The value of 'MaximumWeight' property cannot be null (introduced for 'Maximum' in range).");
    }

    [Fact(DisplayName = "Must throws 'ArgumentNullException' when 'Minimum' is empty")]
    public static void MustThrowsArgumentNullExceptionWhenMinimumIsEmpty()
    {
        var attribute = new DynamicRangeAttribute(typeof(decimal), string.Empty, "5.2");

        var action = () => attribute.Validate("Any", new ValidationContext(new object()));

        action.Should()
              .Throw<ArgumentNullException>()
              .WithMessage("Value cannot be null. (Parameter 'Minimum')");
    }

    [Fact(DisplayName = "Must throws 'ArgumentNullException' when 'Maximum' is empty")]
    public static void MustThrowsArgumentNullExceptionWhenMaximumIsEmpty()
    {
        var attribute = new DynamicRangeAttribute(typeof(decimal), "0.1", string.Empty);

        var action = () => attribute.Validate("Any", new ValidationContext(new object()));

        action.Should()
              .Throw<ArgumentNullException>()
              .WithMessage("Value cannot be null. (Parameter 'Maximum')");
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
        public Model(decimal? minimumWeight, decimal? maximumWeight)
        {
            MinimumWeight = minimumWeight;
            MaximumWeight = maximumWeight;
        }

        [DynamicRange(type: typeof(decimal), minimum: "0.1", maximum: nameof(MaximumWeight))]
        public decimal? MinimumWeight { get; set; }

        [DynamicRange(type: typeof(decimal), minimum: nameof(MinimumWeight), maximum: "5.2")]
        public decimal? MaximumWeight { get; set; }
    }

    public class ModelWrongMaximumPropertyType
    {
        public ModelWrongMaximumPropertyType(decimal? minimumWeight, double? maximumWeight)
        {
            MinimumWeight = minimumWeight;
            MaximumWeight = maximumWeight;
        }

        [DynamicRange(type: typeof(decimal), minimum: "0.1", maximum: nameof(MaximumWeight))]
        public decimal? MinimumWeight { get; set; }

        public double? MaximumWeight { get; set; }
    }

    public class ModelWrongMinimumPropertyType
    {
        public ModelWrongMinimumPropertyType(double? minimumWeight, decimal? maximumWeight)
        {
            MinimumWeight = minimumWeight;
            MaximumWeight = maximumWeight;
        }

        public double? MinimumWeight { get; set; }

        [DynamicRange(type: typeof(decimal), minimum: nameof(MinimumWeight), maximum: "5.2")]
        public decimal? MaximumWeight { get; set; }
    }
}