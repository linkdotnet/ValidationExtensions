using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.ValidationExtensions.Tests.Generic;

public class DynamicRangeBuiltInTypesTests
{
    [Theory]
    [MemberData(nameof(TestData))]
    public static void TestAllBuiltInTypes<T>(T minimumRange, T minimum, T maximum, T maximumRange, bool minimumIsValid, bool maximumIsValid)
       where T : IComparable<T>
    {
        var data = new Model<T>(minimum, maximum);
        var context = new ValidationContext(data);
        var minimumAttribute = new DynamicRangeAttribute<T>(minimumRange, "Maximum");
        var maximumAttribute = new DynamicRangeAttribute<T>("Minimum", maximumRange);

        var minimumValidation = minimumAttribute.GetValidationResult(minimum, context);
        var maximumValidation = maximumAttribute.GetValidationResult(maximum, context);

        if (minimumIsValid)
        {
            minimumValidation.Should().Be(ValidationResult.Success);
        }
        else
        {
            minimumValidation.Should().NotBe(ValidationResult.Success);
        }

        if (maximumIsValid)
        {
            maximumValidation.Should().Be(ValidationResult.Success);
        }
        else
        {
            maximumValidation.Should().NotBe(ValidationResult.Success);
        }
    }

    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
    public static IEnumerable<object[]> TestData()
    {
        // bool - it's weird but 'bool' type is IComparable<bool>
        //        minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { true, false, true, false, false, false };
        yield return new object[] { false, false, true, true, true, true };

        // sbyte - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { (sbyte)3, (sbyte)2, (sbyte)5, (sbyte)4, false, false };
        yield return new object[] { (sbyte)1, (sbyte)2, (sbyte)5, (sbyte)6, true, true };
        yield return new object[] { (sbyte)2, (sbyte)2, (sbyte)5, (sbyte)5, true, true };
        yield return new object[] { (sbyte)2, (sbyte)2, (sbyte)2, (sbyte)2, true, true };

        // byte - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { (byte)3, (byte)2, (byte)5, (byte)4, false, false };
        yield return new object[] { (byte)1, (byte)2, (byte)5, (byte)6, true, true };

        // short - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { (short)3, (short)2, (short)5, (short)4, false, false };
        yield return new object[] { (short)1, (short)2, (short)5, (short)6, true, true };

        // ushort - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { (ushort)3, (ushort)2, (ushort)5, (ushort)4, false, false };
        yield return new object[] { (ushort)1, (ushort)2, (ushort)5, (ushort)6, true, true };

        // int - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { 3, 2, 5, 4, false, false };
        yield return new object[] { 1, 2, 5, 6, true, true };

        // uint - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { 3u, 2u, 5u, 4u, false, false };
        yield return new object[] { 1u, 2u, 5u, 6u, true, true };

        // long - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { 3L, 2L, 5L, 4L, false, false };
        yield return new object[] { 1L, 2L, 5L, 6L, true, true };

        // ulong - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { 3ul, 2ul, 5ul, 4ul, false, false };
        yield return new object[] { 1ul, 2ul, 5ul, 6ul, true, true };

        // char - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { 'c', 'b', 'f', 'e', false, false };
        yield return new object[] { 'a', 'b', 'f', 'g', true, true };

        // float - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { 3.2f, 2.2f, 5.2f, 4.2f, false, false };
        yield return new object[] { 1.2f, 2.2f, 5.2f, 6.2f, true, true };

        // double - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { 3.2, 2.2, 5.2, 4.2, false, false };
        yield return new object[] { 1.2, 2.2, 5.2, 6.2, true, true };

        // decimal - minimumRange <= minimum <= maximum <= maximumRange, minimumIsValid, maximumIsValid
        yield return new object[] { 3.2m, 2.2m, 5.2m, 4.2m, false, false };
        yield return new object[] { 1.2m, 2.2m, 5.2m, 6.2m, true, true };
    }

    public class Model<T>
         where T : IComparable<T>
    {
        public Model(T? minimum, T? maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public T? Minimum { get; set; }

        public T? Maximum { get; set; }
    }
}