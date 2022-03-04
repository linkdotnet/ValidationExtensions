using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests;

public class RequiredDynamicTests
{
    [Fact(DisplayName = "Should not be valid if mobile number is null")]
    public void ShouldNotBeValidIfMobileNumberIsNull()
    {
        var model = new Model(null, "John", "Wick", false, null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        foreach (var result in results)
        {
            Debug.WriteLine(result.ErrorMessage);
        }

        isValid.Should().BeFalse();
    }

    [Fact(DisplayName = "Should not be valid if firstname and surname is null")]
    public void ShouldNotBeValidIfFirstnameAndSurnameIsNull()
    {
        var model = new Model("+15854380259", null, null, false, null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        foreach (var result in results)
        {
            Debug.WriteLine(result.ErrorMessage);
        }

        isValid.Should().BeFalse();
    }

    [Fact(DisplayName = "Should be valid if firstname is not null")]
    public void ShouldBeValidIfFirstnameIsNotNull()
    {
        var model = new Model("+15854380259", "John", null, false, null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        foreach (var result in results)
        {
            Debug.WriteLine(result.ErrorMessage);
        }

        isValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Should be valid if surname is not null")]
    public void ShouldBeValidIfSurnameIsNotNull()
    {
        var model = new Model("+15854380259", null, "Wick", false, null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        foreach (var result in results)
        {
            Debug.WriteLine(result.ErrorMessage);
        }

        isValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Should not be valid if notice by email is null")]
    public void ShouldNotBeValidIfNoticeByEmailIsNull()
    {
        var model = new Model("+15854380259", "John", "Wick", null, null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        foreach (var result in results)
        {
            Debug.WriteLine(result.ErrorMessage);
        }

        isValid.Should().BeFalse();
    }

    [Fact(DisplayName = "Should not be valid if notice by email is activated and email address is null")]
    public void ShouldNotBeValidIfNoticeByEmailIsActivatedAndEmailAddressIsNull()
    {
        var model = new Model("+15854380259", "John", "Wick", true, null);
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        foreach (var result in results)
        {
            Debug.WriteLine(result.ErrorMessage);
        }

        isValid.Should().BeFalse();
    }

    public class Model
    {
        public Model(string? mobileNumber, string? firstname, string? surname, bool? noticeByEmail, string? emailAddress)
        {
            MobileNumber = mobileNumber;
            Firstname = firstname;
            FirstnameStaticPublic = firstname;
            FirstnameInstancePublic = firstname;
            FirstnameInstanceNonePublic = firstname;
            Surname = surname;
            NoticeByEmail = noticeByEmail;
            EmailAddress = emailAddress;
        }

        [Required]
        public string? MobileNumber { get; set; }

        [RequiredDynamic(nameof(ValidateRequiredFullname), "Fullname can't be empty (Static, NonePublic)")]
        public string? Firstname { get; set; }

        [RequiredDynamic(nameof(ValidateRequiredFullnameStaticPublic), "Fullname can't be empty (Static, Public)")]
        public string? FirstnameStaticPublic { get; set; }

        [RequiredDynamic(nameof(ValidateRequiredFullnameInstancePublic), "Fullname can't be empty (Instance, Public)")]
        public string? FirstnameInstancePublic { get; set; }

        [RequiredDynamic(nameof(ValidateRequiredFullnameInstanceNonePublic), "Fullname can't be empty (Instance, NonePublic)")]
        public string? FirstnameInstanceNonePublic { get; set; }

        [RequiredDynamic(nameof(ValidateRequiredFullname), "Fullname can't be empty")]
        public string? Surname { get; set; }

        [Required]
        public bool? NoticeByEmail { get; set; }

        [RequiredDynamic(nameof(ValidateRequiredNoticeByEmail), "Notice by email is activated")]
        public string? EmailAddress { get; set; }

        public static bool ValidateRequiredFullname(Model value)
        {
            if (string.IsNullOrWhiteSpace(value.Firstname) && string.IsNullOrWhiteSpace(value.Surname))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ValidateRequiredFullnameInstanceNonePublic(Model value) => ValidateRequiredFullname(value);

        private static bool ValidateRequiredNoticeByEmail(Model value)
        {
            if (!value.NoticeByEmail.HasValue)
            {
                return false;
            }

            if (!value.NoticeByEmail.Value)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(value.EmailAddress))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ValidateRequiredFullnameStaticPublic(Model value) => ValidateRequiredFullname(value);

        private bool ValidateRequiredFullnameInstancePublic(Model value) => ValidateRequiredFullname(value);
    }
}
