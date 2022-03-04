using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests;

public class RequiredDynamicTests
{
    [Fact]
    public void Should_not_be_valid_if_MobileNumber_is_null()
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

    [Fact]
    public void Should_not_be_valid_if_Firstname_and_Surname_is_null()
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

    [Fact]
    public void Should_be_valid_if_Firstname_is_not_null()
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

    [Fact]
    public void Should_be_valid_if_Surname_is_not_null()
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

    [Fact]
    public void Should_not_be_valid_if_notice_by_email_is_null()
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

    [Fact]
    public void Should_not_be_valid_if_notice_by_email_is_activated_and_email_address_is_null()
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
            Firstname_StaticPublic = firstname;
            Firstname_InstancePublic = firstname;
            Firstname_InstanceNonePublic = firstname;
            Surname = surname;
            NoticeByEmail = noticeByEmail;
            EmailAddress = emailAddress;
        }

        [Required]
        public string? MobileNumber { get; set; }

        [RequiredDynamic(nameof(ValidateRequired_Fullname), "Fullname can't be empty (Static, NonePublic)")]
        public string? Firstname { get; set; }

        [RequiredDynamic(nameof(ValidateRequired_Fullname_StaticPublic), "Fullname can't be empty (Static, Public)")]
        public string? Firstname_StaticPublic { get; set; }

        [RequiredDynamic(nameof(ValidateRequired_Fullname_InstancePublic), "Fullname can't be empty (Instance, Public)")]
        public string? Firstname_InstancePublic { get; set; }

        [RequiredDynamic(nameof(ValidateRequired_Fullname_InstanceNonePublic), "Fullname can't be empty (Instance, NonePublic)")]
        public string? Firstname_InstanceNonePublic { get; set; }

        [RequiredDynamic(nameof(ValidateRequired_Fullname), "Fullname can't be empty")]
        public string? Surname { get; set; }

        [Required]
        public bool? NoticeByEmail { get; set; }

        [RequiredDynamic(nameof(ValidateRequired_NoticeByEmail), "Notice by email is activated")]
        public string? EmailAddress { get; set; }

        public static bool ValidateRequired_Fullname(Model value)
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

        public bool ValidateRequired_Fullname_InstanceNonePublic(Model value) => ValidateRequired_Fullname(value);

        private static bool ValidateRequired_Fullname_StaticPublic(Model value) => ValidateRequired_Fullname(value);

        private bool ValidateRequired_Fullname_InstancePublic(Model value) => ValidateRequired_Fullname(value);

        private static bool ValidateRequired_NoticeByEmail(Model value)
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
    }
}
