using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests;

public class MaxLengthIfTests
{
    [Fact]
    public void ShouldNotCheckForMaxIfNotRequired()
    {
        var model = new Model("123", false);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldCheckWhenConditionIsNotMet()
    {
        var model = new Model("123", true);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        isValid.Should().BeFalse();
        results.Should().HaveCount(1);
        results.Single().ErrorMessage.Should().Be("The field 'Name' must be a string or array type with a maximum length of '1' when 'IsRequired' is 'True'.");
    }

    public class Model
    {
        public Model(string name, bool isRequired)
        {
            Name = name;
            IsRequired = isRequired;
        }

        [MaxLengthIf(nameof(IsRequired), true, 1)]
        public string Name { get; set; }

        public bool IsRequired { get; set; }
    }
}