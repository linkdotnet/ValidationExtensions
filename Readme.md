# ValidationExtensions

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

Here we this small library comes into play:
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