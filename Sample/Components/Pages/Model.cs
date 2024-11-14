using System.ComponentModel.DataAnnotations;
using LinkDotNet.ValidationExtensions;

namespace Sample.Components.Pages;

public class Model
{
    [Required]
    public required string FirstName { get; set; }
    
    [RequiredIf(nameof(FirstName), "Steven")] 
    public string? LastName { get; set; }
}