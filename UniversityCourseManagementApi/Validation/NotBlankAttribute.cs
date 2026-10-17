using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.Validation;

public class NotBlankAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is string text &&
               !string.IsNullOrWhiteSpace(text);
    }
}