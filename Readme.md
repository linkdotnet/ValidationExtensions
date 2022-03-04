# ValidationExtensions

[![.NET](https://github.com/linkdotnet/ValidationExtensions/actions/workflows/dotnet.yml/badge.svg)](https://github.com/linkdotnet/ValidationExtensions/actions/workflows/dotnet.yml)
[![nuget](https://img.shields.io/nuget/v/LinkDotNet.ValidationExtensions)](https://www.nuget.org/packages/LinkDotNet.ValidationExtensions)

The motivation behind this small project is simple. Just imagine you have the following model in Blazor:
```csharp
public class MyModel
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public bool IsPublished { get; set; }
}
```

Now as a consumer you have to provide all of those 3 values. That is all good and nice, but what if we want to say:
"Okay as long as it doesn't get published, we don't have to provide the content?". Well that does not work with the default implementation.

Here is where this small library comes into play:
```csharp
public class MyModel
{
    [Required]
    public string Title { get; set; }

    [RequiredIf(nameof(IsPublished), true)]
    public string Content { get; set; }

    [Required]
    public bool IsPublished { get; set; }
}
```

Now `Title` will always be required. But as long as `IsPublished` is false `Content` can be null or empty.

## Get Started
To install either go the [nuget](https://www.nuget.org/packages/LinkDotNet.ValidationExtensions) or execute the following command:
```
dotnet add LinkDotNet.ValidationExtensions
```

## Example
```csharp
using LinkDotNet.ValidationExtensions;

public class BlogArticle
{
    [Required]
    public string Title { get; set; }

    [RequiredIf(nameof(IsPublished), true)]
    public string ArticleContent { get; set; }

    [RequiredIfNot(nameof(ArticleContent), null)]
    public string ReplacementContent { get; set; }
    
    [Required]
    public bool? NoticeByEmail { get; set; }

    [RequiredDynamic(nameof(ValidateRequired_NoticeByEmail), "Notice by email is activated")]
    public string? EmailAddress { get; set; }
    
    private static bool ValidateRequired_NoticeByEmail(BlogArticle value)
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
```

## Currently implemented additional attributes:
 * `RequiredIf`
 * `MinLengthIf` / `MaxLengthIf`
 * `RangeIf`
 * `MinIf` / `MaxIf`
 * `Min` / `Max`
 * `Dynamic`
