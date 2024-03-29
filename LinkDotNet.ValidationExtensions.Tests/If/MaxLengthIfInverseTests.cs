﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests;

public class MaxLengthIfNotTests
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
        results.Single().ErrorMessage.Should().Be("The field 'Name' must be a string or array type with a maximum length of '1' when 'IsRequired' is not 'False'.");
    }

    public class Model
    {
        public Model(string name, bool isRequired)
        {
            Name = name;
            IsRequired = isRequired;
        }

        [MaxLengthIf(nameof(IsRequired), false, 1, true)]
        public string Name { get; set; }

        public bool IsRequired { get; set; }
    }
}