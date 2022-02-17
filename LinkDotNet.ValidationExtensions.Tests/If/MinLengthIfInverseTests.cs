using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests;

public class MinLengthIfInverseTests
{
    [Fact]
    public void ShouldCheckForMinIfNotRequired()
    {
        var model = new Model("1", false);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldCheckWhenConditionIsNotMet()
    {
        var model = new Model("1", true);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeFalse();
        results.Should().HaveCount(1);
        results.Single().ErrorMessage.Should().Be("The field 'Name' must be a string or array type with a minimum length of '10' when 'IsRequired' is not 'False'.");
    }

    public class Model
    {
        public Model(string name, bool isRequired)
        {
            Name = name;
            IsRequired = isRequired;
        }

        [MinLengthIf(nameof(IsRequired), false, 10, true)]
        public string Name { get; set; }

        public bool IsRequired { get; set; }
    }
}